using System;
using SpecialPackage.Messages;
using NetworkingCore;

namespace SpecialPackage.Handlers
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
