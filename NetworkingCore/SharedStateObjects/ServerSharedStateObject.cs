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
        public Queue<IMessage> MessageQueue;
        public Dictionary<Guid, TcpClient> ClientQueue;
    }
}
