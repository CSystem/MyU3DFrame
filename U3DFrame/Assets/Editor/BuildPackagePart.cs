using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

partial class BuildPackages
{
    /// <summary>
    /// version:版本号  
    /// </summary>
    public const string PACKAGE_INFO_EXT = ".txt";
    public const string DATA_EXT = ".bytes";
    public const string BUNDLE_EXT = ".unity3d";
    public const string VERSION_EXT = ".version";

    enum E_RES_TYPE
    {
        E_UiResource = 1,
        E_XmlResource = 2,
        E_ArtResource = 3,
        E_Root = 4,
    }

    private static E_RES_TYPE GetPackageResType(string path)
    {
        path = path.ToLower();

        if (path.Contains("uiresource"))
            return E_RES_TYPE.E_UiResource;
        else if (path.Contains("xmlresource"))
            return E_RES_TYPE.E_XmlResource;
        else if (path.Contains("artresource"))
            return E_RES_TYPE.E_ArtResource;
        else
            return E_RES_TYPE.E_Root;
    }

    private static string GetPackageResPath(string path)
    {
        path = path.ToLower();
        string dataPath = Application.streamingAssetsPath + "/";
        if (path.Contains("uiresource"))
            return dataPath + "uiresource/";
        else if (path.Contains("xmlresource"))
            return dataPath + "xmlresource/";
        else if (path.Contains("artresource"))
            return dataPath + "artresource/";
        else
            return dataPath ;
    }

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
            return delegate (PackageInfo info)
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

}
