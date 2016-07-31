using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace U3DFrame.Tool
{
    public class UnityTools
    {
        public static bool IsPackageResource(string path)
        {
            return path != null && path.Length > 0 && path.Contains("/package");
        }

        static public string GetPackageName(string assetPath)
        {
            return CaptureWord(assetPath, @"\/(package\w+)\/");
        }

        static public string GetPackagePath(string assetPath)
        {
            return CaptureWord(assetPath, @"(.*\/package\w+)\/");
        }

        static public string GetAssetnameInPackage(string assetPath)
        {
            //return CaptureWord(assetPath, @".*\/(\w+)\.\w+"); //获取纯文件名
            return CaptureWord(assetPath, @"\/package\w+\/(.*)");   //获取package之后的路径
        }

        static public string CaptureWord(string src, string reg)
        {
            Regex _reg = new Regex(reg);
            Match mc = _reg.Match(src);
            if (mc.Groups.Count > 1 && mc.Groups[1].Captures.Count > 0)
            {
                return mc.Groups[1].Captures[0].Value;
            }
            else
            {
                return "";
            }
        }

        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
        }
        public static AsyncOperation LoadLevel(string lvlName)
        {
            if (Application.isLoadingLevel && Application.loadedLevelName == lvlName)
            {
                // return null;
            }
            AsyncOperation so = Application.LoadLevelAsync(lvlName);
            return so;
        }
        public static void Destroy(params object[] eles)
        {
            _Destroy(eles as IEnumerable);
        }

        private static void _Destroy(IEnumerable eles)
        {
            Dictionary<string, GUITexture> tts = eles as Dictionary<string, GUITexture>;
            Dictionary<string, GUIText> txs = eles as Dictionary<string, GUIText>;
            if (tts != null)
            {
                eles = tts.Values;
            }
            else if (txs != null)
            {
                eles = txs.Values;
            }
            foreach (object ele in eles)
            {
                GameObject go = ele as GameObject;
                Component comp = ele as Component;
                IEnumerable iEnum = ele as IEnumerable;
                Object obj = ele as Object;

                if (go != null)
                {
                    Object.Destroy(go);
                    continue;
                }
                if (comp != null)
                {
                    Object.Destroy(comp.gameObject);
                    continue;
                }
                if (iEnum != null)
                {
                    _Destroy(iEnum);
                    continue;
                }
                if (obj != null)
                {
                    Object.DestroyImmediate(obj, true);
                    continue;
                }
            }
        }
       
        /// <summary>
        /// 存储了资源，并非实例
        /// </summary>
        private static Dictionary<string, object> objectDict = new Dictionary<string, object>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetResouces(string key, object obj)
        {
            objectDict[key] = obj;
        }        
          
   
        public static bool IsActive(Transform tf)
        {
            return IsActive(tf.gameObject);
        }
        public static bool IsActive(GameObject go)
        {
            return go.activeInHierarchy;
        }
        public static string GetPlayerPrefsString(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        public static int GetPlayerPrefsInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }
        public static bool HasPlayerPrefs(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public static void SetPlayerPrefsInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
            PlayerPrefs.Save();
        }
        public static void SetPlayerPrefsStr(string key, string val)
        {
            PlayerPrefs.SetString(key, val);
            PlayerPrefs.Save();
        }
        public static void SetActive(bool bActive, params GameObject[] objs)
        {
            if (objs == null || objs.Length == 0)
                return;

            // foreach (GameObject obj in objs)
            for (int i = 0; i < objs.Length; i++ )
            {
                GameObject obj = objs[i];
                SetActive(bActive, obj);
            }
        }
        public static void SetActive(bool bActive, params Transform[] tfs)
        {
            if (tfs == null || tfs.Length == 0)
                return;

            //foreach (Transform tf in tfs)
            for (int i = 0; i < tfs.Length; i++)
            {
                Transform tf = tfs[i];
                SetActive(bActive, tf);
            }
        }
        public static void SetActive(bool bActive, Transform tf)
        {
            if (tf == null)
            {
                return;
            }
            SetActive(bActive, tf.gameObject);
        }

        public static GameObject Inst(GameObject go)
        {
            return Inst(go, go.transform.parent);
        }

        public static GameObject Inst(GameObject go, Component parent)
        {
            GameObject clone = Object.Instantiate(go) as GameObject;
            clone.name = go.name;
            if (parent != null)
            {
                clone.transform.parent = parent.transform;
            }
            Renderer render = clone.GetComponent<Renderer>();
            if (render != null && render.sharedMaterial == null)
            {
                render.sharedMaterial = go.GetComponent<Renderer>().sharedMaterial;
            }
            clone.transform.localPosition = go.transform.localPosition;
            clone.transform.localEulerAngles = go.transform.localEulerAngles;
            clone.transform.localScale = go.transform.localScale;
            return clone;
        }

        public static GameObject Inst(GameObject go, Vector3 posi, Component parent)
        {
            GameObject clone = Inst(go, parent);
            clone.transform.position = posi;
            return clone;
        }

        public static GameObject Inst(GameObject go, Quaternion rota, Component parent)
        {
            GameObject clone = Inst(go, parent);
            clone.transform.rotation = rota;
            return clone;
        }

        public static GameObject Inst(GameObject go, Vector3 posi, Quaternion rota, Component parent)
        {
            GameObject clone = Inst(go, posi, parent);
            clone.transform.rotation = rota;
            return clone;
        }

        private static void ActGo(bool isAct, GameObject go)
        {
            go.SetActive(isAct);
        }

        public static void ActComp(bool isAct, Component comp)
        {
            ActGo(isAct, comp.gameObject);
        }

        public static void ActComp(bool isAct, params object[] eles)
        {
            _ActComp(false, isAct, eles as IEnumerable);
        }

        public static void ToggleComp(params object[] eles)
        {
            _ActComp(true, false, eles as IEnumerable);
        }

        private static void _ActComp(bool isToggle, bool isAct, IEnumerable eles)
        {
            Dictionary<string, GUITexture> tts = eles as Dictionary<string, GUITexture>;
            Dictionary<string, GUIText> txs = eles as Dictionary<string, GUIText>;

            if (tts != null)
            {
                eles = tts.Values;

            }
            else if (txs != null)
            {
                eles = txs.Values;
            }

            foreach (object ele in eles)
            {
                Component comp = ele as Component;
                IEnumerable iEnum = ele as IEnumerable;

                if (comp != null)
                {
                    if (isToggle)
                    {
                        ToggleComp(comp);
                    }
                    else
                    {
                        ActComp(isAct, comp);
                    }
                    continue;
                }
                if (iEnum != null)
                {
                    _ActComp(isToggle, isAct, iEnum);
                    continue;
                }
            }
        }

        public static List<T> FindCompentInChildren<T>(Transform root) where T : UnityEngine.Component
        {
            List<T> ret = new List<T>();
            T com = root.GetComponent<T>();
            if (com != null)
            {
                ret.Add(com);
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform cld = root.GetChild(i);
                List<T> temp = FindCompentInChildren<T>(cld);
                if (temp != null)
                {
                    ret.AddRange(temp);
                }
            }
            return ret;
        }
    }
}
