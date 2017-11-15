using DefaultPackage.Messages;
using NetworkingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkingCore.SharedStateObjects;
using NetworkingCore.Messages;

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

        public override void ClientProcessMessage(BaseMessage message, ClientSharedStateObject sharedStateObj)
        {
            throw new NotImplementedException();
        }

        public override void ServerProcessMessage(BaseMessage message, ServerSharedStateObject sharedStateObj)
        {
            message.ServerProcessMessage(sharedStateObj);
        }

        public BasicMessageHandler()
        {

        }
    }
}
