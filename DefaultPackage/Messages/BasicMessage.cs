using NetworkingCore;
using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using NetworkingCore.SharedStateObjects;

namespace DefaultPackage.Messages
{
    public class BasicMessage : BaseMessage
    {
        public string Data { get; set; }
        public string Sender { get; private set; }

        public List<Guid> Recipients { get; set ; }

        public BasicMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = sender;

        }

        public override void ClientProcessMessage(ClientSharedStateObject SharedStateObj)
        {
            throw new NotImplementedException();
        }

        public override void ServerProcessMessage(ServerSharedStateObject SharedStateObj)
        {
            throw new NotImplementedException();
        }
    }
}
