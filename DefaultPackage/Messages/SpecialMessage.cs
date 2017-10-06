using NetworkingCore;
using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using NetworkingCore.SharedStateObjects;

namespace DefaultPackage.Messages
{
    public class SpecialMessage : BaseMessage
    {
        public SpecialMessage(Guid clientID, string data) : base(clientID)
        {
            this.Data = data;
        }

        public string Data { get; set; }

        public List<Guid> Recipients { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
