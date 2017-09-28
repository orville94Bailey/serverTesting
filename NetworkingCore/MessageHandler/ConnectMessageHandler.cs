using NetworkingCore.SharedStateObjects;
using System;

namespace NetworkingCore
{
    public class ConnectMessageHandler : BaseMessageHandler
    {
        public Guid ClientGuid { get; set; }

        public override Type HandledMessageType
        {
            get { return typeof(ConnectMessage); }
        }

        public override void ClientProcessMessage(IMessage message)
        {
            throw new NotImplementedException();
            /* assign guid to the client
             * client will then echo back the guid to the server
             */
        }

        public override void ServerProcessMessage(IMessage message, object o)
        {
            /* Here we will send the client it's guid
             */

            var SharedStateObj = (ServerSharedStateObject)o;

            lock (SharedStateObj.ClientQueue)
            {
                SharedStateObj.ClientQueue.Add(Guid.NewGuid,)
            }
        }
    }
}
