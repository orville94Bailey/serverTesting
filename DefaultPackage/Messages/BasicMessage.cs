using MessagingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultPackage.Messages
{
    class BasicMessage : IMessage
    {
        public bool data { get; set; }

        public void ProcessMessage()
        {
            Console.WriteLine(this.data);
        }
    }
}
