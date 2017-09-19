using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultPackage.Messages;
using NetworkingCore;

namespace DefaultPackage.Handlers
{
    class SpecialHandler : BaseMessageHandler
    {
        public string HandlerData { get { return "SpecialHandler"; } }

        public override Type HandledMessageType
        {
            get { return typeof(SpecialMessage); }
        }

        public override void ClientProcessMessage(IMessage message)
        {
            Console.WriteLine(HandlerData);
            message.ProcessMessage();
        }

        public override void ServerProcessMessage(IMessage message, object o)
        {
            Console.WriteLine(HandlerData);
            message.ProcessMessage();
        }

        public SpecialHandler()
        {

        }
    }
}
