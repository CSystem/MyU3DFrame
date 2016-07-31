
using System;
using System.Collections.Generic;
using U3DFrame.Action;
using U3DFrame.Interface;
using U3DFrame.Manager;

namespace U3DFrame.Manager
{
    public class ActionMgr
    {
        private Dictionary<string, BaseAction> actionDict = new Dictionary<string, BaseAction>();
        private Dictionary<string, Type> actionTypeDict = new Dictionary<string, Type>();

        public ParamDtoDelegate doProcessEnd;

        private IYxClient _client;
        public IYxClient Client { get { return _client; } }
        public ActionMgr(IYxClient client)
        {
            this._client = client;
        }
        public EventManager eventManager
        {
            get
            {
                return null;// this._client.GetEventMgr();
            }
        }

        public BaseAction GetAction(string msg)
        {
            BaseAction ret = null;
            if (this.actionDict.ContainsKey(msg))
            {
                ret = this.actionDict[msg];
            }
            else
            {
                Type type = GetType(msg);
                if (type != null)
                {
                    ret = (BaseAction)Activator.CreateInstance(type);
                    ret.SetActionMgr(this);
                    ret.SetDoProcessEndDelegate(this.doProcessEnd);
                    this.actionDict[msg] = ret;
                }
            }
            return ret;
        }
        private Type GetType(string msg)
        {
            if (this.actionTypeDict.ContainsKey(msg))
            {
                return this.actionTypeDict[msg];
            }
            return null;
        }
        public void AddAction(string msg, Type type)
        {
            this.actionTypeDict[msg] = type;
        }
    }
}