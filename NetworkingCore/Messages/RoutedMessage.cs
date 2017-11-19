using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkingCore.SharedStateObjects;

namespace NetworkingCore.Messages
{
    public class RoutedMessage : BaseMessage
    {
        public string Data { get; set; }
        public Guid RoutedThrough { get; set; }

        public RoutedMessage(string Data, Guid clientID) : base(clientID)
        {
            this.Data = Data;
        }
        public override void ClientProcessMessage(ClientSharedStateObject SharedStateObj)
        {
            Console.WriteLine("Sent from: " + this.Sender);
            Console.WriteLine("\tData: " + this.Data);
            Console.WriteLine("\tRouted through: " + this.RoutedThrough);
        }

        public override void ServerProcessMessage(ServerSharedStateObject SharedStateObj)
        {
            this.RoutedThrough = SharedStateObj.serverID;
            lock (SharedStateObj.OutBoundMessageQueue)
            {
                SharedStateObj.OutBoundMessageQueue.Enqueue(new ServerMessageWrapper(this, SharedStateObjectHelper.ForwardMessageToAllButSender(SharedStateObj, this.Sender)));
            }

            Console.WriteLine("Sent from: " + this.Sender);
            Console.WriteLine("\tData: " + this.Data);
            Console.WriteLine("\tRouted through: " + this.RoutedThrough);
        }
    }
}
