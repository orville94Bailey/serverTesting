using DefaultPackage.Messages;
using NetworkingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkingCore.SharedStateObjects;

namespace DefaultPackage.Handlers
{
    public class BasicMessageHandler : BaseMessageHandler
    {
        /* Before messages make it here they need to be deserialized and turned back into an object.
         */
        public string HandlerData { get { return "BasicHandler"; } }

        public override Type HandledMessageType
        {
            get { return typeof(BasicMessage); }
        }

        //public override void ClientProcessMessage(IMessage message)
        //{
        //    Console.WriteLine(HandlerData);
        //    message.ClientProcessMessage();
        //}

        public override void ServerProcessMessage(IMessage message, object o)
        {
            Console.WriteLine(HandlerData);
            message.ServerProcessMessage();
        }

        public override void ClientProcessMessage(IMessage message, ClientSharedStateObject sharedStateObj)
        {
            Console.WriteLine(HandlerData);
            message.ClientProcessMessage();
        }

        public BasicMessageHandler()
        {

        }
    }
}
