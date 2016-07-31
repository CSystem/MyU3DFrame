using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U3DFrame.Interface
{
    //具有发消息功能的模块
    public interface ISender
    {
        void SendMessage(IMessage message);
    }
}
