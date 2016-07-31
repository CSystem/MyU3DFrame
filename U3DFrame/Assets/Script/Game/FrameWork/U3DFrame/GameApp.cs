using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame;
using U3DFrame.Base;
using U3DFrame.UI.Manager;
using UnityEngine;

public class GameApp : GameBase
{
    private static GameApp _instance;

    public static GameApp Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameApp();
            }
            return _instance;
        }
    }
    public override void InitFramework()
    {
//         this.AddManager<GameManager>(typeof(GameManager).ToString());
//         this.AddManager<AudioManager>(typeof(AudioManager).ToString());
         this.AddManager<CUilManager>(typeof(CUilManager).ToString());
         this.AddManager<GuiPanelManager>(typeof(GuiPanelManager).ToString());
//         this.AddManager<ResourceManager>(typeof(ResourceManager).ToString());
//         this.AddManager<ThreadManager>(typeof(ThreadManager).ToString());
//         this.AddManager<TimerManager>(typeof(TimerManager).ToString());
        base.InitFramework();
        UFile.Instance.InitUFile();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
    }
}

