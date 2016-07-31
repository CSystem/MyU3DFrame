using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using U3DFrame.Json;
using U3DFrame.Tool;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class BuildPackages
{  
    [MenuItem("Build/Build Android Resource", false, 101)]
    public static void BuildWindowsResource()
    {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Build/Set BundleName", false, 101)]
    public static void SetBundleName()
    {
        string[] path = Directory.GetDirectories(GameConst.ResRootPath, "*package*", SearchOption.AllDirectories);
        for (int i = 0; i < path.Length; i++)
        {
            string packageName = path[i].Substring(path[i].LastIndexOf("\\") + 1);
            Debug.Log("packageName === " + packageName);
            string realitivePath = path[i].Substring(path[i].IndexOf("Assets"));
            Debug.Log(realitivePath);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(realitivePath);
            AssetImporter importer = AssetImporter.GetAtPath(realitivePath);
            importer.assetBundleName = GameConst.UiResource + "/" + packageName + ".bytes";
            Debug.Log(obj);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static void BuildAssetResource(BuildTarget target)
    {
        string path = GameConst.ResRootPath;

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [MenuItem("Build/GetDep", false, 101)]
    public static void GetDep()
    {
        string path = Application.dataPath + "/Art/UiResource/package_ui/test.prefab";
        string realitivePath = path.Substring(path.IndexOf("Assets"));

        string[] arrDepAssetPath = AssetDatabase.GetDependencies(realitivePath);
        for(int i = 0; i < arrDepAssetPath.Length; i++)
        {
            Debug.Log(arrDepAssetPath[i]);
        }
        PackageInfo info = new PackageInfo();
        GetPackageInfo(path,ref info);
    }

    [MenuItem("Build/TestMainfest", false, 101)]
    public static void TestMainfest()
    {
        
        AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetInfo");
        AssetBundleManifest assetManifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        Debug.Log(assetManifest);
        string[] deps = assetManifest.GetAllDependencies("horse.bytes");
        Debug.Log(deps.Length);
//         for (int i = 0; i< deps.Length; i++)
//         {
//             Debug.Log(deps[i]);
//         }
        bundle.Unload(false);
    }
    [MenuItem("Build/RefreshAssetDataBase", false, 101)]
    public static void RefreshAssetDataBase()
    {
        Caching.CleanCache();
    }

    //传入包的路径，返回一个包的信息，包括名字和路径
    static bool GetPackageInfo(string assetPath, ref PackageInfo info)
    {
        assetPath += "/";
        string name = BuilderTools.GetPackageName(assetPath);
        string path = BuilderTools.GetPackagePath(assetPath);
        if (name.Length > 0 && path.Length > 0)
        {
            info.packageName = name;
            info.packagePath = path;
            return true;
        }
        else
        {
            return false;
        }
    }

    //static bool CheckDependence(PackageInfo info)
    //{
    //    //计算依赖项，并写入文件
    //    if (!GetAllAssetPathInPackage(ref info))
    //        return false;

    //    info.ltDepPackages = new List<PackageInfo>();
    //    List<string> ltUnPackagedAssets = new List<string>();
    //    GetDependencePackageList(info.packageAssets, ref info.ltDepPackages, ref ltUnPackagedAssets);

    //    if (ltUnPackagedAssets.Count > 0)
    //    {
    //        string errAssets = "";
    //        foreach (string s in ltUnPackagedAssets)
    //        {
    //            errAssets += s + "\n";
    //        }
    //        Debug.LogError("包所依赖的资源不在包中：\n" + errAssets);
    //    }

    //    return true;
    //}

    //static bool GetDependencePackageList(string[] arrAssets, ref List<PackageInfo> ltDepPackage, ref List<string> ltUnPackagedAssets)
    //{
    //    string[] arrDepAssetPath = AssetDatabase.GetDependencies(arrAssets);

    //    for (int i = 0; i < arrDepAssetPath.Length; ++i)
    //    {
    //        string name = BuilderTools.GetPackageName(arrDepAssetPath[i]);
    //    }


    //    foreach (string depAsset in arrDepAssetPath)
    //    {
    //        PackageInfo dep_info = new PackageInfo();
    //        if (GetPackageInfo(depAsset, ref dep_info))
    //        {
    //            if (null == ltDepPackage.Find(delegate (PackageInfo _info) { return (bool)(_info.packageName == dep_info.packageName); }))
    //            {
    //                ltDepPackage.Add(dep_info);
    //            }
    //        }
    //        else
    //        {
    //            if (!depAsset.Contains(".cs")
    //                && !depAsset.Contains(".js")
    //                && !depAsset.Contains("T4M/Shaders"))
    //            {
    //                ltUnPackagedAssets.Add(depAsset);
    //            }
    //        }
    //    }
    //    return true;
    //}
    [MenuItem("Build/GenerateDepAndMd5", false, 101)]
    public static void GenerateDepAndMd5()
    {
        Object obj = Selection.activeObject;
        if (null == obj)
            return;
        AssetBundleManifest manifest;
        AssetBundle bundle = AssetBundle.LoadFromFile(AssetDatabase.GetAssetPath(obj));
        if (null == bundle)
            return;
        manifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        if(null == manifest)
        {
            Debug.LogError("obj : " + obj.name + "dont contain AssetBundleManifest");
            return;
        }
        bundle.Unload(false);
        List<PackageInfo> lt = new List<PackageInfo>();
        string[] bundleNames = manifest.GetAllAssetBundles();
        for(int i = 0; i < bundleNames.Length; i++)
        {
            string name = Path.GetFileName(bundleNames[i]);
            string dir = GetPackageResPath(bundleNames[i]);
            PackageInfo info = new PackageInfo();
     
            Debug.Log(name);
        }
        
    }
    [MenuItem("Build/GenerateResouceRefFile", false, 101)]
    static void GenerateResouceRefFile()
    {
        //生成一个json文件存放所有资源的 名字，路径，依赖，hash
        Object obj = Selection.activeObject;
        if (null == obj)
            return;
        AssetBundleManifest manifest;
        AssetBundle bundle = AssetBundle.LoadFromFile(AssetDatabase.GetAssetPath(obj));
        if (null == bundle)
            return;
        manifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        if (null == manifest)
        {
            Debug.LogError("obj : " + obj.name + "dont contain AssetBundleManifest");
            return;
        }
        bundle.Unload(false);
        Hashtable uiRes = new Hashtable();
        Hashtable artRes = new Hashtable();
        Hashtable xmlRes = new Hashtable();
        string[] bundleNames = manifest.GetAllAssetBundles();
        for (int i = 0; i < bundleNames.Length; i++)
        {
            string name = Path.GetFileNameWithoutExtension(bundleNames[i]);
            Hashtable singleBundle = new Hashtable();
            singleBundle["name"] = name;
            singleBundle["path"] = bundleNames[i];
            singleBundle["hash"] = manifest.GetAssetBundleHash(bundleNames[i]).ToString();
            Hashtable deps = new Hashtable();
            string[] strDeps = manifest.GetAllDependencies(bundleNames[i]);
            for (int j = 0; j < strDeps.Length; j++)
            {
                deps["dependent_" + j] = Path.GetFileNameWithoutExtension(strDeps[j]);
            }

            singleBundle["dependencies"] = deps;
            E_RES_TYPE eType = GetPackageResType(bundleNames[i]);
            switch(eType)
            {
                case E_RES_TYPE.E_ArtResource:
                    artRes[name] = singleBundle;
                    break;
                case E_RES_TYPE.E_Root:
                    break;
                case E_RES_TYPE.E_XmlResource:
                    xmlRes[name] = singleBundle;
                    break;
                case E_RES_TYPE.E_UiResource:
                    uiRes[name] = singleBundle;
                    break;
            }

        }
        string uiJson = MiniJSON.JsonEncode(uiRes);
        string artJson = MiniJSON.JsonEncode(artRes);
        string xmlJson = MiniJSON.JsonEncode(xmlRes);

        Debug.Log(uiJson);
        string uiFilePath = Application.streamingAssetsPath + "/uiresource/";
        string artFilePath = Application.streamingAssetsPath + "/artresource/";
        string xmlFilePath = Application.streamingAssetsPath + "/xmlresource/";

        if (!Directory.Exists(uiFilePath))
            Directory.CreateDirectory(uiFilePath);

        if (!Directory.Exists(artFilePath))
            Directory.CreateDirectory(artFilePath);

        if (!Directory.Exists(xmlFilePath))
            Directory.CreateDirectory(xmlFilePath);

        WriteIndexFile(uiFilePath + "index.txt", uiJson);
        WriteIndexFile(artFilePath + "index.txt", artJson);
        WriteIndexFile(xmlFilePath + "index.txt", xmlJson);
    }

    [MenuItem("Build/CombineIndexFile", false, 101)]
    static private void CombineIndexFile()
    {
        string uiFilePath = Application.streamingAssetsPath + "/uiresource/";
        string artFilePath = Application.streamingAssetsPath + "/artresource/";
        string xmlFilePath = Application.streamingAssetsPath + "/xmlresource/";
        string root = Application.streamingAssetsPath + "/";

        string indexName = "index.info";

        string rootIndex = File.ReadAllText(root + indexName);
        Hashtable rootTable = MiniJSON.JsonDecode(rootIndex) as Hashtable;
        foreach(string key in rootTable.Keys)
        {
            Debug.Log("key : " + key);
        }

        //合并UI
        if(File.Exists(uiFilePath + indexName))
        {
            string uiInfo = File.ReadAllText(uiFilePath + indexName);
            Hashtable uiInfoTable = MiniJSON.JsonDecode(uiInfo) as Hashtable;
            foreach (string uikey in uiInfoTable.Keys)
            {
                if(rootTable.ContainsKey(uikey))
                {
                    rootTable[uikey] = uiInfoTable[uikey];
                }
                Debug.Log("uikey : " + uikey);

            }
        }
        //合并美术资源
        if(File.Exists(artFilePath + indexName))
        {
            string artInfo = File.ReadAllText(artFilePath + indexName);
            Hashtable artTable = MiniJSON.JsonDecode(artInfo) as Hashtable;
            foreach (string artKey in artTable.Keys)
            {
                if (rootTable.ContainsKey(artKey))
                {
                    rootTable[artKey] = artTable[artKey];
                }
                Debug.Log("artKey : " + artKey);
            }
        }
        //合并XML
        if(File.Exists(xmlFilePath + indexName))
        {
            string xmlInfo = File.ReadAllText(xmlFilePath + indexName);
            Hashtable xmlTable = MiniJSON.JsonDecode(xmlInfo) as Hashtable;
            foreach(string xmlKey in xmlTable.Keys)
            {
                if(rootTable.ContainsKey(xmlKey))
                {
                    rootTable[xmlKey] = xmlTable[xmlKey];
                }
                Debug.Log("xmlKey : " + xmlKey);
            }
        }

        WriteIndexFile(root + indexName, MiniJSON.JsonEncode(rootTable));
        Debug.Log("rootIndex : " + rootIndex);
    }

    static void WriteIndexFile(string filePath, string content)
    {
        FileStream infoFile = new FileStream(filePath, FileMode.Create);
        StreamWriter writer = new StreamWriter(infoFile);
        writer.Write(content);
        writer.Close();
        infoFile.Close();
    }

    static void WritePackageInfoFile(string strFilePath, List<PackageInfo> ltDepPackages)
    {
        try
        {
            FileStream infoFile = new FileStream(strFilePath, FileMode.Create);
            StreamWriter writer = new StreamWriter(infoFile);
            if (ltDepPackages != null)
            {
                foreach (PackageInfo depInfo in ltDepPackages)
                {
                    writer.WriteLine(depInfo.packageName);
                }
            }
            writer.Close();
            infoFile.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Write dep file fail. " + e.Message);
        }
    }
}
