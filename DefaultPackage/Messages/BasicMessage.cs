using NetworkingCore;
using System;
using System.Collections.Generic;

namespace DefaultPackage.Messages
{
    public class BasicMessage : IMessage
    {
        public string Data { get; set; }
        public string Sender { get; private set; }

        Guid IMessage.Sender => throw new NotImplementedException();

        public List<Guid> Recipients { get; set ; }

        public BasicMessage(string data, string sender)
        {
            this.Data = data;
            this.Sender = sender;

        }

        public void ClientProcessMessage(params object[] argsList)
        {
            throw new NotImplementedException();
        }

        public void ServerProcessMessage(params object[] argsList)
        {
            throw new NotImplementedException();
        }
    }
}
