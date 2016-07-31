using System.Net;
using UnityEngine;
using U3DFrame.Tool;

namespace Com.Youxin.Common.Data
{
    public class ResponseData
    {
        public U3DFrame.Json.JSONObject sendJson;
        public string resp;
        public HttpStatusCode status;
        public bool gzip = false;
        public WWW www;
        public int sendCnt = 0;
        private string _wwwText;
        public string wwwText
        {
            get
            {
                if (string.IsNullOrEmpty(_wwwText))
                {
                    if (gzip)
                    {
                        _wwwText = CommonUtils.GzipUncompressStr(www);
                    }
                    else
                    {
                        _wwwText = www.text;
                    }
                }
                return _wwwText;
            }
        }
//         public ResponseExecute respFunc;
//         public DateTime reuqestTime;
//         public string respError;
//         
//         public void GC()
//         {
//             this.GC(null);
//         }
//         public void GC(object param)
//         {
//             sendJson = null;//.GC();
//             UnityTools.Destroy(www);
//             respFunc = null;
//         }
    }
}
