using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using U3DFrame.Tool;

namespace U3DFrame.Json
{
    public class JSONObject
    {
        private WWWForm wForm;
        private Hashtable sendData;
        private ArrayList sendList;
        public ArrayList SendList
        {
            get { return sendList; }
            set { sendList = value; }
        }

        private ArrayList recdList;
        public ArrayList RecdList
        {
            get { return recdList; }
            set { recdList = value; }
        }

        private bool isArray;
        public bool IsArray
        {
            get { return isArray; }
        }

        private Hashtable recdData;
        public Hashtable RecdData
        {
            get { return recdData; }
            set { this.recdData = value; }
        }

        public Dictionary<string, string> paramDict;
        public JSONObject()
        {
            Init();
        }
        public JSONObject(bool newWF)
        {
            Init(newWF);
        }

        public object this[string key]
        {
            get { return this.recdData[key]; }
        }
        private void Init()
        {
            Init(true);
        }
        private void Init(bool newWF)
        {
            if (newWF)
            {
                wForm = new WWWForm();
            }
            sendData = new Hashtable();
            paramDict = new Dictionary<string, string>();
            isArray = false;
            recdData = new Hashtable();
        }

        public Hashtable SendData
        {
            get { return sendData; }
            set { this.sendData = value; }
        }

        public void Add(object key, byte[] bytes)
        {
            if (wForm != null)
            {
                wForm.AddBinaryData(key.ToString(), bytes);
            }

            sendData[key] = bytes;
        }

        public void Add(object key, byte[] bytes, string name)
        {
            if (wForm != null)
            {
                wForm.AddBinaryData(key.ToString(), bytes, name);
            }

            sendData[key] = bytes;
        }

        public void Add(object key, byte[] bytes, string name, string mime)
        {
            if (wForm != null)
            {
                wForm.AddBinaryData(key.ToString(), bytes, name, mime);
            }

            sendData[key] = bytes;
        }

        public void Del(object key)
        {
            string sKey = key.ToString();
            if (this.sendData.ContainsKey(sKey))
            {
                sendData.Remove(sKey);
            }
        }

        public void Add(object key, object val)
        {
            string sKey = key.ToString();
            string sVal;

            if (val is bool)
            {
                bool bVal = (bool)val;
                sVal = bVal ? "1" : "0";
            }
            else
            {
                if (val == null)
                {
                    sVal = "";
                }
                else
                {
                    sVal = val.ToString();
                }
            }

            if (wForm != null)
            {
                wForm.AddField(sKey, sVal);
            }

            sendData[key] = val;
            paramDict[sKey] = sVal;
        }

        public void JustAdd(object key, object val)
        {
            sendData[key] = val;
        }
        public void AddRecv(object key, object val)
        {
            recdData[key] = val;
        }

        public T GetSend<T>(string key)
        {
            return this.GetSend<T>(key, default(T));
        }
        public T GetSend<T>(string key, T dftVal)
        {
            return CommonUtils.GetVal<T>(sendData, key, dftVal);
        }

        public bool SendHasKey(object key)
        {
            return sendData.ContainsKey(key);
        }

        public bool SendHasVal(object val)
        {
            return sendData.ContainsValue(val);
        }
        public bool isSocket = false;
        public string recdStr;
        public T GetRecd<T>(string key)
        {
            return CommonUtils.GetVal<T>(recdData, key);
        }
        public T GetRecd<T>(string key, T dftVal)
        {
            return CommonUtils.GetVal<T>(recdData, key, dftVal);
        }

        public string GetRecdBase64Decode(string key)
        {
            string data = GetRecd<string>(key);
            if (!string.IsNullOrEmpty(data))
            {
                data = CommonUtils.Base64Decode(data);
            }
            return data;
        }

        public bool RecdHasKey(object key)
        {
            return recdData.ContainsKey(key);
        }

        public bool RecdHasVal(object val)
        {
            return recdData.ContainsValue(val);
        }

        public static string ToString(object table)
        {
            return MiniJSON.JsonEncode(table);
        }

        public JSONObject ParseRecdJosn(string json)
        {
            object retObj = MiniJSON.JsonDecode(json);

            if (null == retObj)
            {
                recdStr = json;
                return this;
            }

            if (retObj is ArrayList)
            {
                recdList = retObj as ArrayList;
                this.isArray = true;
            }
            else
            {
                recdData = retObj as Hashtable;
            }

            return this;
        }
        public void SetSendToRecd()
        {
            IDictionaryEnumerator enumer = sendData.GetEnumerator();
            while (enumer.MoveNext())
            {
                object key = enumer.Key;
                this.AddRecv(key, enumer.Value);
            }
        }
        public void SetRecdToSend()
        {
            IDictionaryEnumerator enumer = recdData.GetEnumerator();
            while (enumer.MoveNext())
            {
                object key = enumer.Key;
                this.Add(key, enumer.Value);
            }
        }
        public void Clear()
        {
            this.sendData.Clear();
            if (this.recdData != null)
            {
                this.recdData.Clear();
            }
        }
        public void HashTableToForm(Hashtable table)
        {
            IDictionaryEnumerator enumer = sendData.GetEnumerator();
            while (enumer.MoveNext())
            {
                object key = enumer.Key;
                this.AddRecv(key, enumer.Value);
            }
        }

        public JSONObject ParseSendJosn(string json)
        {
            object retObj = MiniJSON.JsonDecode(json);

            if (null == retObj)
            {
                return null;
            }

            if (retObj is ArrayList)
            {
                sendList = retObj as ArrayList;
                this.isArray = true;
            }
            else
            {
                sendData = retObj as Hashtable;
            }

            return this;
        }
        public static JSONObject ParseSendAndRec(string json)
        {
            JSONObject jsonObject = Parse(json);
            jsonObject.SetRecdToSend();
            return jsonObject;
        }
        public static JSONObject Parse(string json)
        {
            return Parse(json, true);
        }
        public static JSONObject Parse(string json, bool newWF)
        {
            JSONObject jsonObj = new JSONObject(newWF);
            jsonObj.ParseRecdJosn(json);
            return jsonObj;
        }
        public static JSONObject ParseSend(string json, bool newWF = true)
        {
            JSONObject jsonObj = new JSONObject(newWF);
            jsonObj.ParseRecdJosn(json);
            jsonObj.SetRecdToSend();
            return jsonObj;
        }

        public WWWForm WForm
        {
            get { return wForm; }
        }

        public byte[] PostData
        {
            get
            {
                if (wForm != null)
                {
                    return wForm.data;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return "SendDataJosn:" + SendDataJosn + ", RecdDataJosn:" + RecdDataJosn;
        }
        public string ToFileString()
        {
            return SendDataJosn;
        }

        public string SendDataJosn
        {
            get { return MiniJSON.JsonEncode(sendData); }
        }

        public string RecdDataJosn
        {
            get { return MiniJSON.JsonEncode(recdData); }
        }
        public object GetSendData(object key)
        {
            return sendData[key];
        }
        public void GC()
        {
            this.paramDict = null;
            //this.urlParamList = null;
            this.sendData = null;
            this.recdData = null;
            this.recdList = null;
            this.wForm = null;
        }
    }
}