using System;
using U3DFrame.UI.Manager;

namespace U3DFrame.UI.Controller
{
    //UI逻辑层，调用Designer里实现的接口取控制View的显示
    public abstract class CUiBase : ICUiBase
    {
        protected IUiDesigner m_iDesigner = null;
        protected bool m_bShowing = false;
        protected bool m_bVisible = true;
        protected bool m_bDesignerLoadFinish = false;
        protected int m_nWindowDepth = -1;

        protected abstract void OnDesignerLoadFinish(bool bSuccess);
        public abstract E_UIWINDOW_TYPE UIWindowType();

        public int WindowDepth
        {
            get { return m_nWindowDepth; }
            set
            {
                m_nWindowDepth = value;
                if (m_iDesigner != null)
                {
                    m_iDesigner.SetWindowDepth(m_nWindowDepth);
                }
            }
        }

        public virtual void Close()
        {

            GameApp.Instance.GetManager<CUilManager>(typeof(CUilManager).ToString()).RemoveCtrl(this);
            DestoryDesigner();
            m_bShowing = false;
        }


        public virtual void Open()
        {
            if (m_bShowing)
                return;

            GameApp.Instance.GetManager<CUilManager>(typeof(CUilManager).ToString()).AddCtrl(this);
            m_bShowing = true;
        }

        public virtual void Update(double delta)
        {

        }

        public void DesignerLoadFinish(bool bSuccess)
        {
            m_bDesignerLoadFinish = true;
            OnDesignerLoadFinish(bSuccess);
        }

        public bool Showing()
        {
            return m_bShowing;
        }

        protected bool IsDesignerLoadFinish()
        {
            return m_bDesignerLoadFinish;
        }

        public bool IsOpen()
        {
            return Showing();
        }

        public void SetVisible(bool b)
        {
            m_bVisible = b;
            RefreshVisible();
        }

        virtual public void RefreshVisible()
        {
            if (m_iDesigner != null)
            {
                m_iDesigner.SetVisible(m_bVisible);
            }
        }

        protected void InitDesigner(Type designerType)
        {
            m_iDesigner = (IUiDesigner)System.Activator.CreateInstance(designerType);
            m_iDesigner.SetUIBase(this);
            m_iDesigner.Init(this);
            RefreshVisible();
        }

        protected T InitDesigner<T>(params object[] args) where T : IUiDesigner
        {
            Type designerType = typeof(T);
            m_iDesigner = (T)System.Activator.CreateInstance(designerType, args);
            m_iDesigner.SetUIBase(this);
            m_iDesigner.Init(this);
            RefreshVisible();
            return (T)m_iDesigner;
        }

        protected void DestoryDesigner()
        {
            if (m_iDesigner != null)
            {
                m_iDesigner.Destory();
            }
        }

        protected T GetDesigner<T>()
        {
            return (T)m_iDesigner;
        }
    }
}
