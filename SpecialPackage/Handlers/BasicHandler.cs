using DefaultPackage.Messages;
using MessagingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultPackage.Handlers
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

        public override void ProcessMessage(IMessage message)
        {
            Console.WriteLine(HandlerData);
            message.ProcessMessage();
        }

        public BasicMessageHandler()
        {

        }
    }
}
