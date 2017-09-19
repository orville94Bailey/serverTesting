using NetworkingCore;
using System;

namespace DefaultPackage.Messages
{
    public class SpecialMessage : IMessage
    {
        public string Data { get; set; }

        public string Sender { get; private set; }

        public SpecialMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = sender;
        }

        public void ProcessMessage(params object[] argsList)
        {
            Console.WriteLine(Data);
            Console.WriteLine(Data);
        }
    }
}
