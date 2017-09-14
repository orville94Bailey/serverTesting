using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagingBase;
using DefaultPackage.Messages;

namespace DefaultPackage.Handlers
{
    class SpecialHandler : BaseMessageHandler
    {
        public string HandlerData
        {
            get
            {
                return "SpecialSpecialHandler";
            }
            set
            {
                ;
            }
        }

        public override Type HandledMessageType
        {
            get { return typeof(SpecialMessage); }
        }

        public override void ProcessMessage(IMessage message)
        {
            Console.WriteLine(HandlerData);
            message.ProcessMessage();
        }

        public SpecialHandler()
        {

        }
    }
}
