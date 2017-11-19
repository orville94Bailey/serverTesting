using NetworkingCore.SharedStateObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore.Messages
{
    [JsonObject(MemberSerialization.Fields)]
    abstract public class BaseMessage
    {
        [JsonProperty]
        public Guid Sender { get; private set; }
        abstract public void ClientProcessMessage(ClientSharedStateObject SharedStateObj);
        abstract public void ServerProcessMessage(ServerSharedStateObject SharedStateObj);

        public BaseMessage(Guid clientID)
        {
            this.Sender = clientID;
        }
    }
}
