using NetworkingCore;
using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using NetworkingCore.SharedStateObjects;

namespace SpecialPackage.Messages
{
    public class SpecialMessage : BaseMessage
    {
        public string Data { get; set; }

        public SpecialMessage(Guid clientID, string data) : base(clientID)
        {
            this.Data = data;
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
