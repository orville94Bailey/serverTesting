using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;

namespace NetworkingCore.MessageHandler
{
    class RoutedMessageHandler : BaseMessageHandler
    {
        public override Type HandledMessageType
        {
            get { return typeof(RoutedMessage); }
        }

        public override void ClientProcessMessage(BaseMessage message, ClientSharedStateObject sharedStateObj)
        {
            message.ClientProcessMessage(sharedStateObj);
        }

        public override void ServerProcessMessage(BaseMessage message, ServerSharedStateObject sharedStateObj)
        {
            message.ServerProcessMessage(sharedStateObj);
        }
    }
}
