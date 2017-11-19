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

        public List<Guid> Recipients { get; set ; }

        public BasicMessage(Guid clientID, string data) : base(clientID)
        {
            Data = data;
        }

        public override void ClientProcessMessage(ClientSharedStateObject SharedStateObj)
        {
            throw new NotImplementedException();
        }

        public override void ServerProcessMessage(ServerSharedStateObject SharedStateObj)
        {
            Console.WriteLine("Received from: " + this.Sender + "\n \t" + "Contained Data: " + Data);
            if (Data.ToUpper().Equals("QUIT"))
            {
                SharedStateObj.ContinueProcess = false;
            }
        }
    }
}
