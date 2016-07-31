using System.Collections.Generic;
using U3DFrame.UI.Controller;

namespace U3DFrame.UI.Manager
{
    //管理游戏控制层，缓存C#UI的控制类实例，为了控制为单例模式
    public class CUilManager : U3DFrame.Manager.Manager
    {
        private List<CUiBase> _ltUiCtrl = new List<CUiBase>();

        public CUilManager() : base()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            _ltUiCtrl.Clear();
        }

        public T GetCtrl<T>() where T : CUiBase
        {
            T result = default(T);
            for(int i = 0; i < _ltUiCtrl.Count; i++)
            {
                if (null == _ltUiCtrl[i])
                    continue;
                if(typeof(T) == _ltUiCtrl[i].GetType())
                {
                    result = (T)_ltUiCtrl[i];
                }
            }
            if (null == result)
                result = System.Activator.CreateInstance<T>();
            AddCtrl(result);
            return result;
        }

        public int AddCtrl(CUiBase uiCtrl)
        {
            _ltUiCtrl.Add(uiCtrl);
            return _ltUiCtrl.Count;
        }

        public void RemoveCtrl(CUiBase uiCtrl)
        {
            _ltUiCtrl.Remove(uiCtrl);
        }

        public override void Update()
        {
            for(int i = 0; i < _ltUiCtrl.Count; i++)
            {
                if (null == _ltUiCtrl[i])
                    continue;
                _ltUiCtrl[i].Update(0);
            }
        }
    }
}
