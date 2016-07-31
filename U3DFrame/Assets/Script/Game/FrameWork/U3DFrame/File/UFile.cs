using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using U3DFrame.Json;
using U3DFrame.Tool;

public class UFileInfo
{
    public string name;
    public string hash;
    public string path;
    public int size;
    public string[] dependencies;
}

public class UFile
{
    private Dictionary<string, UFileInfo> _uiFileInfoDict = new Dictionary<string, UFileInfo>();
    private Dictionary<string, UFileInfo> _xmlFileInfoDict = new Dictionary<string, UFileInfo>();
    private Dictionary<string, UFileInfo> _artFileInfoDict = new Dictionary<string, UFileInfo>();

    private static UFile _instance;
    public static UFile Instance
    {
        get
        {
            if (null == _instance)
                _instance = new UFile();
            return _instance;
        }   
    }

    public void InitUFile()
    {
        string path = GameConst.ResUiPath + "index.info";
        if(File.Exists(path))
        {
            string str = File.ReadAllText(path);
            DebugTool.Log(str);
            Hashtable table = MiniJSON.JsonDecode(str) as Hashtable;
            ParseIndexInfoTable(table);
        }

        string xmlPath = GameConst.ResXmlPath + "index.info";
        if (File.Exists(xmlPath))
        {
            string str = File.ReadAllText(xmlPath);
            DebugTool.Log(str);
            Hashtable table = MiniJSON.JsonDecode(str) as Hashtable;
            ParseIndexInfoTable(table);
        }

        string artPath = GameConst.ResArtPath + "index.info";
        if (File.Exists(artPath))
        {
            string str = File.ReadAllText(artPath);
            DebugTool.Log(str);
            Hashtable table = MiniJSON.JsonDecode(str) as Hashtable;
            ParseIndexInfoTable(table);
        }
    }

    public UFileInfo GetFileInfo(string fileName)
    {
        UFileInfo fileInfo = null;

        fileInfo = GetUiFileInfo(fileName);
        if (null != fileInfo)
            return fileInfo;
        fileInfo = GetXmlFileInfo(fileName);
        if (null != fileInfo)
            return fileInfo;
        fileInfo = GetArtFileInfo(fileName);
        if (null != fileInfo)
            return fileInfo;

        return null;
    }

    public UFileInfo GetUiFileInfo(string fileName)
    {
        if (_uiFileInfoDict.ContainsKey(fileName))
            return _uiFileInfoDict[fileName];
        return null;
    }

    public UFileInfo GetXmlFileInfo(string fileName)
    {
        if (_xmlFileInfoDict.ContainsKey(fileName))
            return _xmlFileInfoDict[fileName];
        return null;
    }

    public UFileInfo GetArtFileInfo(string fileName)
    {
        if (_artFileInfoDict.ContainsKey(fileName))
            return _artFileInfoDict[fileName];
        return null;
    }

    private void ParseIndexInfoTable(Hashtable table)
    {
        if (null == table)
            return;
        foreach(string key in table.Keys)
        {
            Hashtable singleTable = table[key] as Hashtable;
            UFileInfo fileInfo = ParseSingleTable(singleTable);
            AddFileInfo(fileInfo);
        }
    }

    private void AddFileInfo(UFileInfo fileInfo)
    {
        if (null == fileInfo)
            return;
        if(fileInfo.path.Contains("uiresource"))
        {
            _uiFileInfoDict[fileInfo.name] = fileInfo;
        }

        if (fileInfo.path.Contains("xmlresource"))
        {
            _xmlFileInfoDict[fileInfo.name] = fileInfo;
        }

        if (fileInfo.path.Contains("artresource"))
        {
            _artFileInfoDict[fileInfo.name] = fileInfo;
        }
    }

    private UFileInfo ParseSingleTable(Hashtable table)
    {
        if (null == table)
            return null;
        UFileInfo fileInfo = new UFileInfo();
        foreach(string key in table.Keys)
        {
            switch(key)
            {
                case "name":
                    fileInfo.name = table[key] as string;
                    break;
                case "path":
                    fileInfo.path = table[key] as string;
                    break;
                case "hash":
                    fileInfo.hash = table[key] as string;
                    break;
                case "dependencies":
                    fileInfo.dependencies = ParseDependencies(table[key] as Hashtable);
                    break;
                case "size":
                    
                    break;
            }
        }
        return fileInfo;
    }

    private string[] ParseDependencies(Hashtable table)
    {
        if (null == table)
            return new string[0];
        int count = table.Count;
        string[] deps = new string[count];
        int i = 0;
        foreach(string val in table.Values)
        {
            deps[i] = val;
            i++;
        }
        return deps;
    }
}

