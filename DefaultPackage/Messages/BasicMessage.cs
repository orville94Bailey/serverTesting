using NetworkingCore;
using System;

namespace DefaultPackage.Messages
{
    public class BasicMessage : IMessage
    {
        public string Data { get; set; }
        public string Sender { get; private set; }

        public BasicMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = sender;
        }

        public void ProcessMessage(params object[] argsList)
        {
            Console.WriteLine(this.Data);
        }
    }
}
