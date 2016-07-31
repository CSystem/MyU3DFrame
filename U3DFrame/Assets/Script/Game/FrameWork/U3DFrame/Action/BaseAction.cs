
using U3DFrame.Manager;
namespace U3DFrame.Action
{
    public interface BaseAction
    {
        /// <summary>
        /// 接收服务器消息
        /// </summary>
        /// <param name="respData"></param>
        // void DoProcess(JSONObject json);
        void DoProcess(BaseDto dto);
        void SetActionMgr(ActionMgr actionMgr);
        void SetDoProcessEndDelegate(ParamDtoDelegate doProcessEnd);
    }
}