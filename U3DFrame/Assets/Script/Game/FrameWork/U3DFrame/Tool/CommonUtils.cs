using System;
using System.Globalization;
using System.Text;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Collections.Generic;
using U3DFrame.Zip;

namespace U3DFrame.Tool
{
    public class CommonUtils
    {
        private const int NUM_MIN = 48;
        private const int NUM_MAX = 57;

        private const int CHAR_A_U = 65;
        private const int CHAR_Z_U = 90;
        private const int CHAR_A_L = 97;
        private const int CHAR_Z_L = 122;
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string DATE_FORMAT_YYYY_MM_DD = "yyyy-MM-dd";
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string DATE_FORMAT_YYYY_MM_DD_HHMMSS = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// yy/MM/dd HH:mm:ss
        /// </summary>
        public const string DATE_FORMAT_SPLIT_YYMMDD_HHMMSS = "yy/MM/dd HH:mm:ss";
        /// <summary>
        /// yy-MM-dd
        /// </summary>
        public const string DATE_FORMAT_YY_MM_DD = "yy-MM-dd";
        /// <summary>
        /// HH:mm:ss
        /// </summary>
        public const string DATE_FORMAT_HH_MM_SS = "HH:mm:ss";
        public static string GetFormatDate(DateTime date, string format)
        {
            return string.Format("{0:" + format + "}", date);
        }
        public static DateTime DateParse(string date)
        {
            DateTime ret = DateTime.Now;
            DateTime.TryParse(date, out ret);
            return ret;
        }
        public static string Datetime()
        {
            return DateTime.Now.ToString();
        }
        public static string Format(string str, params object[] paramArr)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            try
            {
                return string.Format(str, paramArr);
            }
            catch (System.Exception ex)
            {
                DebugTool.LogError(ex.Message);
                return string.Empty;
            }
        }
        public static string Format(float val)
        {
            return val.ToString("##,###.##");
        }
        public static bool IsSameDay(DateTime src, DateTime dest)
        {
            return src.DayOfYear == dest.DayOfYear && src.Year == dest.Year;
        }

        public static string GenUUID()
        {
            return Guid.NewGuid().ToString();
        }
        public static string Replace8198(string val)
        {
            char c = (char)0x2006;
            string retVal = val.Replace(c.ToString(), "");
            return retVal;
        }
        public static bool IsNumeric(string str)
        {
            Regex reg = new Regex(@"^[-]?\d+[.]?\d*$");
            return reg.IsMatch(str);
        }

        public static int SubMin0(int dest, int sub)
        {
            int ret = dest - sub;
            ret = Mathf.Clamp(ret, 0, ret);
            return ret;
        }

        public static byte[] GetFileData(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open);
            byte[] fileData = new byte[stream.Length];
            stream.Read(fileData, 0, fileData.Length);
            stream.Close();
            return fileData;
        }
        public static string[] Split(string val, char split)
        {
            string[] ret = null;
            if (val == null)
            {
                return ret;
            }
            if (string.IsNullOrEmpty(val))
            {
                ret = new string[] { };
                return ret;
            }
            ret = val.Split(split);
            return ret;
        }

        public static string Base64Decode(object base64)
        {
            if (base64 == null || string.IsNullOrEmpty(base64.ToString()))
            {
                return string.Empty;
            }
            string str = Base64Decode(base64.ToString());
            return str;
        }

        public static string Base64Decode(string base64)
        {
            try
            {
                if (string.IsNullOrEmpty(base64))
                {
                    return string.Empty;
                }
                byte[] bytes = Convert.FromBase64String(base64);
                string str = GetString(bytes);
                if (string.IsNullOrEmpty(str))
                {
                    return base64;
                }
                return str;
            }
            catch (Exception e)
            {
                DebugTool.LogError(e.Message + e.StackTrace);
                return base64;
            }
        }

        public static string Base64Encode(object val)
        {
            return Base64Encode(val.ToString());
        }

        public static string Base64Encode(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = string.Empty;
            }
            byte[] bytes = GetBytes(val);
            string retVal = Convert.ToBase64String(bytes);
            return retVal;
        }

        public static string URLDecode(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = string.Empty;
            }
            return WWW.UnEscapeURL(url);
        }
        public static string URLEncode(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = string.Empty;
            }
            return WWW.EscapeURL(val);
        }

        public static string ToString(int intVal, int toBase)
        {
            return Convert.ToString(intVal, toBase);
        }

        public static bool EqualsIgnoreCase(string src, string target)
        {
            if (null == src || null == target)
            {
                return false;
            }
            return src.ToLower().Equals(target.ToLower());
        }

        private static System.Random random = new System.Random();
        /// <summary>
        /// 获取小于val的随机数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int GetRandomInt(int val)
        {
            return random.Next(val);
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="val"></param>
        /// <returns>int</returns>
        public static int ParseInt16(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            int retVal = 0;
            try
            {
                retVal = int.Parse(val, NumberStyles.HexNumber);
            }
            catch
            {
            }
            return retVal;
        }

        //为了检查空串或者包含空格
        public static int ParseInt(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            try
            {
                return int.Parse(val);
            }
            catch (System.Exception ex)
            {
                DebugTool.LogError("[CommonUtils.ParseInt] val : " + val + ", exception : " + ex.Message);
                return 0;
            }
        }

        public static string RemoveCharacterInString(string str, char character)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(character.ToString()))
                return str;
            if (!str.Contains(character.ToString()))
            {
                return str;
            }
            string newStr = str.Replace(character.ToString(), string.Empty);
            return newStr;
        }

        public static uint ParseUInt(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            try
            {
                return uint.Parse(val);
            }
            catch (System.Exception ex)
            {
                DebugTool.LogError("[CommonUtils.ParseInt] val : " + val + ", exception : " + ex.Message);
                return 0;
            }
        }
        public static long ParseLong(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            return long.Parse(val);
        }
        public static ulong ParseULong(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            try
            {
                return ulong.Parse(val);
            }
            catch (System.Exception ex)
            {
                DebugTool.LogError("[CommonUtils.ParseULong] ex : " + ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 字符串转bool
        /// </summary>
        /// <param name="val"></param>
        /// <returns>int</returns>
        public static bool ParseBool(string val)
        {
            return val == "1";
        }
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="val"></param>
        /// <returns>int</returns>
        public static float ParseFloat(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return 0;
            }
            try
            {

                return float.Parse(val);
            }
            catch (System.Exception ex)
            {
                DebugTool.LogError("[CommonUtils.ParseFloat] val : " + val + ", exception : " + ex.Message);
                return 0;
            }
        }
        public static Vector2 ParseVector2(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return Vector2.zero;
            }

            string temp = val.Replace("(", "");
            temp = temp.Replace(")", "");

            string[] vals = temp.Split(',');
            if (vals.Length == 2)
            {
                Vector2 ret = new Vector2();
                ret.x = ParseFloat(vals[0]);
                ret.y = ParseFloat(vals[1]);

                return ret;
            }
            else
            {
                return Vector2.zero;
            }
        }
        public static Vector3 ParseVector3(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return Vector3.zero;
            }

            string temp = val.Replace("(", "");
            temp = temp.Replace(")", "");

            string[] vals = temp.Split(',');
            if (vals.Length == 3)
            {
                Vector3 ret = new Vector3();
                ret.x = ParseFloat(vals[0]);
                ret.y = ParseFloat(vals[1]);
                ret.z = ParseFloat(vals[2]);

                return ret;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public static string Int162String(int n)
        {
            return n.ToString("x");
        }

        public static string ParseUnicode(string us)
        {
            if (string.IsNullOrEmpty(us))
            {
                return "";
            }

            string[] uss = Regex.Split(us, @"\\u");
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < uss.Length; i++)
            {
                char c = (char)ParseInt16(uss[i]);
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// int转字节数组 
        /// </summary>
        /// <param name="intVal">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte[]"/>
        /// </returns>
        public static byte[] GetBytes(int intVal)
        {
            byte[] bt = new byte[4];
            bt[0] = (byte)(0xff & intVal);
            bt[1] = (byte)((0xff00 & intVal) >> 8);
            bt[2] = (byte)((0xff0000 & intVal) >> 16);
            bt[3] = (byte)((0xff000000 & intVal) >> 24);
            return bt;
        }
        /// <summary>
        /// 字节转int 
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static int GetInt(byte[] bytes)
        {
            int num = bytes[0] & 0xFF;
            num |= ((bytes[1] << 8) & 0xFF00);
            num |= ((bytes[2] << 16) & 0xFF0000);
            num |= (int)((bytes[3] << 24) & 0xFF000000);
            return num;
        }
        /// <summary>
        /// byte[] 转字符串 
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="charset">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public static string GetString(byte[] bytes, string charset)
        {
            Encoding encoding = Encoding.GetEncoding(charset);
            return encoding.GetString(bytes);
        }
        /// <summary>
        /// byte[] 转字符串 
        /// </summary>
        /// <param name="bytes">
        ///  <see cref="System.Byte[]"/>
        /// </param>
        /// <returns>
        ///  <see cref="System.String"/>
        /// </returns>
        public static string GetString(byte[] bytes)
        {
            if (bytes.Length <= 0)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(bytes);
        }
        public static byte[] GetBytes(string param)
        {
            return Encoding.UTF8.GetBytes(param);
        }
        /// <summary>
        /// 读取len长度的字符串 UTF8编码
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="startIndex">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="len">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public static string GetString(byte[] data, int startIndex, int len)
        {
            byte[] strBytes = GetBytes(data, startIndex, len);
            string retVal = GetString(strBytes);
            return retVal;
        }

        public static byte[] GetBytes(byte[] data, int startIndex, int len)
        {
            if (len <= 0 || startIndex + len > data.Length)
            {
                return new byte[0];
            }
            byte[] retBytes = new byte[len];
            int byteIndex = 0;
            for (int idx = startIndex; idx < startIndex + len; idx++)
            {
                retBytes[byteIndex] = data[idx];
                byteIndex++;
            }
            return retBytes;
        }
        /// <summary>
        /// 读取len长度的字符串 指定charset编码
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="startIndex">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="len">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="charset">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public static string GetString(byte[] data, int startIndex, int len, string charset)
        {
            byte[] strBytes = GetBytes(data, startIndex, len);
            string retVal = GetString(strBytes, charset);
            return retVal;
        }
        /// <summary>
        /// 从startIndex位置开始读取四个字节转成int类型返回 
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="startIndex">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static int GetInt(byte[] data, int startIndex)
        {
            byte[] retVal = GetBytes(data, startIndex, 4);
            return GetInt(retVal);
        }

        public static byte[] GetBytes(float[] data)
        {
            byte[][] allBytes = new byte[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                allBytes[i] = BitConverter.GetBytes(data[i]);
            }
            return CombineBytes(allBytes);
        }

        /// <summary>
        /// 合并两个字节数组 
        /// </summary>
        /// <param name="param1">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="param2">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte[]"/>
        /// </returns>
        private static byte[] CombineTwoBytes(byte[] param1, byte[] param2)
        {
            byte[] retBytes = new byte[param1.Length + param2.Length];
            Array.Copy(param1, retBytes, param1.Length);
            Array.Copy(param2, 0, retBytes, param1.Length, param2.Length);
            return retBytes;
        }
        /// <summary>
        /// 合并字节数组 
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte[][]"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte[]"/>
        /// </returns>
        public static byte[] CombineBytes(params byte[][] bytes)
        {
            if (bytes.Length == 1)
            {
                return bytes[0];
            }
            byte[] retBytes = new byte[0];
            for (int i = 0; i < bytes.Length; i++)
            {
                retBytes = CombineTwoBytes(retBytes, bytes[i]);
            }
            return retBytes;
        }
        public static string GetValBase64Decode(Hashtable table, string key)
        {
            string ret = GetVal<string>(table, key);
            ret = Base64Decode(ret);
            return ret;
        }
        public static T GetVal<T>(Hashtable table, string key)
        {
            return GetVal<T>(table, key, default(T));
        }
        public static T GetVal<T>(Hashtable table, string key, T dftVal)
        {
            object rtn = dftVal;
            object oVal = null;
            try
            {
                if (table != null && table.ContainsKey(key))
                {
                    oVal = table[key];
                    if (oVal != null)
                    {
                        Type type = typeof(T);
                        if (type == typeof(int))
                        {
                            rtn = ParseInt(oVal.ToString());
                        }
                        else if (type == typeof(Int16))
                        {
                            rtn = ParseInt16(oVal.ToString());
                        }
                        else if (type == typeof(long))
                        {
                            rtn = ParseLong(oVal.ToString());
                        }
                        else if (type == typeof(ulong))
                        {
                            rtn = ParseULong(oVal.ToString());
                        }
                        else if (type == typeof(float))
                        {
                            rtn = float.Parse(oVal.ToString());
                        }
                        else if (type == typeof(bool))
                        {
                            if (oVal is bool)
                            {
                                rtn = (bool)oVal;
                            }
                            else
                            {
                                rtn = oVal.ToString() == "1";
                            }
                        }
                        else if (type == typeof(string))
                        {
                            rtn = oVal as string;
                        }
                        else if (type == typeof(DateTime))
                        {
                            rtn = CommonUtils.DateParse(oVal as string);
                        }
                        else if (type == typeof(Vector3))
                        {
                            rtn = new Vector3();
                            string vector = oVal.ToString();
                            vector = vector.Replace("(", "");
                            vector = vector.Replace(")", "");
                            string[] xyz = vector.Split(',');
                            if (xyz.Length == 3)
                            {
                                Vector3 ret = new Vector3();
                                ret.x = ParseFloat(xyz[0]);
                                ret.y = ParseFloat(xyz[1]);
                                ret.z = ParseFloat(xyz[2]);
                                rtn = ret;
                            }
                        }
                        else if(type == typeof(Hashtable))
                        {
                            if (oVal is Hashtable)
                            {
                                rtn = oVal;
                            }
                            else
                            {
                                DebugTool.LogError("[CommonUtil.GetVal] oVal is not hashTable : " + oVal);
                            }
                        }
                        else if (type == typeof(ArrayList))
                        {
                            if (oVal is ArrayList)
                            {
                                rtn = oVal;
                            }
                            else
                            {
                                DebugTool.LogError("[CommonUtil.GetVal] oVal is not ArrayList : " + oVal);
                            }
                        }
                        else
                        { 
                            rtn = oVal; 
                        }
                    }
                }
                return (T)rtn;
            }
            catch (Exception e)
            {
                DebugTool.LogWarning("[CommonUtil.GetVal] exception : " , e.Message , ",  oVal : " , oVal);
                if (rtn is T)
                {
                    return (T)rtn;
                }
                else
                {
                    return dftVal;
                }
            }
        }

        public static float GetRandom(float baseFloat, float range)
        {
            float retVal = baseFloat + (range * GetRandomInt(100) / 100);
            return retVal;
        }

        public static string ToGB2312(string str)
        {
            string r = "";
            MatchCollection mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.IgnoreCase);
            byte[] bts = new byte[2];
            foreach (Match m in mc)
            {
                bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                r += Encoding.Unicode.GetString(bts);
            }
            return r;
        }

        //默认密钥向量
        private static byte[] enKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = enKeys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception ex)
            {
                DebugTool.LogWarning("[CommonUtils.EncryptDES] exception : " , ex.Message);
                return encryptString;
            }
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = enKeys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="First">第一个数组</param>
        /// <param name="Second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        public static string[] MergerArray(string[] first, string[] second)
        {
            string[] result = new string[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }
        /// <summary>
        /// 数组追加
        /// </summary>
        /// <param name="Source">原数组</param>
        /// <param name="str">字符串</param>
        /// <returns>合并后的数组(数组+字符串)</returns>
        public static string[] MergerArray(string[] src, string str)
        {
            string[] result = new string[src.Length + 1];
            src.CopyTo(result, 0);
            result[src.Length] = str;
            return result;
        }

        /// <summary>
        ///判断字符串中是否包含字母 
        /// </summary>
        /// <param name="str">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public static bool StrContainChar(string str)
        {
            string tempStr = str.ToLower();

            for (char temp = 'a'; temp <= 'z'; temp++)
            {
                if (tempStr.Contains(temp.ToString()))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 是否是合法的email格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return email.Length >= 5 && IsMail(email); // Regex.IsMatch(email, EMAIL_REGEX);
        }

        public static bool IsNum(char num)
        {
            int c = (int)num;
            return NUM_MIN <= c && c <= NUM_MAX;
        }

        public static bool IsLetter(char letter)
        {
            int c = (int)letter;

            return CHAR_A_L <= c && c <= CHAR_Z_L ||
                   CHAR_A_U <= c && c <= CHAR_Z_U;
        }
        public static bool IsLetterOrNum(char c)
        {
            return IsNum(c) || IsLetter(c);
        }

        public static bool IsMail(string email)
        {
            // 当前字符的位置
            int idx = 0;
            // 是否为特殊字符
            bool isSpecial = false;
            // 是否有 '@'
            bool hasAt = false;
            // '@' 之后，必须有一个 '.'
            bool afterAtDot = false;

            foreach (char c in email)
            {
                // 首字符必须是字母或数字
                if (idx == 0 && !IsLetterOrNum(c))
                {
                    return false;
                }
                // 上一个字符是特殊字符
                else if (isSpecial)
                {
                    // 当前字符不是字母或者数字
                    if (!IsLetterOrNum(c))
                    {
                        return false;
                    }

                    isSpecial = false;
                }
                // 当前字符是特殊字符
                else if (c == '.' || c == '_' || c == '-')
                {
                    isSpecial = true;

                    if (hasAt && c == '.')
                    {
                        afterAtDot = true;
                    }
                }
                // 当前字符是 '@'
                else if (c == '@')
                {
                    // 已经有 '@'
                    if (hasAt)
                    {
                        return false;
                    }

                    hasAt = true;
                    isSpecial = true;
                }
                // 当前字符不是字母或数字
                else if (!IsLetterOrNum(c))
                {
                    return false;
                }

                idx++;
            }

            // 没有 '@' 或 '@' 之后没有 '.' 或尾字符是特殊字符
            if (!hasAt || isSpecial || !afterAtDot)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// _@.
        /// </summary>
        /// <param name="toVerified"></param>
        /// <returns></returns>
        public static bool IsAccount(string toVerified)
        {
            Regex rx = new Regex(@"^[_\.@a-zA-Z0-9]+$");
            return rx.IsMatch(toVerified, 0);//不能.Trim()
        }
        /// <summary>
        /// 是否为数字串
        /// </summary>
        public static bool IsNumberOnly(string toVerified)
        {
            Regex rx = new Regex(@"^[0-9]*$");
            return rx.IsMatch(toVerified, 0);//不能.Trim()
        }
        /// <summary>
        /// 是否为数字串
        /// </summary>
        public static bool IsPhoneNum(string toVerified)
        {
            Regex rx = new Regex(@"^((13[0-9])|(15[^4,\\D])|(18[0,5-9]))\\d{8}$");
            return rx.IsMatch(toVerified, 0);//不能.Trim()
        }
        
        /// <summary>
        /// 把paramVals拼成字符串返回
        /// </summary>
        public static string BuildString(params object[] paramVals)
        {
            StringBuilder sb = new StringBuilder();
            if (paramVals != null)
            {
                int len = paramVals.Length;
                for (int i = 0; i < len; i++)
                {
                    sb.Append(paramVals[i]);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 把paramVals拼成字符串（用空格分隔）返回
        /// </summary>
        public static string BuildStringWithSpace(params object[] paramVals)
        {
            return BuildStringWithStr(" ", paramVals);
        }

        public static string BuildStringWithEnter(params object[] paramVals)
        {
            return BuildStringWithStr("\n", paramVals);
        }

        public static string BuildStringWithStr(string str, params object[] paramVals)
        {
            StringBuilder sb = new StringBuilder();
            if (paramVals != null)
            {
                int len = paramVals.Length;
                for (int i = 0; i < len; i++)
                {
                    sb.Append(paramVals[i]);
                    sb.Append(str);
                }
            }
            return sb.ToString();
        }

        //格式：
        //** ***
        //*** ******
        //**** **
        public static string BuildText(params object[] paramVals)
        {
            StringBuilder sb = new StringBuilder();
            if (paramVals != null)
            {
                int len = paramVals.Length;
                for (int i = 0; i < len; i++)
                {
                    sb.Append(paramVals[i]);
                    if (i % 2 == 0)
                        sb.Append(" ");
                    else
                        sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        public static string GetDisplayTime(string publishTime)
        {
            if (string.IsNullOrEmpty(publishTime))
            {
                return publishTime;
            }
            else
            {
                DateTime publishDate = CommonUtils.DateParse(publishTime);
                if (CommonUtils.IsSameDay(DateTime.Now, publishDate))
                {
                    return CommonUtils.GetFormatDate(publishDate, CommonUtils.DATE_FORMAT_HH_MM_SS);
                }
                else
                {
                    return CommonUtils.GetFormatDate(publishDate, CommonUtils.DATE_FORMAT_SPLIT_YYMMDD_HHMMSS);
                }
            }
        }
        public static string GzipUncompressStr(WWW res)
        {
            try
            {
                return GZipStream.UncompressString(res.bytes);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[GameUtils.GzipUncompressStr] " + res.text + "\n" + e.Message + e.StackTrace);
                return res.text;
            }
        }

        public static string ConvertBigNumToSamll(int num)
        {
            string strNum = "";
            if (UnityEngine.Mathf.Abs(num) > 99999999)
            {
                int smallNum = num / 100000000;
                int tmp = (int)(smallNum * 100);
                smallNum = tmp / 100;
                strNum = smallNum.ToString() + "B";
            }
            else if (UnityEngine.Mathf.Abs(num) > 999999)
            {
                int smallNum = num / 1000000;
                int tmp = (int)(smallNum * 100);
                smallNum = tmp / 100;
                strNum = smallNum.ToString() + "M";
            }
            else if (UnityEngine.Mathf.Abs(num) > 9999)
            {
                int smallNum = num / 10000;
                int tmp = (int)(smallNum * 100);
                smallNum = tmp / 100;
                strNum = smallNum.ToString() + "W";
            }
            else if (UnityEngine.Mathf.Abs(num) > 999)
            {
                int smallNum = num / 1000;
                int tmp = (int)(smallNum * 100);
                smallNum = tmp / 100;
                strNum = smallNum.ToString() + "K";
            }
            else
            {
                strNum = num.ToString();
            }
            return strNum;
        }
//         public static bool UnzipFile(WWW www, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             bool ret= UnzipFile(www.bytes, destPath, completeDelegate, param);
//             www.Dispose();
//             return ret;
//         }
//         public static bool UnzipFile(byte[] bytes, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             MemoryStream mStream = new MemoryStream(bytes);
//             ZipInputStream iStream = new ZipInputStream(mStream);
//             totalSize = mStream.Length;
//             return UnzipFile(iStream, destPath, completeDelegate, param);
//         }
//         public static bool UnzipFile(string filePath, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             FileStream fileStream = File.OpenRead(filePath);
//             ZipInputStream stream = new ZipInputStream(fileStream);
//             totalSize = fileStream.Length;
//             return UnzipFile(stream, destPath, completeDelegate, param);
//         }
//         public static void UnzipFileThread(WWW www, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             byte[] bytes = www.bytes;
//             www.Dispose();
//             ThreadStart start = delegate { UnzipFile(bytes, destPath, completeDelegate, param); };
//             Thread t = new Thread(start);
//             t.IsBackground = true;
//             t.Start();
//         }
//         public static void UnzipFileThread(string filePath, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             ThreadStart start = delegate { UnzipFile(filePath, destPath, completeDelegate, param); };
//             Thread t = new Thread(start);
//             t.IsBackground = true;
//             t.Start();
//         }
        private static long unzipSize = 0;
        private static long totalSize = 0;
        public static float UnzipPercent
        {
            get
            {
                return 1.0f * unzipSize / totalSize;
            }
        }
        private static bool unzipComplete = false;
        public static bool UnzipComplete
        {
            get
            {
                return unzipComplete;
            }
        }
        public static List<string> allUnzipFileList = new List<string>();
//         private static bool UnzipFile(ZipInputStream stream, string destPath, VoidObjectDelegate completeDelegate, object param)
//         {
//             try
//             {
//                 if (stream != null)
//                 {
//                     ZipEntry theEntry; 
//                     unzipSize = 0;
//                     unzipComplete = false;
//                     while ((theEntry = stream.GetNextEntry()) != null)
//                     {
//                         string directoryName = Path.GetDirectoryName(theEntry.Name);
//                         string fileName = Path.GetFileName(theEntry.Name);
//                         // create directory
//                         if (directoryName.Length > 0)
//                         {
//                             Directory.CreateDirectory(destPath + directoryName);
//                         }
//                         if (fileName != String.Empty)
//                         {
//                             DebugTool.Log("fileName : " + directoryName + "/" + fileName);
//                             using (FileStream streamWriter = File.Create(destPath + theEntry.Name))
//                             {
//                                 int size = 2048;
//                                 byte[] data = new byte[2048];
//                                 while (true)
//                                 {
//                                     size = stream.Read(data, 0, data.Length);
//                                     if (size > 0)
//                                     {
//                                         streamWriter.Write(data, 0, size);
//                                         unzipSize += size;
//                                         DebugTool.Log("unzipSize : " + unzipSize, ", totalSize : " + totalSize);
//                                     }
//                                     else
//                                     {
//                                         break;
//                                     }
//                                 }
//                                 streamWriter.Close();
//                                 allUnzipFileList.Add(directoryName + "/" + fileName);
//                             }
//                         }
//                     } 
//                     stream.Close();
//                     unzipComplete = true;
//                     if (completeDelegate != null)
//                     {
//                         YxPlat.yxClient.DispatchToMainThread(completeDelegate, param);
//                     }
//                 }
//                 else
//                 {
//                     return false;
//                 }
//             }
//             catch (Exception e)
//             {
//                 DebugTool.LogError("[CommonUtils.UnzipFile] Unzip file exception : " + e.StackTrace);
//                 return false;
//             }
//             return true;
//         }

        public static long FileLen(FileInfo file)
        {
#if UNITY_WEBPLAYER
            return 0;
#else
            return file.Length;
#endif
        }
    }
}

