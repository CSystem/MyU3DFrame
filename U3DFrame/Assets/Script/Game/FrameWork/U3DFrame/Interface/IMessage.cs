using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U3DFrame.Interface
{
    //消息体
    public interface IMessage
    {
        ushort MsgId { get; set; }
        string Name { get; }
        object Body { get; set; }
        string Type { get; set; }
        string ToString();
    }
}
