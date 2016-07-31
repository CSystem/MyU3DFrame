using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.UI.View;

public class TestDesigner : UiDesignerBase
{
    TestController testCtrl;
    protected override void InitializeForm(bool bSuccess)
    {
        if (!bSuccess)
            return;
        UIEventListener listener = AddEvent("Sprite", E_EventObjType.EventObj_Sprite);
        if(null != listener)
        {
            listener.onClick = OnClickCloseBtn;
        }
    }

    public override void Init(object iUIObj)
    {
        base.Init(iUIObj);
        if (iUIObj is TestController)
            testCtrl = iUIObj as TestController;
    }

    public void Open()
    {
        AddNeedResInfo(E_ResourceType.E_MainForm, "Assets/Art/UiResource/package_ui/test.prefab");
        OpenForm();
    }

    private void OnClickCloseBtn(UnityEngine.GameObject go)
    {
        testCtrl.Command_OnClickCloseBtn(go);
    }
}

