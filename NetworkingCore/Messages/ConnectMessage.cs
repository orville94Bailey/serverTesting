using System;

namespace NetworkingCore
{
    public class ConnectMessage : IMessage
    {
        public string Sender { get; private set; }

        public void ProcessMessage(params object[] argsList)
        {
            throw new NotImplementedException();
        }
    }
}
