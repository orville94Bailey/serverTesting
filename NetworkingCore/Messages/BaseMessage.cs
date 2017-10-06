﻿using NetworkingCore.SharedStateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkingCore.Messages
{
    abstract public class BaseMessage
    {
        abstract public void ClientProcessMessage(ClientSharedStateObject SharedStateObj);
        abstract public void ServerProcessMessage(ServerSharedStateObject SharedStateObj);
    }
}