using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.Interface;
using UnityEngine;
namespace U3DFrame.Core
{
    //消息中心，处理各个模块的事件分发
    public class MessageCenter : MessageCenterBase
    {
        public override void Register(IReciever iReciever, ushort[] msgId)
        {
            base.Register(iReciever, msgId);
        }

        public override void RemoveMessage(IReciever iReciever, ushort[] msgId)
        {
            base.RemoveMessage(iReciever, msgId);
        }

        public override bool HasMessage(ushort msgId)
        {
            return base.HasMessage(msgId);
        }

        public override void Update(double delta)
        {
            base.Update(delta);
        }
    }
}
