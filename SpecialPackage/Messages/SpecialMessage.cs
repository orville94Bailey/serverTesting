using MessagingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialPackage.Messages
{
    public class SpecialMessage : IMessage
    {
        public string Data { get; set; }

        public SpecialMessage(string data)
        {
            this.Data = data;
        }

        public void ProcessMessage()
        {
            Console.WriteLine(Data);
            Console.WriteLine(Data);
        }

    }
}
