using UnityEngine;
using System.Collections;
using U3DFrame.Tool;
using System.IO;
using System;
using System.Collections.Generic;
//  [6/22/2016 WangPengfei]

public class ResInfoRef : MonoBehaviour
{
    public delegate void OnResourceReady(string path, UnityEngine.Object res, System.Object callbackObj);

    protected class SRes
    {
        public string strPath = "";
        public bool bBundle = false;
        // 下面的两个指针只能有一个
        public UnityEngine.Object iResourceObject = null;
        public ResourceRef BundleRes = null;

        public SRes(string _path, UnityEngine.Object _iResourceObject)//针对resource中资源的构造
        {
            strPath = _path;
            bBundle = false;
            iResourceObject = _iResourceObject;
        }

        public SRes(string _path, ResourceRef bundleRes)//针对外部资源包的构造
        {
            strPath = _path;
            bBundle = true;
            BundleRes = bundleRes;
        }

        public static Predicate<SRes> MatchPath(string path)
        {
            return delegate (SRes r) { return r.strPath == path; };
        }
    }
    //追加的请求，当资源加载完毕统一回调这个请求，避免重复加载
    protected class SPendingRequest
    {
        public string path = "";
        public OnResourceReady callback = null;
        public System.Object callbackObj = null;

        public static Predicate<SPendingRequest> MatchPath(string path)
        {
            return delegate (SPendingRequest r) { return r.path == path; };
        }
    }

    protected List<SRes> m_lResKeyList = new List<SRes>();
    protected List<SPendingRequest> m_ltPendingRequest = new List<SPendingRequest>();

    private AssetBundleLoaderBase m_iLoader;
    public AssetBundleLoaderBase iLoader
    {
        set { m_iLoader = value; }
        get
        {
            if (m_iLoader == null)
                return AssetBundleLoaderBase.Instance;

            return m_iLoader;
        }
    }

    public void RegistResource(string path, OnResourceReady cb, System.Object callbackObj)
    {
        if (UnityTools.IsPackageResource(path))
        {
            SRes iRes = m_lResKeyList.Find(SRes.MatchPath(path));
            if (iRes != null && iRes.BundleRes.resource != null)
            {
                cb(path, iRes.BundleRes.resource, callbackObj);
            }
            else
            {
                SPendingRequest request = new SPendingRequest();
                request.path = path;
                request.callback = cb;
                request.callbackObj = callbackObj;
                m_ltPendingRequest.Add(request);

                if(null == iRes)
                {
                    iRes = new SRes(path, iLoader.LoadResource(path));
                    m_lResKeyList.Add(iRes);
                    iRes.BundleRes.SetEventListener(HandlePackageResourceEvent);
                }
                //StartCoroutine(TestLoad(path, cb, callbackObj));
            }
            
        }
        else
        {
            UnityEngine.Object resObj = RegistResource(path);
            //TODO 后续这里加一些判断
            cb(path, resObj, callbackObj);
        }
    }

    private UnityEngine.Object RegistResource(string path)
    {
        if (UnityTools.IsPackageResource(path))
            return null;
        UnityEngine.Object resObj = null;

        if (m_lResKeyList != null)
        {
            SRes iRes = m_lResKeyList.Find(SRes.MatchPath(path));
            if (iRes != null)
            {
                resObj = iRes.iResourceObject;
            }
            else
            {
                resObj = UnityResources.Load(path);
                if(null != resObj)
                {
                    iRes = new SRes(path, resObj);
                    m_lResKeyList.Add(iRes);
                }
            }

        }
        return resObj;
    }

    void HandlePackageResourceEvent(System.Object sender, ResourceRef.ResEventArgs args)
    {
        if (args.Success)
        {
            ResourceRef res = (ResourceRef)sender;
            List<SPendingRequest> ltFinishRequest = m_ltPendingRequest.FindAll(SPendingRequest.MatchPath(res.path));
            if (ltFinishRequest != null)
            {
                foreach (SPendingRequest request in ltFinishRequest)
                {
                    DebugTool.Log("load package ok");
                    request.callback(res.path, res.resource, request.callbackObj);
                    m_ltPendingRequest.Remove(request);
                }
            }
        }
        else
        {
            ResourceRef res = (ResourceRef)sender;
            List<SPendingRequest> ltFinishRequest = m_ltPendingRequest.FindAll(SPendingRequest.MatchPath(res.path));
            if (ltFinishRequest != null)
            {
                foreach (SPendingRequest request in ltFinishRequest)
                {
                    DebugTool.LogError("load package fail");
                    request.callback(res.path, null, request.callbackObj);
                    m_ltPendingRequest.Remove(request);
                }
            }
        }
    }

    IEnumerator TestLoad(string path, OnResourceReady cb, System.Object callbackObj)
    {
       string tmpPath = "file://C:/Projects/U3DFrame/Assets/StreamingAssets/package_ui.bytes";
        path = "file://" + GameConst.ResRootPath +  UnityTools.GetPackageName(path) + ".bytes";
        Debug.Log(tmpPath);
        WWW www = WWW.LoadFromCacheOrDownload(tmpPath, 0);
        yield return www;

//         byte[] bytes = File.ReadAllBytes(path);
//         AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
        //UnityEngine.Object obj = bundle.LoadAsset("Assets/Art/package_ui/test.prefab");

        //Debug.Log("obj === " + obj);
        //GameObject go = GameObject.Instantiate(obj) as GameObject;
        Debug.LogError("load bundle === " + www.assetBundle);
        yield return 0;
    }
}
