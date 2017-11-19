using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace anotherNetworkingTest
{
    class Program
    {
        public static int Main(String[] args)
        {
            var server = new Server.Server(55555);
            //server.StartListening();

            while (server.SharedStateObj.ContinueProcess)
            {
                if (server.SharedStateObj.InBoundMessageQueue.Count > 0)
                {
                    server.HandleMessages();
                }
                Thread.Sleep(1000);
            }
            return 0;
        }
    }
}

