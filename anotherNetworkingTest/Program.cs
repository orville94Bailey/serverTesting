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
            server.StartListening();
            return 0;
        }
    }
}

