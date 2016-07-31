using UnityEngine;
using System.Collections;
using System.IO;

public class ResourcePrepare : MonoBehaviour
{
    private static ResourcePrepare _instance;
    public static ResourcePrepare Instance
    {
        get
        {
            if (null == _instance)
                _instance = new GameObject("ResourcePrepare").AddComponent<ResourcePrepare>();
            return _instance;
        }
    }


    public void TestDown()
    {
        StartCoroutine(StartDown());
    }

    IEnumerator StartDown()
    {
        string ip = "http://u3dframe-testwww.stor.sinaapp.com/cfg_guide_finger.xml";
        WWW www = new WWW(ip);
        yield return www;
        if(null != www.error)
        {
            Debug.LogError(www.error);
            yield break;
        }
        Debug.Log(www.bytes.Length);
        FileStream  stream = File.Create(GameConst.ResXmlPath + "cfg_guide_finger.xml");
        BinaryWriter writer = new BinaryWriter(stream);
        string str = System.Text.Encoding.UTF8.GetString(www.bytes);
        writer.Write(www.bytes);
        writer.Close();
        stream.Close();

//         string ip = "http://u3dframe-testwww.stor.sinaapp.com/game.ico";
// 
//         WWW www = new WWW(ip);
//         yield return www;
//         if (null != www.error)
//         {
//             Debug.LogError(www.error);
//             yield break;
//         }
//         Debug.Log(www.bytes.Length);
//         FileStream stream = File.Create(GameConst.ResXmlPath + "game.ico");
//         BinaryWriter writer = new BinaryWriter(stream);
//         string str = System.Text.Encoding.UTF8.GetString(www.bytes);
//         writer.Write(www.bytes);
//         writer.Close();
//         stream.Close();
    }
}

