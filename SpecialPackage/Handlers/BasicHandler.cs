using SpecialPackage.Messages;
using System;
using NetworkingCore;

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

        public override void ClientProcessMessage(IMessage message)
        {
            Console.WriteLine(HandlerData);
            message.ClientProcessMessage();
        }

        public override void ServerProcessMessage(IMessage message, object os)
        {
            Console.WriteLine(HandlerData);
            message.ServerProcessMessage(); 
        }

        public BasicMessageHandler()
        {

        }
    }
}
