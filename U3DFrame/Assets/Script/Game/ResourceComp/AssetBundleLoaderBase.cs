using UnityEngine;
using System.Collections;
using U3DFrame.Tool;
using System.Collections.Generic;
using System.IO;
//一个bundle所包含的资源里的其中一个
public class ResourceRef
{
    protected string m_strPath = null;
    protected UnityEngine.Object m_iObject = null;
    protected int m_nError = 0;
    protected int m_nRef = 0;
    protected System.EventHandler<ResEventArgs> EventReady;

    public class ResEventArgs : System.EventArgs
    {
        public bool Success;
        public ResEventArgs(bool _Success)
        {
            Success = _Success;
        }
    }

    public UnityEngine.Object resource
    {
        get { return m_iObject; }
    }

    public int error
    {
        get { return m_nError; }
    }

    public string path
    {
        get { return m_strPath; }
    }

    public void Release()
    {
        m_nRef--;
    }

    public void SetEventListener(System.EventHandler<ResEventArgs> evtListener)
    {
        EventReady += evtListener;
        if (m_iObject != null)
        {
            RaiseReadyEvent(true);
        }
        if (m_nError != 0)
        {
            RaiseReadyEvent(false);
        }
    }

    protected ResourceRef()
    {

    }

    protected void RaiseReadyEvent(bool _Success)
    {
        if (EventReady != null)
        {
            EventReady(this, new ResEventArgs(_Success));
        }
        else
        {
            DebugTool.Log("Asset ready but no event listener: ", m_strPath);
        }
    }

    public static System.Predicate<ResourceRef> MatchPath(string path)
    {
        return delegate (ResourceRef r) { return r.path == path; };
    }
}

public class AssetBundleLoaderBase : MonoBehaviour
{
    public delegate void OnLoadingPackage(string strPackageName);
    public OnLoadingPackage onLoadingPackage = null;
    protected string m_strSourcePath = Application.streamingAssetsPath + "/";
    public string SourcePath
    {
        get { return m_strSourcePath; }
        set { m_strSourcePath = value; }
    }

    //下载包错误
    public const int EDownloadError = 1;
    public const int EPackageError = 2;
    public const int EObjectError = 3;
    public const int EFileError = 4;
    public const int ETmpFileError = 5;
    const int MAX_LOADER_COUNT = 10;
    protected int m_nCurrentMaxLoaderCount = MAX_LOADER_COUNT;
    const int MAX_CONOURINE_COUNT = 20;         //当前最多同时加载的协程数量
    protected int m_nCurrentMaxCoroutineCount = MAX_CONOURINE_COUNT;

    protected int m_nCoroutineCount = 0;
    public class SceneEventArgs : System.EventArgs
    {
        public bool Success;
        public SceneEventArgs(bool _Success)
        {
            Success = _Success;
        }
    }

    public class InitPackageArgs : System.EventArgs
    {
        public bool Success;
        public InitPackageArgs(bool _Success)
        {
            Success = _Success;
        }
    }

    protected enum TRequestType //请求协程加载资源包是的原因，可能是为了加载一个资源，可能是为了加载一个场景
    {
        EResource,
        EScene,
        EInitPackage,
    }

    protected class RequestArg
    {
        public TRequestType eType;
        public RequestArg(TRequestType _type)
        {
            eType = _type;
        }
    }

    protected class RequestArg_Resource : RequestArg
    {
        public string path;
        public InternalResourceRef resource_ref;
        public bool bAllowFail;
        public RequestArg_Resource(string _path, InternalResourceRef _resource_ref, bool _bAllowFail)
            : base(TRequestType.EResource)
        {
            path = _path;
            resource_ref = _resource_ref;
            bAllowFail = _bAllowFail;
        }
    }

    protected class RequestArg_Scene : RequestArg
    {
        public string scene_name;
        public System.EventHandler<SceneEventArgs> evtSceneListener;
        public bool bAdditive;
        public RequestArg_Scene(string _scene_name, System.EventHandler<SceneEventArgs> _evtSceneListener, bool _bAdditive) : base(TRequestType.EScene)
        {
            scene_name = _scene_name;
            evtSceneListener = _evtSceneListener;
            bAdditive = _bAdditive;
        }
    }

    protected class RequestArg_InitPackage : RequestArg
    {
        public System.EventHandler<InitPackageArgs> evtListener;
        public RequestArg_InitPackage(System.EventHandler<InitPackageArgs> Listener)
            : base(TRequestType.EInitPackage)
        {
            evtListener = Listener;
        }
    }


    /// <summary>
    /// bundle引用，一个package里包含了一个或多个资源，这个是总的
    /// </summary>
    protected class PackageRef
    {
        public AssetBundle bundle = null;
        public string name = null;
        public int use = 0; //有几个待加载资源在使用，加载后就可以释放了
        public float createtime;
        public string[] dep_package_name;
        public int error = 0;//这个包当前的错误

        public PackageRef(string _name)
        {
            name = _name;
            use = 0;
            createtime = Time.time;
        }
        public static System.Predicate<PackageRef> MatchPath(string _name)
        {
            return delegate (PackageRef r) { return r.name == _name; };
        }
    }

    protected class WWWPakcageLoader
    {
        private static int m_iRandVer = -1;

        WWW m_iWWW = null;
        PackageRef m_iPackageRef = null;

        public WWW www
        {
            get { return m_iWWW; }
            set { m_iWWW = value; }
        }

        public AssetBundleLoaderBase.PackageRef Pack
        {
            get { return m_iPackageRef; }
        }

        public WWWPakcageLoader(AssetBundleLoaderBase.PackageRef packageRef, string strUrl)
        {
            if (m_iRandVer == -1)
            {
                m_iRandVer = UnityEngine.Random.Range(1, 100000);
            }

            if (packageRef == null || strUrl == "")
                return;

            m_iPackageRef = packageRef;
            //使用缓存加载，会节省加载过程中的内存，但是会增大sd卡容量
            //m_iWWW = WWW.LoadFromCacheOrDownload(strUrl, 0);
            m_iWWW = new WWW(strUrl);
        }
    }
    //对资源引用的一个封装，添加公共接口
    protected class InternalResourceRef : ResourceRef
    {
        public InternalResourceRef(string resourcePath)
        {
            m_strPath = resourcePath;
        }

        public void SetObject(UnityEngine.Object iObject)
        {
            m_iObject = iObject;
        }

        public void SetError(int nError)
        {
            m_nError = nError;
        }

        public void SendNotify()
        {
            if (m_iObject != null)
            {
                RaiseReadyEvent(true);
            }
            else if (m_nError != 0)
            {
                RaiseReadyEvent(false);
            }
            else
            {
                DebugTool.LogError("Load object with Unknown Error: ", m_strPath);
                RaiseReadyEvent(false);
            }
        }

        public void AddRef()
        {
            m_nRef++;
        }

        public void DelRef()
        {
            m_nRef--;
            if (m_nRef < 0)
            {
                m_nRef = 0;
            }
        }

        public int RefCount
        {
            get { return m_nRef; }
        }

        public static System.Predicate<InternalResourceRef> MatchPath(string path)
        {
            return delegate (InternalResourceRef r) { return r.path == path; };
        }
    }

    InternalResourceRef GetExistResourceRef(string path)
    {
        InternalResourceRef _res = _ltPreResource.Find(InternalResourceRef.MatchPath(path));
        if (_res != null)
            return _res;

        return _ltResource.Find(InternalResourceRef.MatchPath(path));
    }

    PackageRef GetExistPackageRef(string name)
    {
        return _ltPackageRef.Find(PackageRef.MatchPath(name));
    }

    private static AssetBundleManifest assetBundleManifest = null;
    private Dictionary<string, string> _packNameToBundleName = new Dictionary<string, string>();
    private Dictionary<string, string> _bundleNameToPackName = new Dictionary<string, string>();

    private static AssetBundleLoaderBase m_iInstance = null;
    public static AssetBundleLoaderBase Instance
    {
        get
        {
            if (m_iInstance == null)
            {
                GameObject iLoaderObj = new GameObject("Loader");
                m_iInstance = iLoaderObj.AddComponent<AssetBundleLoaderBase>();
                if (null != m_iInstance)
                    m_iInstance.StartCoroutine(m_iInstance.InitManifest());
            }
            return m_iInstance;
        }
    }

    private static AssetBundleLoaderBase m_iUIInstance = null;
    public static AssetBundleLoaderBase UIInstance
    {
        get
        {
            if (m_iUIInstance == null)
            {
                GameObject iLoaderObj = new GameObject("UILoader");
                m_iUIInstance = iLoaderObj.AddComponent<AssetBundleLoaderBase>();
            }
            return m_iUIInstance;
        }
    }


    protected List<PackageRef> _ltPackageRef = new List<PackageRef>();
    protected List<InternalResourceRef> _ltResource = new List<InternalResourceRef>();
    protected List<InternalResourceRef> _ltPreResource = new List<InternalResourceRef>();
    public ResourceRef LoadResource(string path, System.EventHandler<ResourceRef.ResEventArgs> evtListener = null)
    {
        return DoLoadResource(path, evtListener, false);
    }

    private ResourceRef DoLoadResource(string resourcePath, System.EventHandler<ResourceRef.ResEventArgs> evtListener, bool bAllowFail)
    {
        InternalResourceRef resRef = GetExistResourceRef(resourcePath);
        if (null != resRef)
        {
            resRef.AddRef();
            if (null != evtListener)
            {
                resRef.SetEventListener(evtListener);
            }
            return resRef;
        }

        resRef = new InternalResourceRef(resourcePath);
        resRef.AddRef();
        AddResourceRef(resRef);

        if (evtListener != null)
        {
            resRef.SetEventListener(evtListener);
        }

        string package_name = UnityTools.GetPackageName(resourcePath);

        StartCoroutine(LoadResourceCoroutineMulti(package_name, new RequestArg_Resource(resourcePath, resRef, bAllowFail)));
        return resRef;
    }
    //协程加载资源包，同时存在多个协程
    IEnumerator LoadResourceCoroutineMulti(string rootPackage, RequestArg request_arg)
    {
        while (null == assetBundleManifest)
            yield return new WaitForSeconds(0.1f);

        do
        {
            //如果协程数量超过最大数量限制，在这里等待直到协程数量减少后开始继续执行
            if (m_nCoroutineCount < m_nCurrentMaxCoroutineCount)
                break;
            yield return new WaitForSeconds(0.1f);
        } while (true);

        if (request_arg.eType == TRequestType.EResource)
        {
            ((RequestArg_Resource)request_arg).resource_ref.AddRef();
        }
        //协程计数加1
        m_nCoroutineCount++;

        List<PackageRef> ltPackageRefed = new List<PackageRef>();   //当前协成中所有引用到的包，为了防止垃圾回收，要在这个协成执行过程中增加包的引用技术

        List<WWWPakcageLoader> ltLoader = new List<WWWPakcageLoader>();
        List<WWWPakcageLoader> ltLoaderFinish = new List<WWWPakcageLoader>();

        List<string> ltPackageToLoad = new List<string>();
        ltPackageToLoad.Add(rootPackage);
        int nPackageIndex = 0;

        while (ltLoader.Count > 0 || ltPackageToLoad.Count > 0)
        {
            //看看正在加载的资源有没有加载完
            for (int i = 0; i < ltLoader.Count; i++)
            {
                if (ltLoader[i].www.isDone)
                {
                    ltLoader[i].Pack.bundle = ltLoader[i].www.assetBundle;

                    if (ltLoader[i].Pack.bundle == null)
                    {
                        ltLoader[i].Pack.error = 2;
                        OnRequestPackageFail(request_arg, ltLoader[i].Pack.error);
                        DebugTool.LogError("load error, bundle is null with name: ", ltLoader[i].Pack.name, " strConDebugName: ");

                    }
                    ltLoaderFinish.Add(ltLoader[i]);
                    if (ltLoader[i].www.error != null)
                    {
                        //包加载错误
                        ltLoader[i].Pack.error = 1;
                        OnRequestPackageFail(request_arg, ltLoader[i].Pack.error);
                        DebugTool.LogError("load package error: [", ltLoader[i].Pack.name, "]");
                    }
                    ltLoader[i].www.Dispose();
                    ltLoader[i].www = null;
                }
            }

            //从加载列表中移除已经加载完毕的资源包
            for (int j = 0; j < ltLoaderFinish.Count; ++j)
            {
                ltLoader.Remove(ltLoaderFinish[j]);
            }
            ltLoaderFinish.Clear();


            if (ltPackageToLoad.Count > 0 && ltLoader.Count < m_nCurrentMaxLoaderCount)
            {
                string load_package_name = ltPackageToLoad[0];
                ltPackageToLoad.RemoveAt(0);

                if (onLoadingPackage != null)
                    onLoadingPackage("loading: " + load_package_name);
                PackageRef load_package = GetExistPackageRef(load_package_name);
                if (load_package == null)
                {
                    //此包在所有的加载协程里未找到
                    load_package = new PackageRef(load_package_name);
                    AddPackageRef(load_package);

                    ltPackageRefed.Add(load_package);
                    load_package.use++;

                    WWWPakcageLoader loader = null;
                    string url = "";

                    url = SourcePath + GetBundleNameInManifest(load_package_name);

                    if (!url.Contains("://"))
                    {
                        url = "file://" + url;
                    }
                    loader = new WWWPakcageLoader(load_package, url);

                    ltLoader.Add(loader);
                }
                else if (load_package.error != 0)
                {
                    ltPackageRefed.Add(load_package);
                    load_package.use++;

                    DebugTool.LogError("other load package error: [", load_package.name, "]");
                    OnRequestPackageFail(request_arg, load_package.error);
                }
                else if (load_package.bundle == null)
                {
                    //此包其它协程正在加载,添加到链表里,在while里做检测
                    //                     ltOtherPakcageToLoad.Add(load_package);
                    // 
                    //                     ltPackageRefed.Add(load_package);
                    //                     load_package.use++;


                }
                else
                {
                    ltPackageRefed.Add(load_package);
                    load_package.use++;
                }

                nPackageIndex++;
                if (nPackageIndex == 1)
                {
                    UFileInfo fileInfo = UFile.Instance.GetFileInfo(load_package_name);
                    if (null == fileInfo)
                    {
                        Debug.LogError("cant find fileInfo : " + load_package_name);
                    }
                    else
                    {
                        string[] deps = fileInfo.dependencies;
                        for (int i = 0; i < deps.Length; i++)
                        {
                            ltPackageToLoad.Add(deps[i]);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        if (onLoadingPackage != null)
        {
            if (request_arg.eType == TRequestType.EResource)
                onLoadingPackage("loading model...");
            else if (request_arg.eType == TRequestType.EScene)
                onLoadingPackage("loading scene...");
            else
                onLoadingPackage("loading init package...");
        }

        switch (request_arg.eType)
        {
            case TRequestType.EResource:
                {
                    RequestArg_Resource arg_resource = (RequestArg_Resource)request_arg;

                    PackageRef package = GetExistPackageRef(rootPackage);
                    if (package != null && package.bundle != null)
                    {
                        AssetBundleRequest objRequest = null;

                        objRequest = package.bundle.LoadAssetAsync(UnityTools.GetAssetnameInPackage(arg_resource.path).Trim().ToLower(), typeof(UnityEngine.Object));

                        yield return objRequest;

                        UnityEngine.Object resObject = null;

                        try
                        {
                            resObject = objRequest.asset;
                        }
                        catch (System.Exception e)
                        {
                            DebugTool.LogError("Get asset Error: ", arg_resource.path, " error: ", e.Message);
                        }

                        if (resObject != null)
                        {
                            arg_resource.resource_ref.SetObject(resObject);
                        }
                        else
                        {
                            arg_resource.resource_ref.SetError(EObjectError);
                        }

                    }
                    else
                    {
                        arg_resource.resource_ref.SetError(EObjectError);
                    }
                }
                break;
            case TRequestType.EScene:
                //LoadSceneInPackage((RequestArg_Scene)request_arg);
                break;
            case TRequestType.EInitPackage:
                {
                    RequestArg_InitPackage arg_initpackage = (RequestArg_InitPackage)request_arg;
                    if (arg_initpackage != null && arg_initpackage.evtListener != null)
                    {
                        arg_initpackage.evtListener(GetExistPackageRef(rootPackage), new InitPackageArgs(true));
                    }
                }
                break;
        }

        if (request_arg.eType != TRequestType.EInitPackage)
        {
            //ReleasePackageUseCnt(ltPackageRefed, strConDebugName);
        }

        if (request_arg.eType == TRequestType.EResource)
        {
            //在减引用计数之后在调用回调，这样在回调里就可以释放资源包了
            ((RequestArg_Resource)request_arg).resource_ref.SendNotify();
            ((RequestArg_Resource)request_arg).resource_ref.DelRef();
        }
        m_nCoroutineCount--;
    }

    private void AddResourceRef(InternalResourceRef resRef)
    {
        _ltResource.Add(resRef);
    }

    void AddPackageRef(PackageRef packRef)
    {
        _ltPackageRef.Add(packRef);
    }

    private void OnRequestPackageFail(RequestArg request_arg, int nError)
    {
        switch (request_arg.eType)
        {
            case TRequestType.EResource:
                {
                    RequestArg_Resource arg = (RequestArg_Resource)request_arg;
                    arg.resource_ref.SetError(nError);
                }
                break;
            case TRequestType.EScene:
                {
                    RequestArg_Scene arg = (RequestArg_Scene)request_arg;
                    if (arg.evtSceneListener != null)
                    {
                        arg.evtSceneListener(this, new SceneEventArgs(false));
                    }
                }
                break;
            case TRequestType.EInitPackage:
                {
                    RequestArg_InitPackage arg = (RequestArg_InitPackage)request_arg;
                    if (arg.evtListener != null)
                    {
                        arg.evtListener(this, new InitPackageArgs(false));
                    }
                }
                break;
        }
    }

    private void ReadPackageInfo(PackageRef pack, byte[] data)
    {
        pack.dep_package_name = new string[] { };

        MemoryStream stream = new MemoryStream(data);
        StreamReader reader = new StreamReader(stream);

        do
        {
            string dep_package_name = reader.ReadLine();
            if (dep_package_name == null || dep_package_name.Length == 0)
                break;

            System.Array.Resize<string>(ref pack.dep_package_name, pack.dep_package_name.Length + 1);
            pack.dep_package_name[pack.dep_package_name.Length - 1] = dep_package_name;
        } while (true);

        reader.Close();
        stream.Close();
    }

    IEnumerator InitManifest()
    {
        string url = "file://" + GameConst.ResRootPath + "AssetInfo";
        if (null == assetBundleManifest)
        {
            WWW www = WWW.LoadFromCacheOrDownload(url, 0);
            yield return www;
            if (null != www.assetBundle)
            {
                assetBundleManifest = www.assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            }
        }
        if (null == assetBundleManifest)
        {
            DebugTool.LogError("cant find the file : " + url);
            yield break;
        }

        string[] bundlePaths = assetBundleManifest.GetAllAssetBundles();
        for (int i = 0; i < bundlePaths.Length; i++)
        {
            string name = bundlePaths[i];
            name = Path.GetFileNameWithoutExtension(name);
            Debug.Log(name + "=>" + bundlePaths[i]);
            _packNameToBundleName[name] = bundlePaths[i];
            _bundleNameToPackName[bundlePaths[i]] = name;
        }

    }

    private string GetBundleNameInManifest(string resouceName)
    {
        if (_packNameToBundleName.ContainsKey(resouceName))
            return _packNameToBundleName[resouceName];
        return null;
    }

    private string GetPackNameByBundleName(string bundleName)
    {
        if (_bundleNameToPackName.ContainsKey(bundleName))
            return _bundleNameToPackName[bundleName];
        return null;
    }
}
