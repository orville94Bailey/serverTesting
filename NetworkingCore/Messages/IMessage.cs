using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore
{
    public interface IMessage
    {
        void ClientProcessMessage(params object[] argsList);
        void ServerProcessMessage(params object[] argsList);
    }
}
