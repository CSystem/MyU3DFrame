using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.UI.Controller;

public class TestController : CUiBase
{
    TestDesigner testDesigner;
    protected override void OnDesignerLoadFinish(bool bSuccess)
    {
        
    }

    public override E_UIWINDOW_TYPE UIWindowType()
    {
        return E_UIWINDOW_TYPE.E_UIWINDOW_TEST;
    }

    public override void Open()
    {
        if (Showing())
            Close();
        base.Open();
        testDesigner = InitDesigner<TestDesigner>();
        testDesigner.Open();
    }

    public void Command_OnClickCloseBtn(UnityEngine.GameObject go)
    {
        Close();
    }
}

