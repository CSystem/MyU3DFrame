using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using U3DFrame.Tool;

namespace Com.Youxin.Common.Data
{
    public enum ResType
    {
        RES_DFT = 0,
        RES_XML,
        RES_IMG,
        RES_ASSETBUDDLE,
        RES_AUDIO,
        RES_ZIP,
    }
    public enum ELoadLevel
    {
        /// <summary>
        /// 下载并且加载到内存
        /// </summary>
        DOWN_AND_LOAD2MEM = 0,
        /// <summary>
        /// 只下载，不加载
        /// </summary>
        DOWN_ONLY,
        /// <summary>
        /// 下载并且检测是否有更新
        /// </summary>
        DOWN_AND_UPD_RES,
        /// <summary>
        /// 后台下载
        /// </summary>
        DOWN_DYNAMIC,
        /// <summary>
        /// 下载之后解压
        /// </summary>
        DOWN_AND_UNZIP
    }
    public enum DownState
    {
        DOWN_NONE,
        DOWN_NET,
        DOWN_NET_DONE,
        DOWN_LOCAL,
        DOWN_LOCAL_DONE
    }
    public class AssetBundleContainer
    {
        public DateTime lastUsedTime = DateTime.Now;
        public bool IsUnUsedLongTime
        {
            get
            {
                return this.assetbundle != null && DateTime.Now.Subtract(lastUsedTime).TotalMinutes >= 10;
            }
        }
        public AssetBundle assetbundle;

        public void GC()
        {
            if (this.assetbundle != null)
            {
                this.assetbundle.Unload(false);
            }
            this.assetbundle = null;
        }
    }
    /// <summary>
    /// 下载资源数据类
    /// </summary>
    public class DownLoadData
    {
        public DownState downState = DownState.DOWN_NONE;
        public bool instance = false;
        private object _obj;
        /// <summary>
        /// 资源名称
        /// </summary>
        public string name;
        /// <summary>
        /// 平台
        /// </summary>
        public string plat;
        /// <summary>
        /// 本地相对路径(相对SD卡的路径img/head.jpg)
        /// </summary>
        public string nativeRelativeURL;
        /// <summary>
        /// 网络相对路径(相对downServer的路径img/head.jpg)
        /// </summary>
        public string _netRelativeURL;
//         public string netRelativeURL
//         {
//             get
//             {
//                 if (CommonUtils.IsEmpty(_netRelativeURL))
//                 {
//                     _netRelativeURL = this.nativeRelativeURL;
//                 }
//                 return this._netRelativeURL;
//             }
//             set
//             {
//                 this._netRelativeURL = value;
//             }
//         }
//         public string netAbsoluteURL;
//         /// <summary>
//         /// 本地绝对路径(没有版本号)
//         /// </summary>
//         public string nativeAbsoluteURL
//         {
//             get { return YxPlat.rwPath + nativeRelativeURL; }
//         }
//         /// <summary>
//         /// file:///sdcard加载路径
//         /// </summary>
//         public string loadAbsoluteURL
//         {
//             get { return "file://" + this.GetNativePath(); }
//         }
//         // public object mainAsset;
//         private static Dictionary<string, AssetBundleContainer> assetBundleDict = new Dictionary<string, AssetBundleContainer>();
// 
//         public static void SetAssetBundleDict(string key, AssetBundle assetBundle)
//         {
//             if (assetBundleDict.ContainsKey(key))
//             {
//                 assetBundleDict[key].assetbundle = assetBundle;
//                 assetBundleDict[key].lastUsedTime = DateTime.Now;
//             }
//             else
//             {
//                 AssetBundleContainer assetBundleContainer = new AssetBundleContainer();
//                 assetBundleContainer.assetbundle = assetBundle;
//                 assetBundleContainer.lastUsedTime = DateTime.Now;
//                 assetBundleDict[key] = assetBundleContainer;
//             }
//         }
//         public static bool HasAssetBundle(string key)
//         {
//             return assetBundleDict.ContainsKey(key);
//         }
//         public static AssetBundle GetAssetBundle(string key)
//         {
//             if (HasAssetBundle(key))
//             {
//                 return assetBundleDict[key].assetbundle;
//             }
//             return null;
//         }
//         public static bool IsUnsedLongTime(string key)
//         {
//             if (HasAssetBundle(key))
//             {
//                 return assetBundleDict[key].IsUnUsedLongTime;
//             }
//             return true;
//         }
//         public static void GCAssetBundles()
//         {
//             List<string> keyList = new List<string>();
//             foreach(string key in assetBundleDict.Keys)
//             {
//                 AssetBundleContainer abc = assetBundleDict[key];
//                 if (abc == null)
//                 {
//                     continue;
//                 }
//                 if (abc.IsUnUsedLongTime)
//                 {
//                     abc.GC();
//                     keyList.Add(key);
//                 }
//             }
//             for (int i = 0; i < keyList.Count; i++)
//             {
//                 assetBundleDict.Remove(keyList[i]);
//             }
//         }
//         public GameObject newObjectInstance
//         {
//             get
//             {
//                 if (this.obj != null)
//                 {
//                     if (this.resType == ResType.RES_ASSETBUDDLE)
//                     {
//                         return GameObject.Instantiate(this.obj as GameObject);
//                     }
//                 }
//                 return null;
//             }
//         }
//         public object obj
//         {
//             get
//             {
//                 if (this._obj != null)
//                 {
//                     return this._obj;
//                 }
//                 else if (this.www != null && this.isDownLoaded())
//                 {
//                     switch (this.resType)
//                     {
//                         case ResType.RES_XML:
//                             this._obj = this.www.text;
//                             UnityTools.SetResouces(this.nativeRelativeURL, this._obj);
//                             break;
//                         case ResType.RES_IMG:
//                             this._obj = this.www.texture;
//                             UnityTools.SetResouces(this.nativeRelativeURL, this._obj);
//                             break;
//                         case ResType.RES_ASSETBUDDLE:
//                             AssetBundle assetBundle = null;
//                             AssetBundleContainer assetBundleContainer = null;
//                             if (assetBundleDict.ContainsKey(this.nativeRelativeURL))
//                             {
//                                 assetBundleContainer = assetBundleDict[this.nativeRelativeURL];
//                             }
//                             if (assetBundleContainer == null)
//                             {
//                                 assetBundleContainer = new AssetBundleContainer();
//                                 assetBundleDict[this.nativeRelativeURL] = assetBundleContainer;
//                             }
//                             if (assetBundleContainer.assetbundle != null)
//                             {
//                                 assetBundle = assetBundleDict[this.nativeRelativeURL].assetbundle;
//                             }
//                             else
//                             {
//                                 DebugTool.LogError("[DownLoadData.obj] www.url: " + www.url);
//                                 assetBundle = this.www.assetBundle;
//                                 assetBundleDict[this.nativeRelativeURL].assetbundle = assetBundle;
//                             }
//                             assetBundleDict[this.nativeRelativeURL].lastUsedTime = DateTime.Now;
//                             GameObject go = assetBundle.mainAsset as GameObject;
//                             UnityTools.SetResouces(this.nativeRelativeURL, go);
//                             if (instance)
//                             {
//                                 // this._obj = GameObject.Instantiate(go);
//                                 return GameObject.Instantiate(go);
//                             }
//                             else
//                             {
//                                 // this._obj = go;
//                                 return go;
//                             }
//                         // break;
//                         case ResType.RES_AUDIO:
//                             this._obj = this.www.audioClip;
//                             UnityTools.SetResouces(this.nativeRelativeURL, this._obj);
//                             break;
//                         case ResType.RES_ZIP:
//                             // CommonUtils.UnzipFile(this.nativeAbsoluteURL);
//                             break;
//                     }
//                 }
//                 return this._obj;
//             }
//             set
//             {
//                 this._obj = value;
//             }
//         }
// 
//         public ResType resType;
// 
//         /// <summary>
//         /// 资源下载回调函数
//         /// </summary>
//         // private DownLoadCallBack _callback;
//         private List<DownLoadCallBack> callbackList = new List<DownLoadCallBack>();
//         public DownLoadCallBack callback
//         {
//             set
//             {
//                 // this._callback = value;
//                 this.callbackList.Add(value);
//             }
//         }
//         // private ResCallback _resCallback;
//         private List<ResCallback> resCallbackList = new List<ResCallback>();
//         public ResCallback resCallback
//         {
//             set
//             {
//                 // this._resCallback = value;
//                 this.resCallbackList.Add(value);
//             }
//         }
//         public void Call()
//         {
//             this.IsDone = true;
//             for (int i = 0; i < this.callbackList.Count; i++)
//             {
//                 DownLoadCallBack call = this.callbackList[i];
//                 if (call == null)
//                 {
//                     continue;
//                 }
//                 this._param = this.paramList[i];
//                 call(this);
//             }
//             for (int i = 0; i < this.resCallbackList.Count; i++)
//             {
//                 ResCallback call = this.resCallbackList[i];
//                 if (call == null)
//                 {
//                     continue;
//                 }
//                 call(this.GetNativePath(), "", this.obj, this.paramList[i]);
//             }
//             this.callbackList.Clear();
//             this.resCallbackList.Clear();
//             this.paramList.Clear();
//         }
//         // public ResCallback resCallFunc;
//         /// <summary>
//         /// 参数
//         /// </summary>
//         private List<object> paramList = new List<object>();
//         private object _param;
//         public object param
//         {
//             get
//             {
//                 return this._param;
//             }
//             set
//             {
//                 this._param = value;
//                 this.paramList.Add(value);
//             }
//         }
//         //public void AddParam(object paramObject)
//         //{
//         //    this.paramList.Add(paramObject);
//         //}
//         private WWW _www;
//         /// <summary>
//         /// www下载对象
//         /// </summary>
//         public WWW www
//         {
//             get { return this._www; }
//             set
//             {
//                 this._www = value;
//                 this.obj = null;
//             }
//         }
// 
//         /// <summary>
//         /// HTTP ERROR【用于GPRS访问错误】
//         /// </summary>
//         public string error;
// 
//         /// <summary>
//         /// 版本号
//         /// </summary>
//         public int ver = 0;
//         private float downLoadProcess;
//         public float DownLoadProcess
//         {
//             get
//             {
//                 if (HTTPClientManager.netSettings == NetSettings.GPRS)
//                 {
//                     return this.downLoadProcess;
//                 }
//                 if (www != null)
//                 {
//                     return this.www.progress;
//                 }
//                 return 0;
//             }
//             set
//             {
//                 this.downLoadProcess = value;
//             }
//         }
//         /// <summary>
//         /// 加载次数
//         /// </summary>
//         public int loadCnt = 3;
//         public ELoadLevel loadLvl = ELoadLevel.DOWN_AND_LOAD2MEM;
//         public bool isStreamingAssets = false;
//         public bool onlyDownNotLoad2Mem
//         {
//             get { return this.loadLvl == ELoadLevel.DOWN_ONLY; }
//         }
// 
//         public DownLoadData()
//         {
//             this.Init(ResType.RES_DFT);
//         }
//         public DownLoadData(ResType type)
//         {
//             this.Init(type);
//         }
//         public DownLoadData(string path, int ver)
//             : this(path, ver, ResType.RES_DFT)
//         {
//         }
//         public DownLoadData(string path, int ver, ResType type)
//             : this(path, ver, type, null, null)
//         {
//         }
//         public DownLoadData(string path, int ver, ResType type, DownLoadCallBack callback, object param)
//             : this(path, ver, path, type, callback, param)
//         {
//         }
//         public DownLoadData(string path, int ver, ResType type, DownLoadCallBack callback, ResCallback resCallback, object param)
//             : this(path, ver, path, type, callback, param)
//         {
//             this.Init(type);
//             this.nativeRelativeURL = path;
//             this.netRelativeURL = path;
//             this.ver = ver;
//             this.callback = callback;
//             this.resCallback = resCallback;
//             this.param = param;
//         }
//         public DownLoadData(string path, int ver, string netPath, ResType type, DownLoadCallBack callback, object param)
//         {
//             this.Init(type);
//             this.nativeRelativeURL = path;
//             this.netRelativeURL = netPath;
//             this.ver = ver;
//             this.callback = callback;
//             this.param = param;
//         }
//         private void Init(ResType type)
//         {
//             // this.serverURL = this.downServer;
//             this.loadCnt = 3;
//             this.resType = type;
//         }
//         public string downServer
//         {
//             get { return YxPlat.yxClient.GetYxCfgData().downServer; }
//         }
//         /// <summary>
//         /// 根据相对路径获得本地绝对路径
//         /// </summary>
//         /// <param name="relativePath"></param>
//         /// <returns></returns>
//         private string ParseNativePath(string relativePath, int ver)
//         {
//             if (CommonUtils.IsEmpty(relativePath))
//             {
//                 return null;
//             }
//             string path = relativePath;
//             if (this.isStreamingAssets)
//             {
//                 path = Application.streamingAssetsPath + "/" + path;
//                 if (ver != 0)
//                 {
//                     path = path + "_" + ver;
//                 }
//             }
//             else
//             {
//                 if (ver != 0)
//                 {
//                     path = path + "_" + ver;
//                 }
//                 path = YxPlat.rwPath + path;
//             }
//             return path;
//         }
//         public string GetNativePath()
//         {
//             return ParseNativePath(this.nativeRelativeURL, this.ver);
//         }
//         public bool useNetAbsolute = false;
//         public string GetNetPath()
//         {
//             if (this.isAsset)
//             {
//                 return this.downServer + "res/" + YxPlat.assetPrefix + "/" + this.netRelativeURL;
//             }
//             if (this.useNetAbsolute)
//             {
//                 return this.netAbsoluteURL;
//             }
//             return this.downServer + this.netRelativeURL;
//         }
// 
//         private string _wwwText;
//         public string wwwText
//         {
//             get
//             {
//                 if (_wwwText == null)
//                 {
//                     if (www == null)
//                     {
//                         _wwwText = "";
//                     }
//                     else
//                     {
//                         _wwwText = www.text;
//                     }
//                 }
//                 return _wwwText;
//             }
//         }
// 
//         private string _wwwError;
//         public string wwwError
//         {
//             get
//             {
//                 if (_wwwError == null)
//                 {
//                     _wwwError = www.error;
//                 }
//                 return _wwwError;
//             }
//         }
//         public bool isAsset
//         {
//             get
//             {
//                 return this.resType == ResType.RES_ASSETBUDDLE;
//             }
//         }
//         public void ReleaseAsset(bool dispose = true)
//         {
//             UnityTools.yxClient.StartCoroutine(this.StartGC(dispose));
//         }
//         private IEnumerator StartGC(bool dispose = true)
//         {
//             yield return new WaitForSeconds(3);
//             if (this.www != null)
//             {
//                 //if (this.isDownLoaded() && this.www.assetBundle != null)
//                 //{
//                 //    this.www.assetBundle.Unload(false);
//                 //    // this.mainAsset = null;
//                 //}
//                 if (dispose)
//                 {
//                     this.www.Dispose();
//                 }
//                 this.www = null;
//             }
//         }
//         public void GCALL()
//         {
//             UnityTools.yxClient.StartCoroutine(this.StartGC());
//             if (this._obj != null)
//             {
//                 UnityTools.Destroy(this._obj);
//             }
//             this.obj = null;
//         }
//         private bool _isDone;
//         public bool IsDone
//         {
//             get
//             {
//                 return this._isDone;
//             }
//             set
//             {
//                 this._isDone = value;
//             }
//         }
//         public bool isDownLoaded()
//         {
//             if (this.www == null && IsDone)
//             {
//                 return true;
//             }
//             if (!CommonUtils.IsEmpty(this.error))
//             {
//                 return false;
//             }
//             else if (this.www != null && !CommonUtils.IsEmpty(this.www.error))
//             {
//                 this.error = this.www.error;
//                 return false;
//             }
//             else if (this.resType != ResType.RES_ASSETBUDDLE && this.resType != ResType.RES_IMG && CommonUtils.IsEmpty(this.wwwText))
//             {
//                 this.error = "text is empty";
//                 return false;
//             }
//             else if (this.resType == ResType.RES_ASSETBUDDLE || this.resType == ResType.RES_IMG)
//             {
//                 if (this.www != null && !this.www.isDone)
//                 {
//                     this.error = "asset is empty";
//                     return false;
//                 }
//                 else
//                 {
//                     return true;
//                 }
//             }
//             JSONObject json = JSONObject.Parse(this.wwwText);
//             if (json == null || json.GetRecd<string>(BaseConst.RET_VAL) != BaseConst.ERR_404)
//             {
//                 return true;
//             }
//             return false;
//         }
    }
}