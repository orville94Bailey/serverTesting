using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore.SharedStateObjects
{
    class SharedStateObjectHelper
    {
        public static List<Guid> ForwardMessageToAllButSender(ServerSharedStateObject obj, Guid sender)
        {
            var receiverList = new List<Guid>();

            receiverList = obj.ClientQueue.Keys.Where(x => !x.Equals(sender)).ToList();

            return receiverList;
        }
    }
}
