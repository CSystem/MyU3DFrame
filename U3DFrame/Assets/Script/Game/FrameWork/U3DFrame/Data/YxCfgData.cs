
using System.Collections.Generic;


namespace U3DFrame.Data
{
    public class YxCfgData
    {
        private Dictionary<string, string> cfgDict = new Dictionary<string, string>();
        public void AddCfg(string key, string val)
        {
            this.cfgDict[key] = val;
        }
        public string GetCfgVal(string key)
        {
            if (!this.cfgDict.ContainsKey(key))
            {
                return null;
            }
            return this.cfgDict[key];
        }
// #region 服务器地址
//         /// <summary>
//         /// 握手服务器地址
//         /// </summary>
//         public string handShakeServer;
// 		/// <summary>
// 		/// HTTP服务器地址
// 		/// </summary>
// 		public string loginServer;
//         /// <summary>
//         /// HTTP服务器地址
//         /// </summary>
//         public string httpServer;
//         /// <summary>
//         /// 下载服务器地址
//         /// </summary>
//         public string downServer;
//         /// <summary>
//         /// CDN服务器地址
//         /// </summary>
//         public string cdnServer;
//         /// <summary>
//         /// 支付服务器地址
//         /// </summary>
//         public string payServer;
//         /// <summary>
//         /// 找回密码地址
//         /// </summary>
//         public string findPwdServer;
//         /// <summary>
//         /// JAVA服务器IP
//         /// </summary>
//         public string socketIP;
//         /// <summary>
//         /// JAVA服务器端口
//         /// </summary>
//         public int socketPort;
// #endregion
// #region 项目配置
//         public string cashTag = "";
//         public string cashTagIPhone = "";
//         /// <summary>
//         /// 本地保存文目录
//         /// </summary>
//         public string nativeSaveFolder;
//         /// <summary>
//         /// 版本语言
//         /// </summary>
//         public string language;
// 		/// <summary>
// 		/// 语言种类
// 		/// </summary>
// 		public Lang lang = Lang.CN;
//         /// <summary>
//         /// 设备系统语言
//         /// </summary>
//         public SystemLanguage systemLanguage = SystemLanguage.Chinese;
//         /// <summary>
//         /// 游戏ID
//         /// </summary>
//         public int gameId;
//         /// <summary>
//         /// GPRS代理
//         /// </summary>
//         public string gprsProxy;
//         /// <summary>
//         /// 内部版本号
//         /// </summary>
//         public int intVer;
//         /// <summary>
//         /// 内部版本号
//         /// </summary>
//         public string strVer;
// 		/// <summary>
// 		/// 骨头分类
// 		/// </summary>
//         public int intBoneSub;
//         public string productId;
//         public string productIdIPhone;
//         /// <summary>
//         /// 平台code
//         /// </summary>
//         public string strPlatCode;
//         public string strPlatCodeIPhone;
//         /// <summary>
//         /// 如果服务器网络不通，在serverNetChkInterval时间内不再发送消息到服务器，会直接走客户端本地缓存
//         /// 如果客户端网络是通畅的，那么serverNetChkInterval之后，会继续向服务器发送消息，以检测服务器是否通畅
//         /// </summary>
//         public int serverNetChkInterval = 60;
// #endregion
// #region 其他
//         /// <summary>
//         /// 关于我们
//         /// </summary>
//         public string aboutURL;
//         /// <summary>
//         /// FAQ地址
//         /// </summary>
//         public string faqURL;
//         /// <summary>
//         /// 检测IP归属地
//         /// </summary>
//         public string ipURL;
//         /// <summary>
//         /// 商店应用地址
//         /// </summary>
//         public string marketURL;
//         public string marketURLIPhone;
//         /// <summary>
//         /// 天气获取地址
//         /// </summary>
//         public string weatherServer;
//         /// <summary>
//         /// 全局的GameObject名称，用于平台回调
//         /// </summary>
//         public string globalGameObjectName;
//         /// <summary>
//         /// 全局的平台回调函数，JAVA，ios，java script回调
//         /// </summary>
//         public string globalNativeCallbackName;
//         public Transform global;
// #endregion
    }
}
