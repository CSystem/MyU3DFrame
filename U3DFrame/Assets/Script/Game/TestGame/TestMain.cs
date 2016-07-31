using UnityEngine;
using System.Collections;
using U3DFrame.UI.Manager;

public class TestMain : MonoBehaviour
{
    TestController testCtrl;
    public void OnClickBtn()
    {
        testCtrl = GameApp.Instance.GetManager<CUilManager>(typeof(CUilManager).ToString()).GetCtrl<TestController>();
        testCtrl.Open();
    }
}
