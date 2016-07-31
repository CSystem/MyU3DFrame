
namespace U3DFrame.Const
{
    /// <summary>
    /// 通信协议常量
    /// </summary>
    public class BaseConst
    {
        public const string PLAT = "plat";
        public const string PLAT_ALL = "0";
        public const string PLAT_AND = "100";
        public const string PLAT_AND_CN = "101";
        public const string PLAT_AND_TW = "102";
        public const string PLAT_AND_JP = "103";
        public const string PLAT_AND_KR = "104";
        public const string PLAT_AND_US = "105";
        public const string PLAT_AND_DE = "106";
        public const string PLAT_IPHONE = "200";
        public const string PLAT_IPHONE_CN = "201";
        public const string PLAT_IPHONE_TW = "202";
        public const string PLAT_IPHONE_JP = "203";
        public const string PLAT_IPHONE_KR = "204";
        public const string PLAT_IPHONE_US = "205";
        public const string PLAT_IPHONE_DE = "206";
        public const string PLAT_WEB = "300";
        public const string PLAT_WEB_CN = "301";
        public const string PLAT_WEB_TW = "302";
        public const string PLAT_WEB_JP = "303";
        public const string PLAT_WEB_KR = "304";
        public const string PLAT_WEB_US = "305";
        public const string PLAT_WEB_DE = "306";
        public const string PLAT_EDITOR = "400";
        public const string PLAT_GG = "500";
        public const string PLAT_GG_CN = "501";
        public const string PLAT_GG_TW = "502";
        public const string PLAT_GG_JP = "503";
        public const string PLAT_GG_KR = "504";
        public const string PLAT_GG_US = "505";
        public const string PLAT_GG_DE = "506";

        public const string PLAT_FB = "600";
        public const string PLAT_FB_WEB_CN = "601";
        public const string PLAT_FB_WEB_TW = "602";
        public const string PLAT_FB_WEB_JP = "603";
        public const string PLAT_FB_WEB_KR = "604";
        public const string PLAT_FB_WEB_US = "605";
        public const string PLAT_FB_WEB_DE = "606";
        public const string PLAT_FB_ANR_TW = "611";
        /// <summary>
        /// 平安网页平台1000
        /// </summary>
        public const string PLAT_WEB_PINGAN = "1000";
        public const string PLAT_AND_PINGAN = "1001";
        /// <summary>
        /// qq平台
        /// </summary>
        public const string PLAT_WEB_QQ = "2000";
        /// <summary>
        /// oppo
        /// </summary>
        public const string PLAT_OPPO = "2100";

        public const string PLAT_AND_360 = "700";
        public const string PLAT_AND_MIUI = "800";
        public const string PLAT_AND_HUAWEI = "900";
        /// <summary>
        /// 央广视讯
        /// </summary>
        public const string PLAT_YGSX = "2200";
        /// <summary>
        /// 百度
        /// </summary>
        public const string PLAT_BAIDU = "2300";
        public const string MSG = "msg";
        public const string CMD = "cmd";
        public const string REQUEST_ID = "req_id";
        public const string HTTP_METHOD = "http_method";
        public const string HTTP_POST = "1";
        public const string HTTP_GET = "2";

        public const string HTTP_URL = "url";
        public const string VER = "ver";

        public const string RET_VAL = "ret_val";
        /// <summary>
        /// 服务器返回成功
        /// </summary>
        public const string RET_VAL_SUCC = "0";
        /// <summary>
        /// 服务器返回失败
        /// </summary>
        public const string RET_VAL_FAIL = "1";
        /// <summary>
        /// 服务器超时，走本地逻辑（是否重发消息，读取本地数据，还是丢弃消息）
        /// </summary>
        public const string RET_VAL_NATIVE = "2";

        public const string SOCKET_KEY = "socket_key";
        public const string SOCKET_USER_CMD = "user_cmd";

        //public const string CLIENT_NEW = "cnew";

		public const string ITEM_ID = "item_id";

        public const string USER_ID = "user_id";
        public const string NICK_NAME = "nick_name";
        public const string T_USER_ID = "t_user_id";
        public const string T_NICK_NAME = "t_nick_name";
        public const string RET_VAL_DEF = "-1";
        public const string ERR_404 = "404";
        public const string ERR_502 = "502";
        public const string ERR_503 = "503";
        public const string ERR_504 = "504";
        public const string TRY_CNT = "try_cnt";
        // public const string TRY_TOTAL_CNT = "try_total_cnt";
        public const string RET_VAL_NONE = "99";
        public const string RET_VAL_LOGOUT = "98";
        public const string RET_VAL_SYS_ERRO = "10005";
        /// <summary>
        /// 在其他设备登录，被踢
        /// </summary>
        public const string RET_VAL_97 = "97";
        public const string RET_VAL_STOP_SERVER = "10000";
        public const string STOP_SERVER_MSG = "stop_server_msg";

        public const string MSG_SERVER = "msg_server";

        public const string MIEI = "miei";
        public const string PLAT_MIEI = "plat_miei";
        /// <summary>
        /// 平台token
        /// </summary>
        public const string PLAT_TOKEN = "plat_token";
        public const string AUTHOR_CODE = "author_code";
        //游戏类型
        public const string GAME_ID = "game_id";
        //其他数据
        // public const string DATA_JSON = "data_json";


        public const string RET_TYPE = "ret_type";
        public const string ERR_CODE = "err_code";
        public const string NOW_SERVER_TIME = "now_server_time";

        /**
         * 没有足够的RMB
         */
        public const string RET_TYPE_NO_RMB_MONEY = "10003";
        /**
         * 没有足够的游戏币
         */
        public const string RET_TYPE_NO_GAME_MONEY = "10004";

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public const string LAST_UPD_TIME = "last_upd_time";
        // 回调函数
        public const string CALL_BACK_FUC = "call_back_fuc";
        // 回调参数
        public const string CALL_BACK_PARAM = "call_bcak_param";
        // 回调对象
        public const string CALL_BACK_TARGET = "call_back_target";
        // 回调对象的类型
        public const string CALL_BACK_TYPE = "call_back_type";
        // 本地保存命令数据的key
        public const string LOCAL_CMD_DATA_KEY = "local_cmd_data_key";
        // 发送数据的key
        public const string SEND_DATA_KEY = "send_data_key";
        // 接受数据的key
        public const string REC_DATA_KEY = "rec_data_key";
        // 日期
        public const string DATA_TIME = "data_time";

        public const string GMT_CREATE = "gmt_create";
        public const string GMT_MODIFIED = "gmt_modified";
        public const string UUID = "uuid";
        public const string ID = "id";


        public const string CLIENT_FILE_VER = "client_file_ver";
        public const string CLIENT_FILE_VER_VAL = "1";

        //public const string EXECUTE_PARAM = "execute_param";
        /// <summary>
        /// service回调函数
        /// </summary>
        //public const string NET_RESPONSE_CALLBACK = "net_response_callback";
        //public const string SERVICE_CALLBACK_DO_SUCCESS = "service_callback_do_success";
        //public const string SERVICE_CALLBACK_DO_FAIL = "service_callback_do_fail";
        //public const string NET_PARAM = "net_param";
        //public const string SERVICE_PARAM_DO_SUCCESS = "service_param_do_success";
        //public const string SERVICE_PARAM_DO_FAIL = "service_param_do_fail";
        public const string EVENT_ID = "event_id";
        /// <summary>
        /// 布尔值的标识
        /// </summary>
        public const string BOOL_TRUE = "true";
        public const string BOOL_FALSE = "false";
        public const string TIME_STAMP = "time_stamp";
        /// <summary>
        /// 机器设备信息
        /// </summary>
        public const string SYSTEM_INFO = "system_info";


        public const int DFT_OFFICIAL_USER_ID = 10000;
        public const string DFT_OFFICIAL_USER_ID_STR = "10000";
        //上次向服务器索取数据的时间		
        public const string LAST_RQ_TIME = "last_rq_time";

        public const string WEATHER = "weather";
        public const int WEATHER_NONE = 0;
        public const int WEATHER_SNOWING = 1;
        public const int WEATHER_RAINING = 2;
        public const int WEATHER_CLOUDY = 3;
        
        /**
	     * DB错误
	     */
	    public const string  RET_TYPE_DB_ERR = "10000";
	    /**
	     * memcache错误
	     */
	    public const string  RET_TYPE_MEM_ERR = "10001";
	    /**
	     * 没有UserID
	     */
	    public const string  RET_TYPE_NO_USER_ID = "10002";
	    /**
	     * 没有足够能量
	     */
	    public const string  RET_TYPE_NO_POWER = "10005";
	    /**
	     * 没有记录
	     */
	    public const string  RET_TYPE_NO_RECODE = "10006";
	    /**
	     * 敏感词
	     */
        public const string RET_TYPE_HAS_DIRTY_WORD = "10007";
        /// <summary>
        /// 级别不够
        /// </summary>
	    public const int RET_TYPE_NOT_REACH_LEVEL = 10008;

        public const int RET_TYPE_NO_ENOUGH_CELL = 10010;

    }
}
