using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore
{
    public interface IMessage
    {
        Guid Sender { get; }
        List<Guid> Recipients { get; set; }
        void ClientProcessMessage(params object[] argsList);
        void ServerProcessMessage(params object[] argsList);
    }
}
