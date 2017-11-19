//using DefaultPackage.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Reflection;
using NetworkingCore.SharedStateObjects;
using NetworkingCore;
using NetworkingCore.Messages;
using Newtonsoft.Json;
using System.Configuration;

namespace Client
{
    public class Client
    {
        public ClientSharedStateObject SharedStateObj;
        public TcpClient tcpClient { get; set; }
        private Listener listener { get; set; }
        private MessageSender messageSender { get; set; }
        private Dictionary<Type, List<BaseMessageHandler>> handlerDictionary { get; set; }

        public Client(IPAddress iPAddress, int portNum)
        {
            tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect(iPAddress, portNum);

                SharedStateObj = new ClientSharedStateObject()
                {
                    ContinueProcess = true,
                    Ev = new AutoResetEvent(false),
                    InBoundMessageQueue = new Queue<BaseMessage>(),
                    OutBoundMessageQueue = new Queue<BaseMessage>()
                };

                handlerDictionary = new Dictionary<Type, List<BaseMessageHandler>>();

                #region Building message handling dictionary

                var packageHolder = ConfigurationManager.AppSettings["RulesPackages"].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                packageHolder.Add("NetworkingCore");

                List<Type> typeHolder = new List<Type>();
                foreach (var currentPackage in packageHolder)
                {
                    typeHolder.AddRange(Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + currentPackage + @".dll").GetTypes().ToList());
                }

                List<BaseMessageHandler> handlerHolder = new List<BaseMessageHandler>();

                foreach (var item in typeHolder)
                {
                    if (item.BaseType != null && item.BaseType.Equals(typeof(BaseMessageHandler)) && !item.GetConstructor(Type.EmptyTypes).Equals(null))
                    {
                        handlerHolder.Add((BaseMessageHandler)Activator.CreateInstance(item));
                    }
                }

                foreach (var item in handlerHolder)
                {
                    if (!handlerDictionary.Keys.Contains(item.HandledMessageType))
                    {
                        handlerDictionary.Add(item.HandledMessageType, new List<BaseMessageHandler>());
                    }
                    handlerDictionary[item.HandledMessageType].Add(item);
                }

                #endregion

                Listener listener = new Listener(tcpClient);
                ThreadPool.QueueUserWorkItem(new WaitCallback(listener.Listen), SharedStateObj);

                MessageSender messageSender = new MessageSender(tcpClient);
                ThreadPool.QueueUserWorkItem(new WaitCallback(messageSender.SendMessages), SharedStateObj);
                
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server not available!");
            }
            catch (IOException e)
            {
                Console.WriteLine("Server not available!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void EnqueueMessage(BaseMessage baseMessage)
        {
            lock (SharedStateObj.OutBoundMessageQueue)
            {
                SharedStateObj.OutBoundMessageQueue.Enqueue(baseMessage);
            }
        }

        public void HandleMessages()
        {
            if (SharedStateObj.InBoundMessageQueue.Count > 0)
            {
                lock (SharedStateObj.InBoundMessageQueue)
                {
                    foreach (var item in SharedStateObj.InBoundMessageQueue)
                    {
                        if (handlerDictionary.Keys.Contains(item.GetType()))
                        {
                            foreach (var handler in handlerDictionary[item.GetType()])
                            {
                                handler.ClientProcessMessage(item, SharedStateObj);
                            }
                        }
                    }

                    SharedStateObj.InBoundMessageQueue.Clear();
                }
            }
        }
    }

    class MessageSender
    {
        NetworkStream clientStream;

        public MessageSender(TcpClient clientSocket)
        {
            this.clientStream = clientSocket.GetStream();
        }

        public void SendMessages(object o)
        {
            ClientSharedStateObject SharedStateObj = (ClientSharedStateObject)o;

            while (SharedStateObj.ContinueProcess)
            {
                if (SharedStateObj.OutBoundMessageQueue.Count > 0)
                {
                    lock (SharedStateObj.OutBoundMessageQueue)
                    {
                        foreach (var message in SharedStateObj.OutBoundMessageQueue)
                        {
                            lock (clientStream)
                            {
                                var jsonMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

                                Byte[] sendBytes = Encoding.ASCII.GetBytes(jsonMessage);
                                clientStream.Write(sendBytes, 0, sendBytes.Length);
                            }
                        }

                        SharedStateObj.OutBoundMessageQueue.Clear();
                    }
                }
                Thread.Sleep(100);
            }
            clientStream.Close();
        }
    }

    class Listener
    {
        public TcpClient ClientSocket { get; set; }

        public Listener(TcpClient clientSocket)
        {
            this.ClientSocket = clientSocket;
        }

        public void Listen(Object o)
        {
            ClientSharedStateObject sharedStateObj = (ClientSharedStateObject)o;

            string data = null;

            List<BaseMessage> receivedMessages = null;

            byte[] bytes;

            if(ClientSocket != null)
            {
                NetworkStream networkStream = ClientSocket.GetStream();
                ClientSocket.ReceiveTimeout = 100;

                while (sharedStateObj.ContinueProcess)
                {
                    bytes = new byte[ClientSocket.ReceiveBufferSize];
                    try
                    {
                        int bytesRead;
                        lock (networkStream)
                        {
                            bytesRead = networkStream.Read(bytes, 0, (int)ClientSocket.ReceiveBufferSize);
                        }
                        if (bytesRead > 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                            data = data.Replace("}{\"$type\":", "},{\"$type\":");

                            receivedMessages = JsonConvert.DeserializeObject<List<BaseMessage>>("[" + data + "]", new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

                            lock (sharedStateObj.InBoundMessageQueue)
                            {
                                foreach (var message in receivedMessages)
                                {
                                    sharedStateObj.InBoundMessageQueue.Enqueue(message);
                                }
                            }
                        }
                    }
                    catch (IOException) { } // Timeout 
                    catch (JsonSerializationException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Socket is broken.");
                        break;
                    }
                    Thread.Sleep(200);
                }
            }
        }
    }
}
