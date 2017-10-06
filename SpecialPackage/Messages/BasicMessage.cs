using NetworkingCore;
using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using NetworkingCore.SharedStateObjects;

namespace SpecialPackage.Messages
{
    public class BasicMessage : BaseMessage
    {
        public string Data { get; set; }

        public string Sender { get; private set; }

        public List<Guid> Recipients { get; set; }

        public BasicMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = data;
        }

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
