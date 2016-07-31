using System;
using U3DFrame.Const;
using U3DFrame.Tool;

namespace Com.Youxin.Common.Data
{
    /// <summary>
    /// 数据基类
    /// </summary>
    public abstract class BaseData
    {
        public static string nowServerTime = DateTime.Now.ToString();
        public static DateTime preServerDateTime = DateTime.Now;
        public static DateTime nowServerDateTime = DateTime.Now;
        public static DateTime startServerDateTime = DateTime.Now;
        public static DateTime preChkDayServerDateTime = DateTime.Now;
        public static DateTime nowClientDateTime = DateTime.Now;
        public static DateTime gameStartDateTime = DateTime.Now;
        public static bool initDateTime = false;
        public static string curRootVer = "";
        public static string clientRwPathVer = "20150403";
        //public static LocationInfo locationInfo;
        public static string clientNativeVer = "1";
        public static string clientNativeGGVer = "1";
        public static bool isFirstPayDouble = false;
        public static bool isBoneNDay = false;
        public static bool isStar5 = false;
        public static int weather = BaseConst.WEATHER_NONE;
        public static string actSpringStartTime = "";
        public static string actSpringEndTime = "";
        public static string imgFolder = "img/";
        public static string imgFamilyFolder = "family/";
        public static string fileFolder = "";
        public static bool isActSpring
        {
            get
            {
                return BaseData.nowServerDateTime >= CommonUtils.DateParse(BaseData.actSpringStartTime) && BaseData.nowServerDateTime <= CommonUtils.DateParse(BaseData.actSpringEndTime);
            }
        }

        public virtual void Init(string key)
        {
        }
    }
}
