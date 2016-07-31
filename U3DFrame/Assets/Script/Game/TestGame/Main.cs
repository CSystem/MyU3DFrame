using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //GameApp.Instance.InitFramework();
        UFile.Instance.InitUFile();
        ResourcePrepare.Instance.TestDown();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
