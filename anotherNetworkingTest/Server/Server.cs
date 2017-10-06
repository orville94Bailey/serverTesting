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
using DefaultPackage.Messages;
using NetworkingCore.SharedStateObjects;
using NetworkingCore;
using NetworkingCore.Messages;

namespace anotherNetworkingTest.Server
{
    class Server
    {
        /* Server needs a threadpool to handle incomming connections.
         * A Listening thread
         * A Reclaim thread
         * A way to stop the server
         */
        private const int portNum = 55555;
        private static ServerSharedStateObject SharedStateObj;

        public static void StartListening()
        {
            SharedStateObj = new ServerSharedStateObject()
            {
                ContinueProcess = true,
                NumberOfClients = 0,
                Ev = new AutoResetEvent(false),
                InBoundMessageQueue = new Queue<BaseMessage>(),
                ClientQueue = new Dictionary<Guid, NetworkStream>(),
                OutBoundMessageQueue = new Queue<ServerMessageWrapper>()
            };

            // Console.WriteLine(System.Net.IPAddress.Parse("127.0.0.1"));

            TcpListener Listener = new TcpListener( System.Net.IPAddress.Any ,portNum);

            try
            {
                Listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listeneing for new connections
                Console.WriteLine("Waiting for a connection...");

                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageQueueProcessor.Process), SharedStateObj);
                ThreadPool.QueueUserWorkItem(new WaitCallback(OutBoundMessageQueueProcessor.Process), SharedStateObj);

                while (TestingCycle > 0)
                {
                    TcpClient handler = Listener.AcceptTcpClient();

                    if(handler != null)
                    {
                        Console.WriteLine("Client# {0} accepted!", ++ClientNbr);
                        //An incoming connection needs to be processed
                        ClientHandler client = new ClientHandler(handler);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(client.Process), SharedStateObj);

                        --TestingCycle;
                    }
                    else
                        break;

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

    class MessageQueueProcessor
    {
        public static void Process(Object o)
        {
            ServerSharedStateObject SharedStateObj = (ServerSharedStateObject)o;

            Dictionary<Type, List<BaseMessageHandler>> handlerList = new Dictionary<Type, List<BaseMessageHandler>>();


            // Get a list of all types in the default dll assembly.
            var currentPackage = ConfigurationManager.AppSettings["RulesPackage"];
            var holder = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + currentPackage + @".dll").GetTypes().ToList(); 

            List<BaseMessageHandler> handlerHolder = new List<BaseMessageHandler>();


            foreach (var item in holder)
            {
                // Go through each type and check to see if it extends BaseMessageHandler and that it has a parameterless constructor
                if(item.BaseType.Equals(typeof(BaseMessageHandler)) && !item.GetConstructor(Type.EmptyTypes).Equals(null))
                {
                    // Instanciates objects that match the above criterium
                    handlerHolder.Add((BaseMessageHandler)Activator.CreateInstance(item));
                }
            }

            //build dictionary of lists based on type of messages
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
                    Console.WriteLine("################\n################");
                    Console.WriteLine("Message queue processing\n");
                    lock (SharedStateObj.InBoundMessageQueue)
                    {
                        foreach (var item in SharedStateObj.InBoundMessageQueue)
                        {
                            if(handlerList.Keys.Contains(item.GetType()))
                            {
                                foreach (var handler in handlerList[item.GetType()])
                                {
                                    handler.ServerProcessMessage(item, SharedStateObj);
                                }
                            }
                        }
                        SharedStateObj.InBoundMessageQueue.Clear();
                    }
                }
            }        
        }
    }

    class OutBoundMessageQueueProcessor
    {
        public static void Process(object o)
        {
            ServerSharedStateObject SharedStateObj = (ServerSharedStateObject)o;

            while (SharedStateObj.ContinueProcess)
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
                //if (SharedStateObj.OutBoundMessageQueue.Count > 0)
                //{
                //    foreach (var message in SharedStateObj.OutBoundMessageQueue)
                //    {
                //        lock (SharedStateObj.OutBoundMessageQueue)
                //        {
                //            foreach (var recipient in message.TargetClients)
                //            {
                //                if(SharedStateObj.ClientQueue.ContainsKey(recipient))
                //                {
                //                    var jsonMessage = JsonConvert.SerializeObject(message.MessageToSend, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
                //                    Byte[] sendBytes = Encoding.ASCII.GetBytes(jsonMessage);

                //                    SharedStateObj.ClientQueue[recipient].Write(sendBytes, 0, sendBytes.Length);
                //                }
                //            }
                //            SharedStateObj.OutBoundMessageQueue.Dequeue();
                //        }
                //    }
                //}
            }
        }
    }

    class ClientHandler
    {
        TcpClient ClientSocket;

        public ClientHandler (TcpClient ClientSocket)
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
                                MessageToSend = new ConnectMessage { clientID = guidHolder }
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

                            // show the data in the console
                            Console.WriteLine("Text Received: {0}", data);
                            lock (SharedStateObj.InBoundMessageQueue)
                            {
                                SharedStateObj.InBoundMessageQueue.Enqueue(receivedMessage);
                            }

                            if (data.Equals("quit")) break;
                        }
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
