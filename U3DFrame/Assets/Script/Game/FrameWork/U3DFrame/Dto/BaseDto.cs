using U3DFrame.Const;
using U3DFrame.Interface;
using U3DFrame.Json;
using U3DFrame.Tool;

public delegate void ParamDtoDelegate(BaseDto dto);
public class BaseDto : IDto
{
    public int userId;
    public int tryTotalCnt = 1;
    public static long globalEventId = 10000;
    private long _evtId;
    public long requestId
    {
        get
        {
            if (this._evtId == 0)
            {
                this._evtId = globalEventId++;
            }
            return this._evtId;
        }
        set
        {
            this._evtId = value;
        }
    }
    private IYxClient _clent;
    public IYxClient client
    {
        get { return this._clent; }
        set { this._clent = value; }
    }
    public string msg;
    public string cmd;
    private ParamDtoDelegate _netResponseCallback;
    public ParamDtoDelegate netResponseCallback
    {
        get { return this._netResponseCallback; }
        set
        {
            this._netResponseCallback = value;
        }
    }
    private bool _isSucc;
    public bool IsSuccess
    {
        get
        {
            if (this.resposeJSON != null)
            {
                _isSucc = this.resposeJSON.GetRecd<string>(BaseConst.RET_VAL) == BaseConst.RET_VAL_SUCC;
            }
            return _isSucc;
        }
        set
        {
            _isSucc = value;
        }
    }
    private bool _isFail = false;
    public bool IsFail
    {
        get
        {
            if (this.resposeJSON != null)
            {
                _isFail = this.resposeJSON.GetRecd<string>(BaseConst.RET_VAL) == BaseConst.RET_VAL_FAIL;
            }
            return _isFail;
        }
        set
        {
            _isFail = value;
        }
    }
    private bool _isNative;
    public bool IsNative
    {
        get
        {
            if (this.resposeJSON != null)
            {
                _isNative = this.resposeJSON.GetRecd<string>(BaseConst.RET_VAL) == BaseConst.RET_VAL_NATIVE;
            }
            return _isNative;
        }
        set
        {
            _isNative = value;
        }
    }
    private string _exeEvt;
    /// <summary>
    /// 执行事件
    /// </summary>
    public string exeEvt
    {
        get { return this._exeEvt; }
        set
        {
            this._exeEvt = value;
        }
    }
    private ParamDtoDelegate _uiCallback;
    public ParamDtoDelegate uiCallback
    {
        get { return this._uiCallback; }
        set { this._uiCallback = value; }
    }
    public object uiCallbackParam;
    public object[] param;
    public JSONObject resposeJSON;
    public int retType;
    public virtual JSONObject GetSendJSONObject()
    {
        return CommonTool.GenJSONObject(this.msg, this.cmd);
    }
    public T ParseDto<T>() where T : BaseDto
    {
        return (T)this;
    }

    public T GetSend<T>(string key)
    {
        return this.resposeJSON.GetSend<T>(key);
    }
    public T GetRecd<T>(string key)
    {
        return this.GetRecd<T>(key, default(T));
    }
    public T GetRecd<T>(string key, T dftVal)
    {
        return this.resposeJSON.GetRecd<T>(key, dftVal);
    }
    public bool RecHasKey(string key)
    {
        return this.resposeJSON.RecdHasKey(key);
    }

    public void ExecuteNetResponseCallback()
    {
        if (this._netResponseCallback != null)
        {
            this.client.DispatchToMainThread(this._netResponseCallback, this);
            this._netResponseCallback = null;
        }
    }
    public void ExecuteUICallback()
    {
        if (this._uiCallback != null)
        {
            this.client.DispatchToMainThread(this._uiCallback, this);
        }
    }
}
