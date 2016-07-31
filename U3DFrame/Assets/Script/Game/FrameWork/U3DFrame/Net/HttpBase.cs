using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Threading;
using Object = System.Object;
using UnityEngine;
/*public delegate void ResponseExecute(ResponseData respData);*/

public class HttpBase
{
//     public CookieContainer cc = new CookieContainer();
//     public Dictionary<string, string> wwwCookie = new Dictionary<string, string>();
//     public static NetSettings netSettings;
//     private HttpWebRequest httpRequest = null;
//     public float upPercent = 0f;
//     public bool isDown = false;
//     public bool isUp = false;
//     private string encode = "UTF-8";
//     private string boundary = "";
//     private byte[] boundarybytes = null;
//     private string strUserAgent = "YouXinPlayer";
// 
//     public string browser = "";
//     private int timeoutTime = 50000;
// 
//     public int TimeoutTime
//     {
//         get { return timeoutTime; }
//         set { timeoutTime = value; }
//     }
// 
//     public void LogoutGC()
//     {
//         responseList.Clear();
//         wwwCookie.Clear();
//         cc = new CookieContainer();
//         cookieVals.Clear();
//     }
// 
//     private Hashtable cookieVals = new Hashtable();
//     public string GetCookieVal(string key)
//     {
//         if (cookieVals.ContainsKey(key))
//         {
//             return CommonUtils.GetVal<string>(cookieVals, key);
//         }
//         return "";
//     }
//     public void SetCookieVal(string key, string val)
//     {
//         if (!CommonUtils.IsEmpty(val))
//         {
//             val = CommonUtils.URLEncode(val);
//             val = val.Replace(",", ";");
//         }
//         cookieVals[key] = val;
//         wwwCookie["Cookie"] = "";
//         foreach (string cKey in cookieVals.Keys)
//         {
//             wwwCookie["Cookie"] += cKey + "=" + cookieVals[cKey] + ";";
//         }
//         DebugTool.Log("[HTTPBase.SetCookieVal] wwwCookie = ", wwwCookie["Cookie"]);
//     }
//     public bool HasWWWCookie()
//     {
//         return wwwCookie.ContainsKey("Cookie");
//     }
//     private void SetCookie(CookieCollection c)
//     {
//         cc.Add(c);
//     }
// 
//     private HttpWebResponse response;
//     private NetworkManager m_NetMgr;
// 
//     public HttpBase(NetworkManager netMgr)
//     {
//         boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
//         boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
//         m_NetMgr = netMgr;
//     }
// 
//     public void PostDataThread(string url, JSONObject json, ResponseExecute responseExecute)
//     {
//         m_NetMgr.StartCoroutine(PostDataThreadWWW(url, json, responseExecute));                 
//     }
// 
//     private float nowTime = Time.realtimeSinceStartup;
//     public void FixedUpdate()
//     {
//         if (Time.realtimeSinceStartup - nowTime < 3)
//         {
//             return;
//         }
//         this.nowTime = Time.realtimeSinceStartup;
//         CheckTimeOut();
//     }
//     public List<ResponseData> responseList = new List<ResponseData>();
//     public void CheckTimeOut()
//     {
//         try
//         {
//             if (responseList.Count <= 0)
//             {
//                 return;
//             }
//             for (int i = responseList.Count - 1; i >= 0; i--)
//             {
//                 ResponseData respData = responseList[i];
//                 if (respData == null)
//                 {
//                     responseList.RemoveAt(i);
//                     continue;
//                 }
//                 if (respData.www.isDone)
//                 {
//                     responseList.RemoveAt(i);
//                     respData.respFunc(respData);
//                     continue;
//                 }
//                 if (respData.www.progress > 0)
//                 {
//                     DebugTool.Log("[HTTPBase.CheckTimeOut] www down data began.");
//                     continue;
//                 }
//                 if (netSettings != NetSettings.NOREACHABLE && CommonUtils.IsEmpty(respData.www.error) && DateTime.Now.Subtract(respData.reuqestTime).TotalSeconds < 30)
//                 {
//                     continue;
//                 }
//                 respData.respError = "request time out or request error.";
//                 responseList.RemoveAt(i);
//                 respData.respFunc(respData);
//             }
//         }
//         catch (Exception ex)
//         {
//             DebugTool.LogWarning("[HTTPBase.CheckTimeOut] Exception : ", ex.Message);
//         }
//     }
// 
//     public IEnumerator PostDataThreadWWW(string url, JSONObject json, ResponseExecute responseExecute)
//     {
//         WWW www = new WWW(url, json.WForm.data, wwwCookie);
//         ResponseData respData = new ResponseData();
//         respData.gzip = true;
//         respData.www = www;
//         respData.sendJson = json;
//         respData.respFunc = responseExecute;
//         respData.reuqestTime = DateTime.Now;
//         responseList.Add(respData);
//         yield return www;
//         if (responseExecute != null)
//         {
//             if (responseList.Contains(respData))
//             {
//                 responseList.Remove(respData);
//                 respData.www = www;
//                 respData.sendJson = json;
//                 responseExecute(respData);
//             }
//         }
//     }
}

