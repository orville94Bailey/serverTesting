using NetworkingCore.SharedStateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore.Messages
{
    abstract public class BaseMessage
    {
        public Guid Sender { get; private set; }
        abstract public void ClientProcessMessage(ClientSharedStateObject SharedStateObj);
        abstract public void ServerProcessMessage(ServerSharedStateObject SharedStateObj);

        public BaseMessage(Guid clientID)
        {
            this.Sender = clientID;
        }
    }
}
