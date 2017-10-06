using SpecialPackage.Messages;
using System;
using NetworkingCore;
using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;

namespace SpecialPackage.Handlers
{
    public class BasicMessageHandler : BaseMessageHandler
    {
        /* Before messages make it here they need to be deserialized and turned back into an object.
         */
        public string HandlerData
        {
            get
            {
                return "SpecialBasicHandler";
            }
            set
            {
                ;
            }
        }

        public override Type HandledMessageType
        {
            get { return typeof(BasicMessage); }
        }

        //public override void ClientProcessMessage(IMessage message)
        //{
        //    Console.WriteLine(HandlerData);
        //    message.ClientProcessMessage();
        //}

        //public override void ServerProcessMessage(IMessage message, object os)
        //{
        //    Console.WriteLine(HandlerData);
        //    message.ServerProcessMessage(); 
        //}

        public override void ClientProcessMessage(BaseMessage message, ClientSharedStateObject sharedStateObj)
        {
            Console.WriteLine(HandlerData);
            message.ClientProcessMessage(sharedStateObj);
        }

        public override void ServerProcessMessage(BaseMessage message, object o)
        {
            Console.WriteLine(HandlerData);
            //message.ServerProcessMessage();
        }

        public BasicMessageHandler()
        {

        }
    }
}
