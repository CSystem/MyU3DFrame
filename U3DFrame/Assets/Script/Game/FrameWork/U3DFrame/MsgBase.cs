using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U3DFrame.Interface;

namespace U3DFrame.Base
{
    public class MsgBase : IMessage
    {

        public MsgBase(ushort id)
        : this(id, null, null)
        { }
        public MsgBase(ushort id, string name)
        : this(id, name, null, null)
        { }

        public MsgBase(ushort id, string name, object body)
        : this(id, name, body, null)
        { }

        public MsgBase(ushort id, string name, object body, string type)
        {
            m_MsgId = id;
            m_Name = name;
            m_Body = body;
            m_Type = type;
        }

        private object m_Body;
        public object Body
        {
            get
            {
                return m_Body;
            }

            set
            {
                m_Body = value;
            }
        }
        private ushort m_MsgId;
        public ushort MsgId
        {
            get
            {
                return m_MsgId;
            }

            set
            {
                m_MsgId = value;
            }
        }
        private string m_Name;
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        private string m_Type;
        public string Type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
            }
        }

        public override string ToString()
        {
            string msg = "Notification id: " + MsgId;
            msg += "\nBody:" + ((Body == null) ? "null" : Body.ToString());
            msg += "\nType:" + ((Type == null) ? "null" : Type);
            return msg;
        }
    }
}
