using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Configuration;
using NetworkingCore;
using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;

namespace anotherNetworkingTest.Server
{
    class Server
    {
        //private const int portNum = 55555;
        private int portNum;
        public ServerSharedStateObject SharedStateObj;
        private Dictionary<Type, List<BaseMessageHandler>> handlerDictionary { get; set; }

        public Server(int portNum)
        {
            this.portNum = portNum;
            handlerDictionary = new Dictionary<Type, List<BaseMessageHandler>>();

            SharedStateObj = new ServerSharedStateObject()
            {
                ContinueProcess = true,
                NumberOfClients = 0,
                Ev = new AutoResetEvent(false),
                InBoundMessageQueue = new Queue<BaseMessage>(),
                ClientQueue = new Dictionary<Guid, NetworkStream>(),
                OutBoundMessageQueue = new Queue<ServerMessageWrapper>(),
                serverID = Guid.NewGuid()
            };

            var packageHolder = ConfigurationManager.AppSettings["RulesPackages"].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            packageHolder.Add("NetworkingCore");

            List<Type> typeHolder = new List<Type>();
            foreach (var currentPackage in packageHolder)
            {
                typeHolder.AddRange(Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + currentPackage + @".dll").GetTypes().ToList());
            }

             #region Building message handling dictionary

            List<BaseMessageHandler> handlerHolder = new List<BaseMessageHandler>();

            foreach (var item in typeHolder)
            {
                if(item.BaseType != null)
                {
                    // Go through each type and check to see if it extends BaseMessageHandler and that it has a parameterless constructor
                    if (item.BaseType.Equals(typeof(BaseMessageHandler)) && !item.GetConstructor(Type.EmptyTypes).Equals(null))
                    {
                        // Instanciates objects that match the above criterium
                        handlerHolder.Add((BaseMessageHandler)Activator.CreateInstance(item));
                    }
                }
            }

            //build dictionary of lists based on type of messages
            foreach (var item in handlerHolder)
            {
                if (!handlerDictionary.Keys.Contains(item.HandledMessageType))
                {
                    handlerDictionary.Add(item.HandledMessageType, new List<BaseMessageHandler>());
                }
                handlerDictionary[item.HandledMessageType].Add(item);
            }

            #endregion

            ThreadPool.QueueUserWorkItem(new WaitCallback(MessageSender.Process), SharedStateObj);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ClientConnector.Process),SharedStateObj);
        }

        public void EnqueueMessage(ServerMessageWrapper wrappedMessage)
        {
            lock (SharedStateObj.OutBoundMessageQueue)
            {
                SharedStateObj.OutBoundMessageQueue.Enqueue(wrappedMessage);
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
                                handler.ServerProcessMessage(item, SharedStateObj);
                            }
                        }
                    }
                    SharedStateObj.InBoundMessageQueue.Clear();
                }
            }
        }

        public void StartListening()
        {

            TcpListener Listener = new TcpListener( System.Net.IPAddress.Any ,portNum);

            try
            {
                Listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listeneing for new connections
                Console.WriteLine("Waiting for a connection...");

                while (SharedStateObj.ContinueProcess)
                {
                    TcpClient handler = Listener.AcceptTcpClient();

                    if(handler != null)
                    {
                        Console.WriteLine("Client# {0} accepted!", ++ClientNbr);
                        //An incoming connection needs to be processed
                        Listener client = new Listener(handler);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(client.Process), SharedStateObj);

                        --TestingCycle;
                    }
                    else
                        break;

                    Thread.Sleep(100);

                }

                Listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Stop and wait for all connections to end

            SharedStateObj.ContinueProcess = false;
            SharedStateObj.Ev.WaitOne();

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

    class ClientConnector
    {
        public static void Process(object o)
        {
            ServerSharedStateObject SharedStateObj = (ServerSharedStateObject)o;

            TcpListener Listener = new TcpListener(System.Net.IPAddress.Any, 55555);

            try
            {
                Listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listeneing for new connections
                Console.WriteLine("Waiting for a connection...");

                while (SharedStateObj.ContinueProcess)
                {
                    TcpClient handler = Listener.AcceptTcpClient();

                    if (handler != null)
                    {
                        Console.WriteLine("Client# {0} accepted!", ++ClientNbr);
                        //An incoming connection needs to be processed
                        Listener client = new Listener(handler);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(client.Process), SharedStateObj);

                        --TestingCycle;
                    }
                    else
                        break;

                    Thread.Sleep(100);

                }

                Listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    class MessageSender
    {
        public static void Process(object o)
        {
            ServerSharedStateObject SharedStateObj = (ServerSharedStateObject)o;

            while (SharedStateObj.ContinueProcess)
            {
                if(SharedStateObj.OutBoundMessageQueue.Count > 0)
                {
                    while (SharedStateObj.OutBoundMessageQueue.Count > 0)
                    {
                        lock (SharedStateObj.OutBoundMessageQueue)
                        {
                            var wrappedMessage = SharedStateObj.OutBoundMessageQueue.Dequeue();

                            var jsonMessage = JsonConvert.SerializeObject(wrappedMessage.MessageToSend, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(jsonMessage);

                            foreach (var recipient in wrappedMessage.TargetClients)
                            {
                                if(SharedStateObj.ClientQueue.Keys.Contains(recipient))
                                {
                                    SharedStateObj.ClientQueue[recipient].Write(sendBytes, 0, sendBytes.Length);
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
    }

    class Listener
    {
        TcpClient ClientSocket;

        public Listener (TcpClient ClientSocket)
        {
            this.ClientSocket = ClientSocket;
        }

        public void Process(Object o)
        {
            Interlocked.Increment(ref ((ServerSharedStateObject)o).NumberOfClients);
            ServerSharedStateObject SharedStateObj = (ServerSharedStateObject)o;

            //incoming data from client
            string data = null;

            BaseMessage receivedMessage = null;

            //data buffer coming in
            byte[] bytes;

            if(ClientSocket != null)
            {
                NetworkStream networkStream = ClientSocket.GetStream();
                ClientSocket.ReceiveTimeout = 100; // 1000 milliseconds
                var guidHolder = Guid.NewGuid();

                lock (SharedStateObj.ClientQueue)
                {
                    SharedStateObj.ClientQueue.Add(guidHolder, networkStream);
                }

                lock (SharedStateObj.OutBoundMessageQueue)
                {
                    SharedStateObj.OutBoundMessageQueue.Enqueue
                        (
                            new ServerMessageWrapper
                            {
                                TargetClients = new List<Guid>(new Guid[] { guidHolder }),
                                MessageToSend = new ConnectMessage(SharedStateObj.serverID, guidHolder)
                            }
                        );
                }
                
                while (SharedStateObj.ContinueProcess)
                {
                    bytes = new byte[ClientSocket.ReceiveBufferSize];
                    try
                    {
                        int BytesRead = networkStream.Read(bytes, 0, (int)ClientSocket.ReceiveBufferSize);
                        if (BytesRead > 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, BytesRead);

                            receivedMessage = (BaseMessage)JsonConvert.DeserializeObject(data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                            lock (SharedStateObj.InBoundMessageQueue)
                            {
                                SharedStateObj.InBoundMessageQueue.Enqueue(receivedMessage);
                            }

                            if (data.Equals("quit")) break;
                        }
                        Thread.Sleep(100);
                    }
                    catch (IOException) { }//timeout
                    catch (SocketException)
                    {
                        Console.WriteLine("Connection is broken");
                        break;
                    }
                    Thread.Sleep(200);
                }
                networkStream.Close();
                ClientSocket.Close();
            }
            Interlocked.Decrement(ref SharedStateObj.NumberOfClients);
            Console.WriteLine("A client left, number of connections is {0}", SharedStateObj.NumberOfClients);
            Console.WriteLine("Current state of message queue:");
            foreach (var item in SharedStateObj.InBoundMessageQueue)
            {
                Console.WriteLine(item);
            }

            if(!SharedStateObj.ContinueProcess && SharedStateObj.NumberOfClients == 0)
            {
                SharedStateObj.Ev.Set();
            }
        }
    }
}
