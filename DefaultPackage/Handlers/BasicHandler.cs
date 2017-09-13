using DefaultPackage.Messages;
using MessagingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultPackage.Handlers
{
    class BasicMessageHandler : BaseMessageHandler
    {
        /* Before messages make it here they need to be deserialized and turned back into an object.
         */
        public override Type HandledMessageType
        {
            get { return typeof(BasicMessage); }
        }
    }
}
