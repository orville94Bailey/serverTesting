using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;
using System;

namespace NetworkingCore
{
    public class ConnectMessageHandler : BaseMessageHandler
    {
        public override Type HandledMessageType
        {
            get { return typeof(ConnectMessage); }
        }

        public override void ClientProcessMessage(BaseMessage message, ClientSharedStateObject sharedStateObj)
        {
            //sharedStateObj.ClientID = message.c
            message.ClientProcessMessage(sharedStateObj);
        }

        public override void ServerProcessMessage(BaseMessage message, object o)
        {
            throw new NotImplementedException();
        }
    }
}
