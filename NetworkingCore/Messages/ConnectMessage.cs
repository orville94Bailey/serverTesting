using System;
using System.Collections.Generic;

namespace NetworkingCore
{
    /// <summary>
    /// The connect message is the first message that will be received after opening a connection with a client.
    /// The client will send this message to the server and the server will reply back with 
    /// </summary>
    public class ConnectMessage : IMessage
    {
        public string Sender { get; private set; }
        public List<Guid> Recipients { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        Guid IMessage.Sender => throw new NotImplementedException();


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
