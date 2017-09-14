using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessagingBase;
using System.Reflection;

namespace anotherNetworkingTest.Server
{
    class SharedState
    {
        public bool ContinueProcess;
        public int NumberOfClients;
        public AutoResetEvent Ev;
        public Queue<IMessage> MessageQueue;
        public Queue<TcpClient> ClientQueue;
    }

    class Server
    {
        /* Server needs a threadpool to handle incomming connections.
         * A Listening thread
         * A Reclaim thread
         * A way to stop the server
         */
        private const int portNum = 55555;
        private static SharedState SharedStateObj;

        public static void StartListening()
        {
            SharedStateObj = new SharedState()
            {
                ContinueProcess = true,
                NumberOfClients = 0,
                Ev = new AutoResetEvent(false),
                MessageQueue = new Queue<IMessage>(),
                ClientQueue = new Queue<TcpClient>()
            };

            // Console.WriteLine(System.Net.IPAddress.Parse("127.0.0.1"));

            TcpListener Listener = new TcpListener( System.Net.IPAddress.Parse("127.0.0.1") ,portNum);

            try
            {
                Listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listeneing for new connections
                Console.WriteLine("Waiting for a connection...");

                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageQueueProcessor.Process), SharedStateObj);

                while (TestingCycle > 0)
                {
                    TcpClient handler = Listener.AcceptTcpClient();

                    if(handler != null)
                    {
                        Console.WriteLine("Client# {0} accepted!", ++ClientNbr);
                        SharedStateObj.ClientQueue.Enqueue(handler);
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
            SharedState SharedStateObj = (SharedState)o;

            Dictionary<Type, List<BaseMessageHandler>> handlerList = new Dictionary<Type, List<BaseMessageHandler>>();


            // Get a list of all types in the default dll assembly.
            var holder = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory +  @"DefaultPackage.dll").GetTypes().ToList(); //Assembly.LoadFrom(DefaultPackage) .GetTypes().ToList();

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
                    handlerList[item.HandledMessageType].Add(item);
                }
            }

            while (SharedStateObj.ContinueProcess)
            {
                Thread.Sleep(20000);

                /* Spit out messages after 30 seconds.
                 */


                Console.WriteLine("################\n################");
                Console.WriteLine("Message queue processing\n");
                lock (SharedStateObj.MessageQueue)
                {
                    foreach (var item in SharedStateObj.MessageQueue)
                    {
                        foreach (var handler in handlerList[item.GetType()])
                        {
                            handler.ProcessMessage(item);
                        }
                    }

                    SharedStateObj.MessageQueue.Clear();
                }
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
            Interlocked.Increment(ref ((SharedState)o).NumberOfClients);
            SharedState SharedStateObj = (SharedState)o;

            //incoming data from client
            string data = null;

            IMessage receivedMessage = null;

            //data buffer coming in
            byte[] bytes;

            if(ClientSocket != null)
            {
                NetworkStream networkStream = ClientSocket.GetStream();
                ClientSocket.ReceiveTimeout = 100; // 1000 milliseconds

                while (SharedStateObj.ContinueProcess)
                {
                    bytes = new byte[ClientSocket.ReceiveBufferSize];
                    try
                    {
                        int BytesRead = networkStream.Read(bytes, 0, (int)ClientSocket.ReceiveBufferSize);
                        if (BytesRead > 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, BytesRead);

                            receivedMessage = (IMessage)JsonConvert.DeserializeObject(data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

                            // show the data in the console
                            Console.WriteLine("Text Received: {0}", data);
                            lock (SharedStateObj.MessageQueue)
                            {
                                SharedStateObj.MessageQueue.Enqueue(receivedMessage);
                            }

                            ////Echo the data back to the client
                            //byte[] sendingBytes = Encoding.ASCII.GetBytes(data);
                            //networkStream.Write(sendingBytes, 0, sendingBytes.Length);

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
            foreach (var item in SharedStateObj.MessageQueue)
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
