using NetworkingCore;
using System;

namespace SpecialPackage.Messages
{
    public class BasicMessage : IMessage
    {
        public string Data { get; set; }

        public string Sender { get; private set; }

        public BasicMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = data;
        }

        public void ProcessMessage(params object[] argsList)
        {
            Console.WriteLine(this.Data);
        }
    }
}
