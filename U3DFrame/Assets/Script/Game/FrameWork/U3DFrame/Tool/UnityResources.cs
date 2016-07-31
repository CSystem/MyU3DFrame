using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public class SharedResource
{
    private Object m_iObj = null;
    private int m_nRefCount = 0;
    public Object ResObj
    {
        get { return m_iObj; }
        set { m_iObj = value; }
    }
    public int RefCount
    {
        get { return m_nRefCount; }
        set { m_nRefCount = value; }
    }
}

public class UnityResources
{
    protected Dictionary<string, SharedResource> m_iResTable = new Dictionary<string, SharedResource>();
    private static UnityResources _instance;
    private static UnityResources Instance
    {
        get
        {
            if (null == _instance)
                _instance = new UnityResources();
            return _instance;
        }
    }

    public static Object Load(string path)
    {
        return Instance.DoLoad(path);
    }

    public static bool UnLoad(string path)
    {
        return Instance.DoUnLoad(path);
    }

    public static void DestroyUnUseRes()
    {
        Instance.DoDestroyUnUseRes();
    }

    private Object DoLoad(string path)
    {
        SharedResource res = null;

        if (m_iResTable.ContainsKey(path))
        {
            if(m_iResTable.TryGetValue(path, out res))
            {
                res.RefCount += 1;
                return res.ResObj;
            }
        }
        else
        {
            res = new SharedResource();
            res.RefCount = 1;
            res.ResObj = Resources.Load(path);
            if(null != res.ResObj)
            {
                m_iResTable.Add(path,res);
                return res.ResObj;
            }
        }
        return null;
    }
    
    private bool DoUnLoad(string path)
    {
        SharedResource res = null;

        if (m_iResTable.ContainsKey(path))
        {
            if (m_iResTable.TryGetValue(path, out res))
            {
                res.RefCount -= 1;
                //TODO 临时把引用计数销毁放在这里
                DoDestroyUnUseRes();
                return true;
            }
        }
            return false;     
    }

    private void DoDestroyUnUseRes()
    {
        IEnumerator<KeyValuePair<string ,SharedResource>> enumer = m_iResTable.GetEnumerator();
        if (null == enumer)
            return;
        enumer.Reset();
        while(enumer.MoveNext())
        {
           if(enumer.Current.Value.RefCount <= 0)
            {
                m_iResTable.Remove(enumer.Current.Key);
            }
        }
    }
}

