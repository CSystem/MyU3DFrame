using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PackageBuilder
{
    public const string PACKAGE_INFO_EXT = ".txt";
    public const string DATA_EXT = ".bytes";
    public const string BUNDLE_EXT = ".unity3d";
    public const string VERSION_EXT = ".version";

#if UNITY_STANDALONE_OSX || UNITY_IPHONE
    public const int SCENE_WARNING_MEMORY = 10 * 1024 * 1024;
#else
    public const int SCENE_WARNING_MEMORY = 20 * 1024 * 1024;
#endif
    public const int SCENE_WARNING_PACKAGE_TOTAL_SIZE = 20 * 1024 * 1024;

    public const string SHADER_PACKAGE = "package_ui_shader";
    public const string SHADER_PACKAGE_PATH = "Assets/UISource/package_ui_shader";
    public const string SHADER_PACKAGE_NATIVE_PATH = "Assets/UISource/package_ui_shader/NativeRef/";

    public const string SCRIPT_PACKAGE = "package_DevScript";

    static List<Shader> m_ltNativeRefShader = new List<Shader>();

    class PackageInfo
    {
        public string packageName;
        public string packagePath;
        public string[] packageAssets;
        public List<PackageInfo> ltDepPackages;

        public string GetBundlePath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + packageName + BUNDLE_EXT;
        }

        public string GetInfoPath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + packageName + PACKAGE_INFO_EXT;
        }

        public string GetDataPath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + packageName + DATA_EXT;
        }

        public string GetVersionPath(BuildTarget buildTarget)
        {
            return GetDataPath(buildTarget) + VERSION_EXT;
        }

        public static System.Predicate<PackageInfo> MatchName(string strPackageName)
        {
            return delegate(PackageInfo info)
            {
                return info.packageName == strPackageName;
            };
        }

        public static int CompareDepdence(PackageInfo x, PackageInfo y)
        {
            if (x.packageName == y.packageName)
                return 0;

            bool bXDepY = x.ltDepPackages.Exists(PackageInfo.MatchName(y.packageName));
            bool bYDepX = y.ltDepPackages.Exists(PackageInfo.MatchName(x.packageName));
            if (bXDepY && bYDepX)
            {
                Debug.LogError("在包" + x.packageName + "和包" + y.packageName + "中存在相互依赖的项");
                return 0;
            }
            else if (!bXDepY && !bYDepX)
            {
                return 0;
            }
            else if (bXDepY && !bYDepX)
            {
                return 1;
            }
            else if (!bXDepY && bYDepX)
            {
                return -1;
            }
            else
            {
                //不可到达
                return 0;
            }
        }
    }

    class SceneInfo
    {
        public string sceneName;
        public string scenePath;
        public List<PackageInfo> ltDepPackages;
        public List<string> ltUnpackageAsset;

        public string GetBundlePath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + sceneName + BUNDLE_EXT;
        }

        public string GetInfoPath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + sceneName + PACKAGE_INFO_EXT;
        }

        public string GetDataPath(BuildTarget buildTarget)
        {
            return BuilderTools.GetOutputPath(buildTarget) + sceneName + DATA_EXT;
        }

        public string GetVersionPath(BuildTarget buildTarget)
        {
            return GetDataPath(buildTarget) + VERSION_EXT;
        }

        public int GetTotalPackageSize(BuildTarget buildTarget) 
        {
            int nTotal = 0;

            int nSelfSize = GetFileSize(GetDataPath(buildTarget));
            if (nSelfSize < 0)
            {
                Debug.LogError("找不到场景包文件: " + sceneName);
            }
            nTotal += nSelfSize;
            foreach (PackageInfo depPackage in ltDepPackages)
            {
                int nPackSize = GetFileSize(depPackage.GetDataPath(buildTarget));
                if (nPackSize < 0)
                {
                    Debug.LogError("找不到资源包文件: " + depPackage.packageName);
                }
                nTotal += nPackSize;
            }

            return nTotal;
        }

        int GetFileSize(string fullPath)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(fullPath);
            if (info != null && info.Exists)
            {
                return (int)info.Length;
            }
            return -1;
        }
    }

    static bool KeepTempBundleAndInfoFile = true;
    static bool ContinueAll = false;

    delegate void FuncBuildOneTarget(BuildTarget buildTarget);

    [MenuItem("Builder/CheckDependenceInScp")]
    static public void CheckDependenceInScp()
    {
        string[] paths = new string[] { AssetDatabase.GetAssetPath(Selection.activeObject) };
        string[] arrDepAssetPath = AssetDatabase.GetDependencies(paths);
        string info = "Dep is: ";
        foreach (string dep in arrDepAssetPath)
        {
            info += "\n" + dep;
        }
        Debug.Log(info);
    }

    [MenuItem("Builder/BuildSelectPackage &b")] // Alt + b
    static public void BuildSelectPackage()
    {
        InitBuildProgress();

        List<PackageInfo> ltPackage = new List<PackageInfo>();
        if (!GetSelectPackage(ref ltPackage))
        {
            Debug.LogWarning("No selected package");
            return;
        }

        List<PackageInfo> ltPackageToBuild = new List<PackageInfo>();//所有需要打包的资源包，包括现有的和依赖的
        GetPackageToBuildByReference(ltPackage, ref ltPackageToBuild);

        //按照依赖关系将资源包排序
        ltPackageToBuild = SortPackage(ltPackageToBuild);

        BuildPipeline.PushAssetDependencies();

        int nPushLevel = 0;
        foreach (PackageInfo info in ltPackageToBuild)
        {
            BuildPipeline.PushAssetDependencies();  //要保证所有的包层层嵌套，不然依赖项有问题
            nPushLevel++;

            BuildPackage(info, EditorUserBuildSettings.activeBuildTarget);
        }

        for (int i = 0; i < nPushLevel; ++i)
        {
            BuildPipeline.PopAssetDependencies();
        }

        BuildPipeline.PopAssetDependencies();
    }


    [MenuItem("Builder/BuildSelectScene &s")]   // Alt + s
    static public void BuildSelectScene()
    {
        InitBuildProgress();

        List<SceneInfo> ltScene = new List<SceneInfo>();
        if (!GetSelectScene(ref ltScene))
        {
            Debug.LogWarning("No selected scene");
            return;
        }

        //计算每个场景的依赖项，生成依赖包的列表
        List<PackageInfo> ltBaseDepPackage = new List<PackageInfo>();
        foreach (SceneInfo info in ltScene)
        {
            int nMemory = BuilderTools.GetTotalDependAssetMemory(info.scenePath);
            if (nMemory > SCENE_WARNING_MEMORY)
            {
                string err = "场景的内存太大了: " + info.scenePath + " 占用内存: " + nMemory.ToString("N");
                Debug.LogError(err);
                if(!ContinueAll)
                {
                    int r = EditorUtility.DisplayDialogComplex("内存错误", err, "继续生成", "停止生成", "全部继续");
                    if (r == 1)//cancel
                        return;
                    else if (r == 2)//all
                        ContinueAll = true;
                }
            }

            if (EditorApplication.OpenScene(info.scenePath))
            {
//                 if (!GameLogicCheck.CheckCurrentSceneValid())
//                 {
//                     string err = "场景中包含错误: " + info.scenePath;
//                     Debug.LogError(err);
//                     if(!ContinueAll)
//                     {
//                         int r = EditorUtility.DisplayDialogComplex("场景错误", err, "继续生成", "停止生成", "全部继续");
//                         if (r == 1)//cancel
//                             return;
//                         else if (r == 2)//all
//                             ContinueAll = true;
//                     }
//                 }
            }

            CheckSceneDependence(info);

            foreach (PackageInfo dep_pack in info.ltDepPackages)
            {
                if (ltBaseDepPackage.Find(delegate(PackageInfo _info) { return (bool)(_info.packageName == dep_pack.packageName); }) == null)
                {
                    ltBaseDepPackage.Add(dep_pack);
                }
            }
        }

        List<PackageInfo> ltPackageToBuild = new List<PackageInfo>();//所有需要打包的资源包，包括现有的和依赖的
        GetPackageToBuildByReference(ltBaseDepPackage, ref ltPackageToBuild);

        //按照依赖关系将资源包排序
        ltPackageToBuild = SortPackage(ltPackageToBuild);

        //先生成包，再生成场景，两层依赖关系
        BuildPipeline.PushAssetDependencies();
        int nPushLevel = 0;
        foreach (PackageInfo info in ltPackageToBuild)
        {
            BuildPipeline.PushAssetDependencies();  //要保证所有的包层层嵌套，不然依赖项有问题
            nPushLevel++;

            BuildPackage(info, EditorUserBuildSettings.activeBuildTarget);
        }
        foreach (SceneInfo info in ltScene)
        {
            BuildScene(info, EditorUserBuildSettings.activeBuildTarget);

            int nTotalPackageSize = info.GetTotalPackageSize(EditorUserBuildSettings.activeBuildTarget);
            string err = "场景包: " + info.sceneName + " 以及其依赖项的总大小: " + nTotalPackageSize.ToString("N");
            if (nTotalPackageSize < SCENE_WARNING_PACKAGE_TOTAL_SIZE)
            {
                Debug.Log(err);
            }
            else
            {
                Debug.LogError(err);
                if (!ContinueAll)
                {
                    EditorUtility.DisplayDialog("场景包大小错误", err, "确定");
                }
            }
        }
        for (int i = 0; i < nPushLevel; ++i)
        {
            BuildPipeline.PopAssetDependencies();
        }

        BuildPipeline.PopAssetDependencies();
    }

    private static void InitBuildProgress()
    {
        ContinueAll = false;
        m_ltNativeRefShader.Clear();
    }

    [MenuItem("Builder/CopyResourcePath &c")]   // Alt + c
    static public void CopyResourcePath()
    {
        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string guid = AssetDatabase.AssetPathToGUID(path);
            Debug.Log("Asset path: " + path + " guid: " + guid + " hashcode: " + Selection.activeObject.GetHashCode().ToString());
            EditorGUIUtility.systemCopyBuffer = path;
        }
    }

    [MenuItem("Builder/检查选中资源的内存 &m")]   // Alt + m
    static public void CheckAssetMemery()
    {
        List<BuilderTools.SAssetMemoryInfo> ltMemoryInfo = new List<BuilderTools.SAssetMemoryInfo>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        int nMemory = BuilderTools.GetTotalDependAssetMemory(path, ltMemoryInfo);
        if (nMemory < SCENE_WARNING_MEMORY)
        {
            Debug.Log("Total memory of " + path + ": " + nMemory.ToString("N"));
        }
        else
        {
            Debug.LogError(path + "占用内存: " + nMemory.ToString("N"));
        }

        ltMemoryInfo.Sort(BuilderTools.SAssetMemoryInfo.CompareSizeDown);
        foreach (BuilderTools.SAssetMemoryInfo info in ltMemoryInfo)
        {
            Debug.Log("DepAsset " + info.assetObject.ToString() + "\n"
                + " size " + info.memorySize.ToString("N0") + "\n"
                + " type: " + info.assetObject.GetType().ToString() + "\n"
                + " name: " + info.assetObject.name + "\n"
                + " path: " + AssetDatabase.GetAssetPath(info.assetObject)
                , info.assetObject);
        }
    }

    [MenuItem("Builder/检查选中目录资源的内存")]   // Alt + m
    static public void CheckAssetMemeryInFolder()
    {
        List<BuilderTools.SAssetMemoryInfo> ltMemoryInfo = new List<BuilderTools.SAssetMemoryInfo>();

        UnityEngine.Object[] arrSelectionObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (UnityEngine.Object obj in arrSelectionObject)
        {
            if (obj is GameObject)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.Contains(".prefab"))
                {
                    int nMemory = BuilderTools.GetTotalDependAssetMemory(path);
                    BuilderTools.SAssetMemoryInfo info = new BuilderTools.SAssetMemoryInfo();
                    info.assetObject = obj;
                    info.assetPath = path;
                    info.memorySize = nMemory;
                    ltMemoryInfo.Add(info);
                }
            }
        }

        ltMemoryInfo.Sort(BuilderTools.SAssetMemoryInfo.CompareSizeDown);
        string strTotalInfo = "Name\tSize\tType\n";
        foreach (BuilderTools.SAssetMemoryInfo info in ltMemoryInfo)
        {
            Debug.Log("Asset " + info.assetObject.ToString() + "\n"
                + " size " + info.memorySize.ToString("N0") + "\n"
                + " type: " + info.assetObject.GetType().ToString() + "\n"
                + " name: " + info.assetObject.name + "\n"
                + " path: " + AssetDatabase.GetAssetPath(info.assetObject)
                , info.assetObject);
            strTotalInfo += info.assetObject.name.Replace(" ", "_") + "\t" + info.memorySize.ToString("N0") + "\t" + info.assetObject.GetType().ToString() + "\n";
        }
        EditorGUIUtility.systemCopyBuffer = strTotalInfo;
        Debug.Log("Memory info is copied to clipboard");
    }

    [MenuItem("Builder/CheckScene &k")]   // Alt + k
    static public void CheckCurrentScene()
    {
        string path = EditorApplication.currentScene;
        if(path != null && path.Length > 0)
        {
            int nMemory = BuilderTools.GetTotalDependAssetMemory(path);
            if (nMemory < SCENE_WARNING_MEMORY)
            {
                Debug.Log("Total memory of " + path + ": " + nMemory.ToString("N"));
            }
            else
            {
                Debug.LogError(path + "占用内存: " + nMemory.ToString("N"));
            }
        }
//         if (GameLogicCheck.CheckCurrentSceneValid())
//         {
//             Debug.Log("Check finish");
//         }
//         else
//         {
//             Debug.LogError("场景中存在一些错误");
//         }
    }

    /// <summary>
    /// 计算所有需要打的包，包括传入的包和所有被引用的包
    /// </summary>
    /// <param name="ltBasePackage"></param>
    /// <param name="ltPackageToBuild"></param>
    static void GetPackageToBuildByReference(List<PackageInfo> ltBasePackage, ref List<PackageInfo> ltPackageToBuild)
    {
        Queue<PackageInfo> qPackToCheck = new Queue<PackageInfo>();
        foreach (PackageInfo info in ltBasePackage)
        {
            qPackToCheck.Enqueue(info);
            ltPackageToBuild.Add(info);
        }

        //强制把shader的包放在打包队列中
        if(!ltPackageToBuild.Exists(PackageInfo.MatchName(SHADER_PACKAGE)))
        {
            PackageInfo packageShader = new PackageInfo();
            packageShader.packageName = SHADER_PACKAGE;
            packageShader.packagePath = SHADER_PACKAGE_PATH;
            qPackToCheck.Enqueue(packageShader);
            ltPackageToBuild.Add(packageShader);
        }

        while (qPackToCheck.Count > 0)
        {
            PackageInfo info = qPackToCheck.Dequeue();
            CheckDependence(info);
			if(info.packageAssets.Length == 0)
			{
				Debug.LogWarning("There is an empty package " + info.packageName);
				ltPackageToBuild.Remove(info);
				continue;//this is an empty package
			}
            foreach (PackageInfo dep_info in info.ltDepPackages)
            {
                if (ltPackageToBuild.Find(delegate(PackageInfo i) { return (bool)(i.packageName == dep_info.packageName); }) == null)
                {
                    PackageInfo new_info = new PackageInfo();
                    new_info.packageName = dep_info.packageName;
                    new_info.packagePath = dep_info.packagePath;
                    qPackToCheck.Enqueue(new_info);
                    ltPackageToBuild.Add(new_info);
                }
            }
        }
    }

    /// <summary>
    /// 计算一个包的依赖项
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    static bool CheckDependence(PackageInfo info)
    {
        //计算依赖项，并写入文件
        if (!GetAllAssetPathInPackage(ref info))
            return false;

        info.ltDepPackages = new List<PackageInfo>();
        List<string> ltUnPackagedAssets = new List<string>();
        GetDependencePackageList(info.packageAssets, ref info.ltDepPackages, ref ltUnPackagedAssets);

        if (ltUnPackagedAssets.Count > 0)
        {
            string errAssets = "";
            foreach (string s in ltUnPackagedAssets)
            {
                errAssets += s + "\n";
            }
            Debug.LogError("包所依赖的资源不在包中：\n" + errAssets);
        }

        return true;
    }

    /// <summary>
    /// 对传入的链表根据依赖关系排序，返回排序后的链表
    /// </summary>
    /// <param name="ltPackageToBuild"></param>
    static List<PackageInfo> SortPackage(List<PackageInfo> ltPackage)
    {
        List<PackageInfo> ltRawPackage = new List<PackageInfo>();
        ltRawPackage.InsertRange(0, ltPackage);

        System.Predicate<PackageInfo> IsTopPackage = p =>
        {
            if (p.packageName == SHADER_PACKAGE && ltRawPackage.Count > 1)    //强制shader的包最后一个被取走，也就是最终排序会出现在第一位
                return false;

            foreach (PackageInfo exist in ltRawPackage)
            {
                if (exist.packageName != p.packageName
                    && exist.ltDepPackages.Exists(delegate(PackageInfo d) { return d.packageName == p.packageName; }))
                    return false;
            }
            return true;
        };

        List<PackageInfo> ltNewList = new List<PackageInfo>();
        while (ltRawPackage.Count > 0)
        {
            PackageInfo top = ltRawPackage.Find(IsTopPackage);
            if (top != null)
            {
                ltRawPackage.Remove(top);
                ltNewList.Insert(0, top);
            }
            else
            {
                string errPackage = "";
                foreach (PackageInfo p in ltRawPackage)
                {
                    errPackage += p.packageName + "\n";
                    ltNewList.Insert(0, p);
                }
                Debug.LogError("存在循环依赖的包在：\n" + errPackage + "之中");
                break;
            }
        }

        return ltNewList;
    }

    /// <summary>
    /// 导出一个指定的包
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    static bool BuildPackage(PackageInfo info, BuildTarget buildTarget)
    {
        //打包package下的所有资源
        UnityEngine.Object[] arrAssetObject = new UnityEngine.Object[info.packageAssets.Length];
        string[] arrAssetNameInPackage = new string[info.packageAssets.Length];
        for (int i = 0; i < info.packageAssets.Length; ++i)
        {
			string assetPath = info.packageAssets[i];
            arrAssetObject[i] = AssetDatabase.LoadMainAssetAtPath(assetPath);
            arrAssetNameInPackage[i] = GetAssetNameInPackage(assetPath).ToLower();//包中物体的名字，目前是相对与package目录的完整路径，以后可以修改

            CheckAssetValid(assetPath, arrAssetObject[i]);
        }

        //这里使用CollectDependencies，但是由于外层使用了PushDependcies，所以依赖项可能是在其他的包中
        string strOutputPath = info.GetBundlePath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + info.packageName + BUNDLE_EXT;
        BuildPipeline.BuildAssetBundleExplicitAssetNames(arrAssetObject, arrAssetNameInPackage, strOutputPath
            , BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget);

        //生成信息文件
        string strPackageInfoFilePath = info.GetInfoPath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + info.packageName + PACKAGE_INFO_EXT;
        WritePackageInfoFile(strPackageInfoFilePath, info.ltDepPackages);

        //生成版本信息文件，依赖项的信息要做为md5的一部分
        string strVersionFilePath = info.GetVersionPath(buildTarget);
        string strVersion = BuilderTools.GetVerifyString(new string[] { strPackageInfoFilePath }, new string[] { info.packagePath });
        File.WriteAllText(strVersionFilePath, strVersion);

        //写入统一的dat文件
        string strDataFilePath = info.GetDataPath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + info.packageName + DATA_EXT;
        CombineFinalPackage(strPackageInfoFilePath, strOutputPath, strDataFilePath);
        Debug.Log("writed output file: " + strDataFilePath);
        return true;
    }

    /// <summary>
    /// 检查一个资源的合法性，包括图片格式，系统shader的引用是否正确
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="assetObj"></param>
    /// <returns></returns>
    static bool CheckAssetValid(string assetPath, UnityEngine.Object assetObj)
    {
        if (assetPath.Contains(".dds"))
        {
            Debug.LogError("invalid texture format for ios: " + assetPath, assetObj);
            return false;
        }

        if (assetObj is GameObject)
        {
            //GameObject相关检查
        }
        else if (assetObj is Material)
        {
            Material mat = (Material)assetObj;
            Shader shader = mat.shader;
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            if (shaderPath == null || shaderPath.Length == 0)
            {
                if (!IsNativeRefShader(shader))
                {
                    Debug.LogError(shader.ToString() + "是系统shader，但是没有增加到系统引用中。(在" + SHADER_PACKAGE_NATIVE_PATH + "中建立一个Material，然后引用此shader", assetObj);
                }
            }
            else if(!shaderPath.Contains(SHADER_PACKAGE))
            {
                Debug.LogError(shaderPath + "没有包含在" + SHADER_PACKAGE + "之中", assetObj);
            }
        }

        return true;
    }

    /// <summary>
    /// 判断一个shader是否添加到了本地shader中
    /// </summary>
    /// <param name="shader"></param>
    /// <returns></returns>
    static bool IsNativeRefShader(Shader shader)
    {
        if (m_ltNativeRefShader.Count == 0)
        {
            string[] arrNativeRef = BuilderTools.GetAllAssetInPath(SHADER_PACKAGE_NATIVE_PATH);
            foreach (string assetRefPath in arrNativeRef)
            {
                UnityEngine.Object assetRef = AssetDatabase.LoadMainAssetAtPath(assetRefPath);
                if (assetRef != null && assetRef is Material)
                {
                    m_ltNativeRefShader.Add(((Material)assetRef).shader);
                    //Debug.Log("find native ref: " + ((Material)assetRef).shader.ToString());
                }
            }
        }

        return m_ltNativeRefShader.Contains(shader);
    }

    /// <summary>
    /// 获取当前Project面板中选中的包的信息，如果成功则返回true
    /// </summary>
    /// <param name="ltPackage"></param>
    /// <returns></returns>
    static bool GetSelectPackage(ref List<PackageInfo> ltPackage)
    {
        if (ltPackage == null)
            return false;

        UnityEngine.Object[] arrSelectionObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        foreach (UnityEngine.Object obj in arrSelectionObject)
        {
            string strAssetPath = AssetDatabase.GetAssetPath(obj);
            PackageInfo info = new PackageInfo();
            if (GetPackageInfo(strAssetPath, ref info))
            {
                if (null == ltPackage.Find(delegate(PackageInfo _info) { return (bool)(_info.packageName == info.packageName); }))
                {
                    ltPackage.Add(info);
                }
            }
        }

        return ltPackage.Count > 0;
    }

    /// <summary>
    /// 检查一个场景所引用的包
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    static bool CheckSceneDependence(SceneInfo scene)
    {
        //获取所有依赖的包
        scene.ltDepPackages = new List<PackageInfo>();
        scene.ltUnpackageAsset = new List<string>();
        GetDependencePackageList(new string[] { scene.scenePath }, ref scene.ltDepPackages, ref scene.ltUnpackageAsset);

        return true;
    }

    /// <summary>
    /// 打一个单独的场景包，包的依赖项已经在之前的逻辑中打出来了
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    static bool BuildScene(SceneInfo scene, BuildTarget buildTarget)
    {
        string strOutputPath = scene.GetBundlePath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + scene.sceneName + BUNDLE_EXT;
        BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { scene.scenePath }, strOutputPath, buildTarget);

        //生成信息文件
        string strPackageInfoFilePath = scene.GetInfoPath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + scene.sceneName + PACKAGE_INFO_EXT;
        WritePackageInfoFile(strPackageInfoFilePath, scene.ltDepPackages);

        //生成版本信息文件，依赖项信息要做为md5的一部分
        string strVersionFilePath = scene.GetVersionPath(buildTarget);
        string[] arrVerifyAssests = new string[scene.ltUnpackageAsset.Count + 1];
        System.Array.Copy(scene.ltUnpackageAsset.ToArray(), arrVerifyAssests, scene.ltUnpackageAsset.Count);
        arrVerifyAssests[arrVerifyAssests.Length - 1] = strPackageInfoFilePath;
//         for (int i = 0; i < scene.ltUnpackageAsset.Count; ++i)
//         {
//             arrVerifyAssests[i] = scene.ltUnpackageAsset[i];
//         }
        string strVersion = BuilderTools.GetVerifyString(arrVerifyAssests, null);
        File.WriteAllText(strVersionFilePath, strVersion);

        //写入统一的dat文件
        string strDataFilePath = scene.GetDataPath(buildTarget);// BuilderTools.GetOutputPath(buildTarget) + scene.sceneName + DATA_EXT;
        CombineFinalPackage(strPackageInfoFilePath, strOutputPath, strDataFilePath);
        Debug.Log("writed output file: " + strDataFilePath);
        return true;
    }

    /// <summary>
    /// 将包信息写入文件
    /// </summary>
    /// <param name="strFilePath"></param>
    /// <param name="ltDepPackages"></param>
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

    /// <summary>
    /// 获取所有选中的场景
    /// </summary>
    /// <param name="ltScenePackage"></param>
    /// <returns></returns>
    static bool GetSelectScene(ref List<SceneInfo> ltScene)
    {
        if (ltScene == null)
            return false;

        UnityEngine.Object[] arrSelectionObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (UnityEngine.Object obj in arrSelectionObject)
        {
            string strAssetPath = AssetDatabase.GetAssetPath(obj);
            FileInfo fileInfo = new FileInfo(BuilderTools.AssetPathToFullPath(strAssetPath));
            if (fileInfo == null
                || fileInfo.Extension != ".unity")  //如果不是场景文件，就要跳过
                continue;

            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.sceneName = obj.name;
            sceneInfo.scenePath = strAssetPath;
            ltScene.Add(sceneInfo);
        }

        return ltScene.Count > 0;
    }

    //const string STR_SHDAER_PACK_NAME = "package_shader";
    //const string STR_SHDAER_PACK_PATH = "Assets/Source_Common/package_shader";
    static bool GetDependencePackageList(string[] arrAssets, ref List<PackageInfo> ltDepPackage, ref List<string> ltUnPackagedAssets)
    {
        string[] arrDepAssetPath = AssetDatabase.GetDependencies(arrAssets);

        bool bIncludedShaderPack = false;
        for (int i = 0; i < arrDepAssetPath.Length; ++i)
        {
            string name = BuilderTools.GetPackageName(arrDepAssetPath[i]);
            if (name == SHADER_PACKAGE)
            {
                bIncludedShaderPack = true;
                break;
            }
        }


        foreach (string depAsset in arrDepAssetPath)
        {
            PackageInfo dep_info = new PackageInfo();
            if (GetPackageInfo(depAsset, ref dep_info))
            {
                if (null == ltDepPackage.Find(delegate(PackageInfo _info) { return (bool)(_info.packageName == dep_info.packageName); }))
                {
                    ltDepPackage.Add(dep_info);
                }
            }
            else
            {
                if (!depAsset.Contains(".cs")
                    && !depAsset.Contains(".js")
                    && !depAsset.Contains("T4M/Shaders"))
                {
                    ltUnPackagedAssets.Add(depAsset);
                }
            }
        }

        if (!bIncludedShaderPack)
        {
            PackageInfo shaderInfo = new PackageInfo();
            shaderInfo.packageName = SHADER_PACKAGE;
            shaderInfo.packagePath = SHADER_PACKAGE_PATH;

            ltDepPackage.Add(shaderInfo);
        }
        return true;
    }

    /// <summary>
    /// 通过一个Asset资源路径获取其所属的包的信息，如果它不属于一个包，则返回失败。
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="info"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 获取一个包中所有的Asset目录，保存在Package信息中
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    static bool GetAllAssetPathInPackage(ref PackageInfo info)
    {
        if (info == null)
            return false;

        info.packageAssets = BuilderTools.GetAllAssetInPath(info.packagePath);
        return true;
    }

    /// <summary>
    /// 将信息文件和bundle文件打包成一个data文件
    /// </summary>
    /// <param name="infoFile">要读取的信息文件</param>
    /// <param name="bundleFile">要读取的bundle文件</param>
    /// <param name="dataFile">要写入的data文件</param>
    static void CombineFinalPackage(string infoFile, string bundleFile, string dataFile)
    {
        byte[] byteInfo = File.ReadAllBytes(infoFile);
        byte[] byteBundle = File.ReadAllBytes(bundleFile);

        if (byteInfo == null || byteBundle == null)
            return;

        try
        {
            MemoryStream stream = new MemoryStream(byteInfo.Length + byteBundle.Length + 8);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(byteInfo.Length);
            writer.Write(byteInfo);
            writer.Write(byteBundle.Length);
            writer.Write(byteBundle);
            File.WriteAllBytes(dataFile, stream.ToArray());
            writer.Close();
            stream.Close();

            if (!KeepTempBundleAndInfoFile)
            {
                File.Delete(infoFile);
                File.Delete(bundleFile);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Build dat file fail: " + dataFile + e.Message);
        }
    }

    

    /// <summary>
    /// 获得一个资源在包中的名字，这个名字是资源在包中的唯一标识，用来加载
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    static string GetAssetNameInPackage(string assetPath)
    {
        string r = BuilderTools.CaptureWord(assetPath, @"package\w+\/(.*)");
        return r;
    }
}
