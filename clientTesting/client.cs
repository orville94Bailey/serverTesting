//using DefaultPackage.Messages;
using SpecialPackage.Messages;
using Newtonsoft.Json;
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

namespace clientTesting
{
    public static class client
    {
        public static ClientSharedStateObject SharedStateObj;

        public static void Run(IPAddress address, int portNum)
        {
            SharedStateObj = new ClientSharedStateObject()
            {
                ContinueProcess = true,
                Ev = new AutoResetEvent(false),
                InBoundMessageQueue = new Queue<BaseMessage>(),
                OutBoundMessageQueue = new Queue<BaseMessage>()
            };

            TcpClient tcpClient = new TcpClient();
            BaseMessage message = null;
            try
            {
                tcpClient.Connect(address, portNum);

                clientListener listener = new clientListener(tcpClient);
                ThreadPool.QueueUserWorkItem(new WaitCallback(listener.Listen), SharedStateObj);

                clientMessageHandler messageHandler = new clientMessageHandler(tcpClient);
                ThreadPool.QueueUserWorkItem(new WaitCallback(messageHandler.ProcessMessages), SharedStateObj);

                clientMessageSender messageSender = new clientMessageSender(tcpClient);
                ThreadPool.QueueUserWorkItem(new WaitCallback(messageSender.SendMessages), SharedStateObj);

                NetworkStream networkStream = tcpClient.GetStream();

                if (networkStream.CanWrite && networkStream.CanRead)
                {
                    string DataToSend = string.Empty;

                    while (!DataToSend.Equals("quit"))
                    {
                        Console.WriteLine("\nType a text to be sent.");
                        DataToSend = Console.ReadLine();

                        

                        if (DataToSend.Length.Equals(0))
                        {
                            break;
                        }

                        if(!DataToSend.Equals("special"))
                        {
                            message = new BasicMessage (DataToSend, tcpClient.Client.LocalEndPoint.ToString());
                        }
                        else
                        {
                            message = new SpecialMessage(DataToSend, tcpClient.Client.LocalEndPoint.ToString());
                        }

                        lock (SharedStateObj.OutBoundMessageQueue)
                        {
                            SharedStateObj.OutBoundMessageQueue.Enqueue(message);
                        }
                    }
                    networkStream.Close();
                    tcpClient.Close();
                }
                else if (!networkStream.CanRead)
                {
                    Console.WriteLine("You can not write data to this stream");
                    tcpClient.Close();
                }
                else if (!networkStream.CanWrite)
                {
                    Console.WriteLine("You can not read data from this stream");
                    tcpClient.Close();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Server not available!");
            }
            catch (IOException)
            {
                Console.WriteLine("Server not available!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    class clientListener
    {
        public TcpClient ClientSocket { get; set; }

        public clientListener(TcpClient clientSocket)
        {
            this.ClientSocket = clientSocket;
        }


        public void Listen(Object o)
        {
            ClientSharedStateObject sharedStateObj = (ClientSharedStateObject)o;

            string data = null;

            BaseMessage receivedMessage = null;

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
                        if(bytesRead > 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, bytesRead);

                            receivedMessage = (BaseMessage)JsonConvert.DeserializeObject(data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

                            lock (sharedStateObj.InBoundMessageQueue)
                            {
                                sharedStateObj.InBoundMessageQueue.Enqueue(receivedMessage);
                            }
                        }
                    }
                    catch (IOException) { } // Timeout 
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

    class clientMessageHandler
    {
        TcpClient clientSocket;

        public clientMessageHandler(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public void ProcessMessages(object o)
        {
            ClientSharedStateObject SharedStateObj = (ClientSharedStateObject)o;

            Dictionary<Type, List<BaseMessageHandler>> handlerList = new Dictionary<Type, List<BaseMessageHandler>>();

            var holder = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "DefaultPackage.dll").GetTypes().ToList();
            holder.AddRange(Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "SpecialPackage.dll").GetTypes().ToList());
            holder.AddRange(Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "NetworkingCore.dll").GetTypes().ToList());

            List<BaseMessageHandler> handlerHolder = new List<BaseMessageHandler>();

            foreach (var item in holder)
            {
                if (item.BaseType != null && item.BaseType.Equals(typeof(BaseMessageHandler)) && !item.GetConstructor(Type.EmptyTypes).Equals(null))
                {
                    handlerHolder.Add((BaseMessageHandler)Activator.CreateInstance(item));
                }
            }

            foreach (var item in handlerHolder)
            {
                if(!handlerList.Keys.Contains(item.HandledMessageType))
                {
                    handlerList.Add(item.HandledMessageType, new List<BaseMessageHandler>());
                }
                handlerList[item.HandledMessageType].Add(item);
            }

            while (SharedStateObj.ContinueProcess)
            {
                if(SharedStateObj.InBoundMessageQueue.Count > 0)
                {
                    lock (SharedStateObj.InBoundMessageQueue)
                    {
                        foreach (var item in SharedStateObj.InBoundMessageQueue)
                        {
                            if (handlerList.Keys.Contains(item.GetType()))
                            {
                                foreach (var handler in handlerList[item.GetType()])
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
    }

    class clientMessageSender
    {
        NetworkStream clientStream;

        public clientMessageSender(TcpClient clientSocket)
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
            }
            clientStream.Close();
        }
    }
}
