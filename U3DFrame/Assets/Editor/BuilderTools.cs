using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

public class BuilderTools
{
    static public string CaptureWord(string src, string reg)
    {
        Regex _reg = new Regex(reg);
        Match mc = _reg.Match(src);
        if (mc.Groups.Count > 1 && mc.Groups[1].Captures.Count > 0)
        {
            return mc.Groups[1].Captures[0].Value;
        }
        else
        {
            return "";
        }
    }

    static public string GetFileNameWithoutExt(string assetPath)
    {
        int nBegin = assetPath.LastIndexOf('/');
        int nEnd = assetPath.LastIndexOf('.');
        if (nBegin > 0 && nEnd > 0)
        {
            return assetPath.Substring(nBegin + 1, nEnd - nBegin - 1);
        }
        else
        {
            return "";
        }
    }

    static public string GetAssetPathWithoutExt(string assetPath)
    {
        int nEnd = assetPath.LastIndexOf('.');
        if (nEnd > 0)
        {
            return assetPath.Substring(0, nEnd);
        }
        else
        {
            return "";
        }
    }

    static public string GetPackageName(string assetPath)
    {
        return CaptureWord(assetPath, @"\/(package\w+)\/");
    }

    static public string GetPackagePath(string assetPath)
    {
        return CaptureWord(assetPath, @"(.*\/package\w+)\/");
    }

    static public string[] GetAllAssetInPath(string assetPath)
    {
        DirectoryInfo root = new DirectoryInfo(BuilderTools.AssetPathToFullPath(assetPath));
        if (root == null || !root.Exists)
            return new string[] {};

        string[] result = new string[] { };
        Queue<DirectoryInfo> qDirInfo = new Queue<DirectoryInfo>();
        qDirInfo.Enqueue(root);

        while (qDirInfo.Count > 0)
        {
            DirectoryInfo dir = qDirInfo.Dequeue();
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension != ".meta" && file.Name != ".DS_Store")
                {
                    System.Array.Resize<string>(ref result, result.Length + 1);
                    result[result.Length - 1] = BuilderTools.FullPathToAssetpath(file.FullName);
                }
            }

            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                if (subdir.Name != ".svn")
                {
                    qDirInfo.Enqueue(subdir);
                }
            }
        }

        return result;
    }

    static public string GetVerifyStringBySingleFile(string strPath)
    {
        MemoryStream iInputStream = new MemoryStream();
        byte[] input_data = File.ReadAllBytes(strPath);
        if (input_data != null)
        {
            iInputStream.Write(input_data, 0, input_data.Length);
        }
        iInputStream.Seek(0, SeekOrigin.Begin);

        MD5 md5Hash = MD5.Create();
        string result = HashToString(md5Hash.ComputeHash(iInputStream));
        iInputStream.Close();

        return result;
    }

    /// <summary>
    /// 获得制定文件夹和文件的总md5字符串
    /// </summary>
    /// <param name="files"></param>
    /// <param name="paths"></param>
    /// <returns></returns>
    static public string GetVerifyString(string[] files, string[] paths)
    {
        System.Predicate<string> IsAssetFile = delegate(string fileName)
        {
            return fileName.Contains("Assets") && !fileName.Contains("Output");
        };

        //整理所有需要计算的文件
        List<string> ltFiles = new List<string>();
        if(files != null)
        {
            foreach (string file in files)
            {
                ltFiles.Add(file);
                if (IsAssetFile(file))//有可能会有依赖项信息文件，所以只有Asset下的文件才需要meta文件
                {
                    ltFiles.Add(file + ".meta");
                }
            }
        }

        if(paths != null)
        {
            foreach (string path in paths)
            {
                string[] assets = GetAllAssetInPath(path);
                foreach (string file in assets)
                {
                    ltFiles.Add(file);
                    ltFiles.Add(file + ".meta");
                }
            }
        }
        ltFiles.Sort();

        //计算md5
        MemoryStream iInputStream = new MemoryStream();
        foreach (string input_file in ltFiles)
        {
            string strFullPath = IsAssetFile(input_file) ? strFullPath = AssetPathToFullPath(input_file) : input_file;
            byte[] input_data = File.ReadAllBytes(strFullPath);
            if (input_data != null)
            {
                iInputStream.Write(input_data, 0, input_data.Length);
            }
        }
        iInputStream.Seek(0, SeekOrigin.Begin);

        MD5 md5Hash = MD5.Create();
        string result = HashToString(md5Hash.ComputeHash(iInputStream));
        iInputStream.Close();

        string allFiles = "";
        foreach (string s in ltFiles)
        {
            allFiles += s + "\n";
        }
        Debug.Log("md5 for files:\n" + allFiles + "result: " + result);
        return result;
    }

    static private string HashToString(byte[] hash)
    {
        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < hash.Length; i++)
        {
            sBuilder.Append(hash[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    /// <summary>
    /// 将相对Asset的路径转换为绝对路径
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    static public string AssetPathToFullPath(string assetPath)
    {
        return Application.dataPath + "/../" + assetPath;
    }

    /// <summary>
    /// 将绝对路径转换为相对与Asset的路径
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    static public string FullPathToAssetpath(string fullPath)
    {
		string reg = "";
		if( System.IO.Path.DirectorySeparatorChar == '\\' )
			reg = @"\\(Assets\\.*)";
		else
			reg = @"\/(Assets\/.*)";
        string tmp = BuilderTools.CaptureWord(fullPath, reg);
        return tmp.Replace("\\", "/");
    }

    /// <summary>
    /// 获取指定包的输出路径
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    static public string GetOutputPath(BuildTarget buildTarget)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            return Application.dataPath + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "OutputIos" + System.IO.Path.DirectorySeparatorChar;
        }
        else if (buildTarget == BuildTarget.Android)
        {
            return Application.dataPath + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "OutputAndroid" + System.IO.Path.DirectorySeparatorChar;
        }
        else
        {
            return Application.dataPath + System.IO.Path.DirectorySeparatorChar + ".." + System.IO.Path.DirectorySeparatorChar + "Output" + System.IO.Path.DirectorySeparatorChar;
        }
    }

    public class SAssetMemoryInfo
    {
        public string assetPath = "";
        public UnityEngine.Object assetObject = null;
        public int memorySize = 0;

        public static int CompareSizeDown(SAssetMemoryInfo x, SAssetMemoryInfo y) { return y.memorySize - x.memorySize; }
    }
    /// <summary>
    /// 获取指定资源自己以及其所有依赖项所占有的资源内存量
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    static public int GetTotalDependAssetMemory(string assetPath, List<SAssetMemoryInfo> ltMemoryInfo = null)
    {
        string[] arrDepAssetPath = AssetDatabase.GetDependencies(new string[] { assetPath });
        int nTotal = 0;
        foreach (string depAssetPath in arrDepAssetPath)
        {
            if (!depAssetPath.Contains(".unity"))
            {
                UnityEngine.Object[] arrObj = AssetDatabase.LoadAllAssetsAtPath(depAssetPath);
                if (arrObj == null)
                    continue;
                foreach (UnityEngine.Object assetObj in arrObj)
                {
                    if (assetObj == null)
                        continue;

                    System.Type assetType = assetObj.GetType();
                    if (typeof(Mesh).IsAssignableFrom(assetType)
                        || typeof(Texture).IsAssignableFrom(assetType)
                        || typeof(AudioClip).IsAssignableFrom(assetType)
                        || typeof(Animation).IsAssignableFrom(assetType)
                        || typeof(AnimationClip).IsAssignableFrom(assetType)
                        || typeof(Material).IsAssignableFrom(assetType))
                    {
                        if (assetObj.name.Contains("__preview")
                            || assetObj.name == "Take 001")
                            continue;

                        if (assetObj.name == "" && typeof(AnimationClip).IsAssignableFrom(assetType))
                            continue;// 忽略一个空名字的动画 [2/18/2014 yao]

                        int nMemory = Profiler.GetRuntimeMemorySize(assetObj);
                        nTotal += nMemory;
                        //Debug.Log("asset\t" + assetObj.ToString() + "\tsize\t" + nMemory.ToString());
                        if (ltMemoryInfo != null)
                        {
                            SAssetMemoryInfo info = new SAssetMemoryInfo();
                            info.assetPath = depAssetPath;
                            info.assetObject = assetObj;
                            info.memorySize = nMemory;
                            ltMemoryInfo.Add(info);
                        }
                    }
                }
            }
        }
        return nTotal;
    }

    /// <summary>
    /// 获得一个Transform的顶级父物体
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    static public Transform GetParentRoot(Transform transform)
    {
        Transform root = transform;
        while (root.parent != null)
        {
            root = root.parent;
        }
        return root;
    }

    /// <summary>
    /// 判断一个资源是不是“很大”的资源，包含模型、材质、贴图、声音、动画
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns></returns>
    static public bool IsLargeAsset(string assetPath)
    {
        if (assetPath.ToLower().Contains(".fbx"))
            return true;

        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (obj.GetType() == typeof(Texture2D))
            return true;

        if (typeof(Texture2D).IsAssignableFrom(obj.GetType()))
            return true;

        if (typeof(AudioClip).IsAssignableFrom(obj.GetType()))
            return true;

        if (typeof(Animation).IsAssignableFrom(obj.GetType()))
            return true;

        if (typeof(AnimationClip).IsAssignableFrom(obj.GetType()))
            return true;

        if (typeof(Material).IsAssignableFrom(obj.GetType()))
            return true;

        return false;
    }
}