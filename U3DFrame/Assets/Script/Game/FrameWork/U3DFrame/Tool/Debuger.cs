using UnityEngine;
using System;
using System.IO;

namespace U3DFrame.Tool
{
    public static class Debuger
    {
        private static StreamWriter sw;
        public static void WriteLog(string direct, LogType type, object info)
        {
#if UNITY_WEBPLAYER
            return;
#else
            string currStr = info.ToString();
            string msg = CommonUtils.Datetime();

            switch (type)
            {
                case LogType.Warning:
                    msg += ": [Warning]";
                    break;
                case LogType.Error:
                    msg += ": [Error]";
                    break;
            }

            InitSw(direct);
            sw.WriteLine(msg + ": " + currStr);
            sw.Flush();
#endif
        }

        private static void InitSw(string direct)
        {
#if UNITY_WEBPLAYER
        return;
#else
            if (sw != null)
            {
                return;
            }

            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            FileInfo fi = new FileInfo(direct + "Game.yxlog");

            if (fi.Exists && CommonUtils.FileLen(fi) < 1024 * 1024)
            {
                sw = fi.AppendText();
            }
            else
            {
                sw = fi.CreateText();
            }
#endif
        }

        public static void Exit()
        {
#if UNITY_WEBPLAYER
        return;
#else
            if (sw == null)
            {
                return;
            }
            sw.Close();
            sw = null;
#endif
        }
    }
}
