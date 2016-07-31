using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.Interface;
using UnityEngine;
namespace U3DFrame.Core
{
    public class MessageCenterBase : IMessageCenter
    {
        protected IDictionary<ushort, IReciever> m_msgSingleRecieverMap;
        protected static readonly object m_syncRoot = new object();
        private static IMessageCenter m_instance;
        private Queue<IMessage> m_iMessageQueue;
        protected MessageCenterBase()
        {
            m_msgSingleRecieverMap = new Dictionary<ushort, IReciever>();
            m_iMessageQueue = new Queue<IMessage>();
        }

        public static IMessageCenter Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_syncRoot)
                    {
                        if (m_instance == null) m_instance = new MessageCenterBase();
                    }
                }
                return m_instance;
            }
        }

        public virtual void Register(IReciever iReciever, ushort[] msgId)
        {
            if (null == iReciever || null == msgId || msgId.Length <= 0)
                return;
            for (int i = 0; i < msgId.Length; i++)
            {
                m_msgSingleRecieverMap[msgId[i]] = iReciever;
            }
        }
        public virtual void RemoveMessage(IReciever iReciever, ushort[] msgId)
        {
            if (null == iReciever || null == msgId || msgId.Length <= 0)
                return;
            lock (m_syncRoot)
            {
                for (int i = 0; i < msgId.Length; i++)
                {
                    if (m_msgSingleRecieverMap.ContainsKey(msgId[i]))
                        m_msgSingleRecieverMap.Remove(msgId[i]);
                }
            }

        }

        public virtual bool HasMessage(ushort msgId)
        {
            return m_msgSingleRecieverMap.ContainsKey(msgId);
        }
     
        public void SendMessage(IMessage message)
        {
            if (null == message)
                return;
//             lock (m_syncRoot)
//             {
//                 if (HasMessage(message.MsgId))
//                 {
//                     IReciever reciever;
//                     m_MsgSingleRecieverMap.TryGetValue(message.MsgId, out reciever);
//                     reciever.ProcessMessage(message);
//                 }
//             }
            m_iMessageQueue.Enqueue(message);

        }

        private void DispatchMessage(IMessage message)
        {
            if (null == message)
                return;
            lock (m_syncRoot)
            {
                if (HasMessage(message.MsgId))
                {
                    IReciever reciever;
                    m_msgSingleRecieverMap.TryGetValue(message.MsgId, out reciever);
                    reciever.ProcessMessage(message);
                }
            }
        }

        public virtual void Update(double delta)
        {
            if(m_iMessageQueue.Count > 0)
            {
                DispatchMessage(m_iMessageQueue.Dequeue());
            }
        }
    }
}
