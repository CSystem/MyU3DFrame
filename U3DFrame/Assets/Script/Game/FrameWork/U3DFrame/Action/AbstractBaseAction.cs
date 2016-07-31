using System;
using Com.Youxin.Common.Data;
using U3DFrame.Interface;
using U3DFrame.Manager;
using U3DFrame.Json;
using U3DFrame.Const;
using U3DFrame.Tool;

namespace U3DFrame.Action
{
    public abstract class AbstractBaseAction : BaseAction
    {
        private ActionMgr _actionMgr;
        private ParamDtoDelegate doProcessEnd;
        public void SetDoProcessEndDelegate(ParamDtoDelegate doProcessEnd)
        {
            this.doProcessEnd = doProcessEnd;
        }
        public void SetActionMgr(ActionMgr actionMgr)
        {
            this._actionMgr = actionMgr;
        }
        public ActionMgr actionMgr
        {
            get { return this._actionMgr; }
        }
        public IYxClient client
        {
            get { return this.actionMgr.Client; }
        }
        public EventManager eventManager
        {
            get
            {
                return this.actionMgr.eventManager;
            }
        }
        /// <summary>
        /// 接收服务器消息
        /// </summary>
        /// <param name="respData"></param>
        public virtual void DoProcess(BaseDto dto)
        {
            JSONObject json = dto.resposeJSON;
            if (json.RecdHasKey(BaseConst.NOW_SERVER_TIME))
            {
                BaseData.preServerDateTime = BaseData.nowServerDateTime;
                DateTime preClientDateTime = BaseData.nowClientDateTime;
                BaseData.nowServerTime = json.GetRecd<string>(BaseConst.NOW_SERVER_TIME);
                BaseData.nowServerDateTime = CommonUtils.DateParse(BaseData.nowServerTime);
                BaseData.nowClientDateTime = DateTime.Now;
                double serverTotalSeconds = BaseData.nowServerDateTime.Subtract(BaseData.preServerDateTime).TotalSeconds;
                double clientTotalSeconds = BaseData.nowClientDateTime.Subtract(preClientDateTime).TotalSeconds;
                if (BaseData.initDateTime)
                {
                    double diffSeconds = Math.Abs(serverTotalSeconds - clientTotalSeconds);
                    if (diffSeconds > 3000)
                    {
                        // this.actionMgr.client.timeWrong = true;
                        // TODO 客户端作弊提示关闭程序
                        // this.actionMgr.client.DispatchEvent(YxEventType.TIME_WRONG, null);
                    }
                }
                else
                {
                    BaseData.startServerDateTime = BaseData.nowServerDateTime;
                    BaseData.preChkDayServerDateTime = BaseData.nowServerDateTime;
                }
                BaseData.initDateTime = true;
            }
            string retMsg = json.GetRecd<string>(BaseConst.RET_VAL);
            dto.retType = json.GetRecd<int>(BaseConst.RET_TYPE);
            switch (retMsg)
            {
                case BaseConst.RET_VAL_SUCC:
                    this.DoSuccess(dto);
                    break;
                case BaseConst.RET_VAL_FAIL:
                    this.DoFail(dto);
                    break;
                case BaseConst.RET_VAL_NATIVE:
                    this.DoNative(dto);
                    break;
                default:
                    break;
            }
            dto.ExecuteNetResponseCallback();
            dto.ExecuteUICallback();
            
            if (this.doProcessEnd != null)
            {
                this.client.DispatchToMainThread(this.doProcessEnd, dto);
            }
        }

        public virtual void DoSuccess(BaseDto dto)
        {
        }

        public virtual void DoFail(BaseDto dto)
        {
            DebugTool.LogWarning("[AbstractBaseAction.DoFail]\n" +
                             "Send: " + dto.resposeJSON.SendDataJosn + "\n" +
                             "Receive: " + dto.resposeJSON.RecdDataJosn);
        }

        public virtual void DoNative(BaseDto dto)
        {
            DebugTool.Log("[AbstractBaseAction.DoNative]\n" +
                             "Send: " + dto.resposeJSON.SendDataJosn + "\n" +
                             "Receive: " + dto.resposeJSON.RecdDataJosn);
            //dto.resposeJSON.AddRecv(BaseConst.RET_VAL, BaseConst.RET_VAL_NONE);
        }
    }
}