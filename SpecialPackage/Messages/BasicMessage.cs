using MessagingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultPackage.Messages
{
    public class BasicMessage : IMessage
    {
        public string Data { get; set; }

        public BasicMessage(string data)
        {
            this.Data = data;
        }

        public void ProcessMessage()
        {
            Console.WriteLine(this.Data);
        }
    }
}
