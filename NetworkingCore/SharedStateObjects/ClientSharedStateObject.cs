using NetworkingCore.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetworkingCore.SharedStateObjects
{
    public class ClientSharedStateObject
    {
        public bool ContinueProcess;
        public AutoResetEvent Ev;
        public Queue<BaseMessage> InBoundMessageQueue;
        public Queue<BaseMessage> OutBoundMessageQueue;
        public Guid ClientID;
    }
}
