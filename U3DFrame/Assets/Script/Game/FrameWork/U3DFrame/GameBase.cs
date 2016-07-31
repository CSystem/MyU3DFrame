using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.Core;
using U3DFrame.Interface;
using UnityEngine;

namespace U3DFrame.Base
{
    public class GameBase
    {
        static GameObject m_GameManager;
        static IDictionary<string, object> m_Managers; 
        protected IMessageCenter m_MessageCenter;
        protected ResInfoRef _resDownCenter;
        public ResInfoRef ResoureceDownCenter { get { return _resDownCenter; } }
//         public CUilManager CUilManager { get { return GetManager<CUilManager>(typeof(CUilManager).ToString()); } }
//         public ResourceManager ResourceManager { get { return GetManager<ResourceManager>(typeof(ResourceManager).ToString()); } }
//         public AudioManager AudioManager { get { return GetManager<AudioManager>(typeof(AudioManager).ToString()); } }
//         public ThreadManager ThreadManager { get { return GetManager<ThreadManager>(typeof(ThreadManager).ToString()); } }
         public GuiPanelManager GuiPanelManager { get { return GetManager<GuiPanelManager>(typeof(GuiPanelManager).ToString()); } }

        public GameBase()
        {
            m_Managers = new Dictionary<string, object>();
        }

        GameObject AppGameManager
        {
            get
            {
                if (m_GameManager == null)
                {
                    m_GameManager = GameObject.Find("GameManager");
                }
                return m_GameManager;
            }
        }

        public virtual void InitFramework()
        {
            m_MessageCenter = MessageCenter.Instance;
            _resDownCenter = AppGameManager.AddComponent<ResInfoRef>();
            GetManager<GuiPanelManager>(typeof(GuiPanelManager).ToString()).InitGUIManage(_resDownCenter);
        }

        public void AddManager(string name, object obj)
        {
            if (!m_Managers.ContainsKey(name))
            {
                m_Managers.Add(name, obj);
            }
        }

        public T AddManager<T>(string name) where T : MonoBehaviour
        {
            object result = null;
            m_Managers.TryGetValue(name, out result);
            if (result != null)
            {
                return (T)result;
            }
            if(null == AppGameManager)
            {
                return null;
            }

            //result = System.Activator.CreateInstance<T>();
            result = AppGameManager.AddComponent<T>();
            m_Managers.Add(name, result);
            return (T)result;
        }

          
        public T GetManager<T>(string name) where T : MonoBehaviour
        {
            if (!m_Managers.ContainsKey(name))
            {
                return default(T);
            }
            object manager = null;
            m_Managers.TryGetValue(name, out manager);
            return (T)manager;
        }


        public void RemoveManager(string name)
        {
            if (!m_Managers.ContainsKey(name))
            {
                return;
            }
            object manager = null;
            m_Managers.TryGetValue(name, out manager);
            m_Managers.Remove(name);
        }

        //消息
        public virtual void Register(IReciever iReciever, ushort[] msgId)
        {
            m_MessageCenter.Register(iReciever, msgId);
        }

        public virtual void RemoveMessage(IReciever iReciever, ushort[] msgId)
        {
            m_MessageCenter.RemoveMessage(iReciever, msgId);
        }

        public virtual void SendMessage(IMessage message)
        {
            m_MessageCenter.SendMessage(message);
        }

        public virtual void Update(double delta)
        {
            m_MessageCenter.Update(delta);
        }
    }
}
