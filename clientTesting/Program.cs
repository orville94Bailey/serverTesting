using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Threading;

namespace Client
{
    class Program
    {
        private const int portNum = 55555;

        static void Main(string[] args)
        {
            IPAddress address;
            Client testClient;

            try
            {
                address = IPAddress.Parse("127.0.0.1");
            }
            catch (Exception e)
            {

                throw;
            }

            testClient = new Client(address, portNum);

            while(testClient.SharedStateObj.ClientID.Equals(Guid.Empty))
            {
                testClient.HandleMessages();
                Thread.Sleep(100);
            }

            string data;
            while (testClient.SharedStateObj.ContinueProcess)
            {
                #region Process messages
                testClient.HandleMessages();
                #endregion

                #region Build Messages
                data = Console.ReadLine();

                if (!data.ToUpper().Equals("QUIT"))
                {
                    testClient.EnqueueMessage(new DefaultPackage.Messages.BasicMessage(testClient.SharedStateObj.ClientID, data));
                }
                else
                {
                    testClient.SharedStateObj.ContinueProcess = false;
                }
                #endregion
            }
        }
    }
}
