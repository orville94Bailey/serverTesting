﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingBase
{
    public interface IMessage
    {
        string Sender { get; }
        void ProcessMessage(params object[] argsList);
    }
}
