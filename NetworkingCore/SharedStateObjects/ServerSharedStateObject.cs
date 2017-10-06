using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkingCore.SharedStateObjects
{
    public class ServerSharedStateObject
    {
        public bool ContinueProcess;
        public int NumberOfClients;
        public AutoResetEvent Ev;
        public Queue<BaseMessage> InBoundMessageQueue;
        public Dictionary<Guid, NetworkStream> ClientQueue;
        public Queue<ServerMessageWrapper> OutBoundMessageQueue;
        public Guid serverID;
    }
}
