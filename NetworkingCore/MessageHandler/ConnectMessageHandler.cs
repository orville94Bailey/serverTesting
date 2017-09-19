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

        public override void ClientProcessMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public override void ServerProcessMessage(IMessage message, object o)
        {
            /* Here we will send the client the necessary information (guid)
             */

            var SharedStateObj = (ServerSharedStateObject)o;
        }
    }
}
