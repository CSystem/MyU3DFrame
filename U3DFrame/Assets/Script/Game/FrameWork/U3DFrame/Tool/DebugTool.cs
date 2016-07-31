using System;
using UnityEngine;

namespace U3DFrame.Tool
{
    /// <summary>
    /// 调试模式
    /// </summary>
    public enum DebugMode
    {
        RELEASE = 0,	//不输出
        DEBUG,			//输出全部
        IGNORE			//忽略正常log
    }

    /// <summary>
    /// 调试工具类
    /// </summary>
    public static class DebugTool
    {
        // public static YxClient client;
        /// <summary>
        /// 保存当前调试模式
        /// </summary>
        private static DebugMode m_mode = DebugMode.DEBUG;
        private static bool isDebug = true;
        public static bool isDebugBuild = isDebug;

        /// <summary>
        /// 调试工具初始化
        /// </summary>
        public static void Init()
        {
            m_mode = (DebugTool.isDebugBuild) ? DebugMode.DEBUG : DebugMode.RELEASE;
            Log("调试工具初始化 模式 ", m_mode);
            isDebug = m_mode == DebugMode.RELEASE;
        }

        public static bool IsDebug
        {
            get { return isDebug; }
        }

        /// <summary>
        /// 暂停编辑器
        /// </summary>
        public static void Break()
        {
            if (m_mode == DebugMode.RELEASE)
                return;

            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.Break();
            }
        }

        #region LOG
        /// <summary>
        /// 普通日志
        /// </summary>
        public static void Log(params object[] paramVals)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.Log(CommonUtils.BuildString(paramVals));
            //if (Application.platform == RuntimePlatform.WindowsEditor
            //    || Application.platform == RuntimePlatform.OSXEditor)
            //{
            //    Debug.Log(CommonUtils.BuildString(paramVals));
            //}
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        public static void LogWarning(params object[] paramVals)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.LogWarning(CommonUtils.BuildString(paramVals));

            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.LogWarning(CommonUtils.BuildString(paramVals));
            }
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        public static void LogException(Exception e)
        {
            if (m_mode == DebugMode.RELEASE)
                return;

            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.LogException(e);
            }
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        public static void LogError(params object[] paramVals)
        {
            if (m_mode == DebugMode.RELEASE && !DebugTool.isDebugBuild)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.LogError(CommonUtils.BuildString(paramVals));
            }
        }

        /// <summary>
        /// 添加日志信息的开始
        /// </summary>
        public static void LogMsgStart(string title)
        {
            Log("=====================" + title + " start =====================");
        }

        /// <summary>
        /// 添加日志信息的结束
        /// </summary>
        public static void LogMsgEnd(string title)
        {
            Log("=====================" + title + " end =====================");
        }
        #endregion

        #region DRAW
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawLine(start, end);
        }
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawLine(start, end, color);
        }
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawLine(start, end, color, duration);
        }
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawLine(start, end, color, duration, depthTest);
        }
        public static void DrawRay(Vector3 start, Vector3 dir)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawRay(start, dir);
        }
        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawRay(start, dir, color);
        }
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawRay(start, dir, color, duration);
        }
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
        {
            if (m_mode == DebugMode.RELEASE)
                return;
            Debug.DrawRay(start, dir, color, duration, depthTest);
        }
        #endregion
    }
}