using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore
{
    public interface IMessage
    {
        string Sender { get; }
        void ProcessMessage(params object[] argsList);
    }
}
