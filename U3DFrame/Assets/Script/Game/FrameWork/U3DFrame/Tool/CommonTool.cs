using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using U3DFrame.Json;

namespace U3DFrame.Tool
{
    public class CommonTool
    {
        public static JSONObject AppendJSONObject(JSONObject json, string msg, string cmd, params object[] param)
        {
            if (json == null)
            {
                json = new JSONObject();
            }
//             json.Add(BaseConst.MSG, msg);
//             json.Add(BaseConst.CMD, cmd);
//             for (int i = 0; i < param.Length; i += 2)
//             {
//                 json.Add(param[i], param[i + 1]);
//             }
            return json;
        }

        public static JSONObject GenJSONObject(string msg, string cmd, params object[] param)
        {
            return AppendJSONObject(null, msg, cmd, param);
        }
    }
}
