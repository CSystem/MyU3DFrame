using System;
using U3DFrame.Interface;
using UnityEngine;
namespace U3DFrame.Manager
{
    public class Manager : MonoBehaviour ,IManager,IDisposable
    {
        protected static readonly object m_syncRoot = new object();

        protected Manager()
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void Register(IReciever iReciever, ushort[] msgId)
        {

            
        }

        public virtual void RemoveMessage(IReciever iReciever,ushort[] msgId)
        {

        }

        public virtual void SendMessage(IMessage message)
        {
            
        }

        public virtual void Update()
        {

        }
    }
}
