using UnityEngine;
using System.Collections;
using U3DFrame.Enum;
using U3DFrame.Data;

public delegate void VoidVoidDelegate();
public delegate void VoidIntDelegate(int val);
public delegate void VoidTransformDelegate(Transform tf);
public delegate void VoidObjectDelegate(object obj);
public delegate void VoidBoolDelegate(bool val);
public delegate void VoidFloatDelegate(float val);
public delegate void VoidBaseDtoDelegate(BaseDto val);
namespace U3DFrame.Interface
{
    public interface IYxClient
    {
        #region 事件
        void AddEventListener(YxEventType eventType, VoidObjectDelegate callFunc);
        void RemoveEventListener(YxEventType eventType);
        VoidObjectDelegate GetEventListener(YxEventType eventType);
        void DispatchEvent(YxEventType eventType, object param);
        void DispatchToMainThread(VoidObjectDelegate call, object param = null);
        void DispatchToMainThread(VoidIntDelegate call, int val = 0);
        void DispatchToMainThread(ParamDtoDelegate call, BaseDto dto = null);
        #endregion
        #region 管理类获取
//         LogicMgr GetLogicMgr();
//         DtoMgr GetDtoMgr();
//         EventManager GetEventMgr();
//         ActionMgr GetActionMgr();
//         RootMgr GetRootMgr();
//         HTTPClientManager GetHTTPMgr();
//         SocketClientManager GetSocketMgr();
        #endregion
        void FixedUpdate();
        void HandShake(BaseDto dto);
        void SendHTTP(BaseDto dto);
        void SendSocket(BaseDto dto);
        void StartCoroutine(IEnumerator routine);
        YxUnityClient GetUnityClient();
        #region 数据
        void SetYxCfgData(YxCfgData data);
        YxCfgData GetYxCfgData();
        int GetUserId();
#endregion
        #region 状态
        bool IsEnterGame();
        bool IsMultiLogin();
        bool IsThreadSafeMode();
        bool IsMainThread();
        bool IsClientNetAble();
        #endregion
    }

    public class YxUnityClient : MonoBehaviour
    {
        public IYxClient client;
        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
