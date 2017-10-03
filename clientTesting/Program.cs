using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DefaultPackage.Messages;
using Newtonsoft.Json;
using System.Net;

namespace clientTesting
{
    class Program
    {
        private const int portNum = 55555;

        static void Main(string[] args)
        {
            IPAddress address;

            try
            {
                address = IPAddress.Parse("127.0.0.1");
            }
            catch (Exception e)
            {

                throw;
            }

            client.Run(address,portNum);
        }
    }
}
