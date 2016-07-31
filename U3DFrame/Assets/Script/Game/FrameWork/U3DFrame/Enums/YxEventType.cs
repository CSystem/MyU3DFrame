using System;
using System.Collections.Generic;

namespace U3DFrame.Enum
{
	public enum YxEventType
	{
        /// <summary>
        /// 停服维护
        /// </summary>
        STOP_SERVER,
        HTTP_RESPONSE,
        /// <summary>
        /// 握手消息
        /// </summary>
        HAND_SHAKE,
        HAND_SHAKE_NATIVE,
        HAND_SHAKE_FAIL,
        /// <summary>
        /// 提示版本更新
        /// </summary>
        VER_UPDATE,
        /// <summary>
        /// 必须更新新版本
        /// </summary>
        VER_UPDATE_MUST,
        /// <summary>
        /// 加载ROOT文件
        /// </summary>
        LOADED_ROOT_ALL,
        LOADED_ROOT_ONE_RES,
        LOADED_UPD_RES,
        /// <summary>
        /// 登出
        /// </summary>
        LOGOUT,
        /// <summary>
        /// 登陆成功
        /// </summary>
        LOGIN_SUCCESS,
        /// <summary>
        /// 切换区
        /// </summary>
        CHANGE_SERVER,
        /// <summary>
        /// 登陆失败
        /// </summary>
        LOGIN_FAIL,
        /// <summary>
        /// 在别处登录
        /// </summary>
        LOGIN_ANOTHER,
        /// <summary>
        /// 注册成功
        /// </summary>
        REGISTER_SUCCESS,
        /// <summary>
        /// 注册过多
        /// </summary>
        REGISTER_TOO_MORE,
        /// <summary>
        /// 封锁账号
        /// </summary>
        SYSTEM_LOCK,
        /// <summary>
        /// 网络不通
        /// </summary>
        NET_UNABLE,
        /// <summary>
        /// 网络连接超时
        /// </summary>
        NET_TIMEOUT,
        /// <summary>
        /// 网络重新连接
        /// </summary>
        NET_RECONNECT,
        /// <summary>
        /// 其他处理
        /// </summary>
        USER_EXTENSION,
        /// <summary>
        /// 支付成功
        /// </summary>
        PAY_SUCCESS,
        /// <summary>
        /// 客户端调了时间
        /// </summary>
        TIME_WRONG,


        SMART_EVENT,
        /// <summary>
        /// 平台登陆回调
        /// </summary>
        PLAT_LOGIN_CALLLBACK,
        /// <summary>
        /// 平台切换帐号回调
        /// </summary>
        PLAT_SWITCH_ACCOUNT_CALLLBACK,
        WEB_MULT_CLIENT,
	}
}
