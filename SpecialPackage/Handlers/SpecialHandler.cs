﻿using System;
using SpecialPackage.Messages;
using NetworkingCore;
using NetworkingCore.Messages;
using NetworkingCore.SharedStateObjects;

namespace SpecialPackage.Handlers
{
    class SpecialHandler : BaseMessageHandler
    {
        public string HandlerData
        {
            get
            {
                return "SpecialSpecialHandler";
            }
            set
            {
                ;
            }
        }

        public override Type HandledMessageType
        {
            get { return typeof(SpecialMessage); }
        }

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

        public SpecialHandler()
        {

        }
    }
}
