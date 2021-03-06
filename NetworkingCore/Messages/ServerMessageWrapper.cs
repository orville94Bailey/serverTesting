﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore.Messages
{
    public sealed class ServerMessageWrapper
    {
        public BaseMessage MessageToSend { get; set; }
        public List<Guid> TargetClients { get; set; }

        /// <summary>
        /// Creates a new instance of the ServerMessageWrapper Class.
        /// </summary>
        /// <param name="MessageToSend">This is the IMessage that is going to be sent.</param>
        /// <param name="TargetClients">This is the list of clients that will receive the message.</param>
        public ServerMessageWrapper(BaseMessage MessageToSend, List<Guid> TargetClients)
        {
            this.MessageToSend = MessageToSend;
            this.TargetClients = TargetClients;
        }

        public ServerMessageWrapper()
        {
        }
    }
}
