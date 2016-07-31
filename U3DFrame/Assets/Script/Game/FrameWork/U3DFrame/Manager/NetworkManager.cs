using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using U3DFrame.Net;

namespace U3DFrame.Core
{
    public class ResponseData
    { }

    public class NetworkManager 
    {
        private SocketClient socket;
        private HttpClient http;
        static Queue<KeyValuePair<int, ByteBuffer>> sEvents = new Queue<KeyValuePair<int, ByteBuffer>>();
        static Queue<ResponseData> responseList = new Queue<ResponseData>();
        public static Queue<ResponseData> ResponseList
        {
            get
            {
                return responseList;
            }
        }

        SocketClient SocketClient
        {
            get
            {
                if (socket == null)
                    socket = new SocketClient();
                return socket;
            }
        }

        HttpClient HttpClient
        {
            get
            {
                if (null == http)
                    http = new HttpClient();
                return http;
            }
        }

        void Awake()
        {
            Init();
        }

        void Init()
        {
            SocketClient.OnRegister();
        }

        public void OnInit()
        {
            CallMethod("Start");
        }

        public void Unload()
        {
            CallMethod("Unload");
        }

        /// <summary>
        /// ִ��Lua����
        /// </summary>
        public object[] CallMethod(string func, params object[] args)
        {
            return null;//GameUtil.CallMethod("Network", func, args);
        }

        ///------------------------------------------------------------------------------------
        public static void AddEvent(int _event, ByteBuffer data)
        {
            sEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
        }

        public static void AddResponseData(ResponseData data)
        {
            responseList.Enqueue(data);
        }

        /// <summary>
        /// ����Command�����ﲻ����ķ���˭��
        /// </summary>
        void Update()
        {
            SocketClient.Update();
            HttpClient.Update();

            if (sEvents.Count > 0)
            {
                while (sEvents.Count > 0)
                {
                    KeyValuePair<int, ByteBuffer> _event = sEvents.Dequeue();
                    //facade.SendMessageCommand(NotiConst.DISPATCH_SOCKET_MESSAGE, _event);
                }
            }
        }

        void FixdUpdated()
        {
            //HttpClient.FixedUpdate();
            if (responseList.Count > 0)
            {
                //ResponseData data = responseList.Dequeue();
                //facade.SendMessageCommand(NotiConst.DISPATCH_HTTP_MESSAGE, data);
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void SendConnect()
        {
            SocketClient.SendConnect();
        }

        /// <summary>
        /// ����HHttp���� 
        /// </summary>
        public void SendHttp()
        {

        }

        /// <summary>
        /// ����SOCKET��Ϣ
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            SocketClient.SendMessage(buffer);
        }

        /// <summary>
        /// ��������
        /// </summary>
        new void OnDestroy()
        {
            SocketClient.OnRemove();
            Debug.Log("~NetworkManager was destroy");
        }
    }
}