
using U3DFrame.Manager;
namespace U3DFrame.Action
{
    public interface BaseAction
    {
        /// <summary>
        /// ���շ�������Ϣ
        /// </summary>
        /// <param name="respData"></param>
        // void DoProcess(JSONObject json);
        void DoProcess(BaseDto dto);
        void SetActionMgr(ActionMgr actionMgr);
        void SetDoProcessEndDelegate(ParamDtoDelegate doProcessEnd);
    }
}