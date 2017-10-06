using NetworkingCore;
using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using NetworkingCore.SharedStateObjects;

namespace SpecialPackage.Messages
{
    public class BasicMessage : BaseMessage
    {
        public BasicMessage(Guid clientID, string data) : base(clientID)
        {
            this.Data = data;
        }

        public string Data { get; set; }

        public override void ClientProcessMessage(ClientSharedStateObject SharedStateObj)
        {
            throw new NotImplementedException();
        }

        public override void ServerProcessMessage(ServerSharedStateObject SharedStateObj)
        {
            Console.WriteLine(Data);
        }
    }
}
