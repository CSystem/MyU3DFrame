using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameConst
{
    public static bool isDebug = false;
    public const string UiResource = "UiResource";
    public const string XmlResource = "XmlResource";
    public const string ArtResource = "ArtResource";

    public static string ResRootPath
    {
        get
        {
            if (isDebug)
            {
                return Application.dataPath + "/Art/";
            }

#if UNITY_EDITOR
            return Application.streamingAssetsPath + "/";
#else
            return Application.persistentDataPath + "/Art/"; 
#endif
        }
    }

    public static string ResUiPath
    {
        get
        {
            return ResRootPath + UiResource + "/";
        }
    }

    public static string ResXmlPath
    {
        get
        {
            return ResRootPath + XmlResource + "/";
        }
    }

    public static string ResArtPath
    {
        get
        {
            return ResRootPath + ArtResource + "/";
        }
    }

}

