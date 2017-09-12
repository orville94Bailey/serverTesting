using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace anotherNetworkingTest.Server
{
    class SharedState
    {
        public bool ContinueProcess;
        public int NumberOfClients;
        public AutoResetEvent Ev;
        public Queue<string> MessageQueue;
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
                MessageQueue = new Queue<string>()
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

                            // show the data in the console
                            Console.WriteLine("Text Received: {0}", data);
                            lock (SharedStateObj.MessageQueue)
                            {
                                SharedStateObj.MessageQueue.Enqueue(data + " " + ClientSocket.Client.RemoteEndPoint);
                            }

                            //Echo the data back to the client
                            byte[] sendingBytes = Encoding.ASCII.GetBytes(data);
                            networkStream.Write(sendingBytes, 0, sendingBytes.Length);

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
