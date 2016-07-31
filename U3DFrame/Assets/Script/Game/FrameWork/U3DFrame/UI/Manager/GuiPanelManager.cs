using System.Collections.Generic;
using U3DFrame.Manager;
using UnityEngine;

public class ResourceObjInfo
{
    private UnityEngine.Object _resObj;
    public UnityEngine.Object ResObj { get { return _resObj; } }
    private string _resName;
    public string ResName { get { return _resName; } }
    private int _resCount;
    public int ResCount { get { return _resCount; } }
}

public class GuiPanelManager : Manager
{ 
    private Dictionary<string, GameObject> _myFormCache = new Dictionary<string, GameObject>();
    ResInfoRef _resInfoRef;
    private GameObject _uiRoot;
    public GameObject UiRoot
    {
        get { return _uiRoot; }
    }

    public void InitGUIManage(ResInfoRef resRef)
    {
        _myFormCache.Clear();
        _uiRoot = GameObject.Find("Camera");
        _resInfoRef = resRef;
    }

    public void LoadResource(string path, ResInfoRef.OnResourceReady cb, System.Object callParam = null)
    {
        //TODO 异步加载
        _resInfoRef.RegistResource(path, cb, callParam);
    }

    public bool isHaveDownLoad(string FormName)
    {
        if (_myFormCache.ContainsKey(FormName))
        {
            if (_myFormCache[FormName] != null)
            {
                return true;
            }
            else
            {
                _myFormCache.Remove(FormName);
            }
        }
        return false;
    }

    public void SetFormToCache(string cacheKey, GameObject ui)
    {
        if (isHaveDownLoad(cacheKey))
            return;

        _myFormCache[cacheKey] = ui;
    }

    public void RemoveFormFromCache(string cacheKey)
    {
        if (!_myFormCache.ContainsKey(cacheKey))
            return;
        _myFormCache.Remove(cacheKey);
    }

    public GameObject GetFormFromCache(string cacheKey)
    {
        if (!HasExistForm(cacheKey))
            return null;
        return _myFormCache[cacheKey];
    }

    public bool HasExistForm(string cacheKey)
    {
        return _myFormCache.ContainsKey(cacheKey);
    }

    public void OnRestore(GuiPanel iPanel)
    {
        if (null == iPanel)
            return;
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}

