 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore
{
    public abstract class BaseMessageHandler
    {
        /* This will be expanded once we're using this framework in the battlemap
         */
        public virtual Type HandledMessageType
        {
            get { return null; }
        }

        public abstract void ClientProcessMessage(IMessage message);

        public abstract void ServerProcessMessage(IMessage message, object o);

    }
}
