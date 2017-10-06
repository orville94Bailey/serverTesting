using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;
using System;
using System.Collections.Generic;

namespace NetworkingCore
{
    /// <summary>
    /// The connect message is the first message that will be received after opening a connection with a client.
    /// The client will send this message to the server and the server will reply back with 
    /// </summary>
    public class ConnectMessage : BaseMessage
    {
        public Guid clientID { get; set; }

        public override void ClientProcessMessage(ClientSharedStateObject SharedStateObj)
        {
            //SharedStateObj.ClientID = 
        }

        public override void ServerProcessMessage(ServerSharedStateObject SharedStateObj)
        {
            throw new NotImplementedException();
        }
    }
}
