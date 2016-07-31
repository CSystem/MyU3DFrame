using System;
using System.Collections.Generic;
using U3DFrame.Tool;
using U3DFrame.UI.Controller;
using UnityEngine;

namespace U3DFrame.UI.View
{

    //UI设计类，实现该UI所需要的所有接口，为了和lua的方式统一，lua脚本无法挂在prefab上，所以要重新实现UI接口
    public abstract class UiDesignerBase : IUiDesigner
    {
        public enum E_EventObjType
        {
            EventObj_Label = 0,
            EventObj_Button,
            EventObj_Texture,
            EventObj_Sprite,
            EventObj_GameObj,
        }

        public enum E_FindType
        {
            E_FindType_ID,
            E_FindType_Index,
        }

        public enum E_ResourceType
        {
            E_MainForm = 0,
            E_UIElement,
        }

        public class NeedResourceInfo
        {
            public string strPath = "";
            public string strCacheToGuiManagerKey = "";
            public E_ResourceType eResType = E_ResourceType.E_MainForm;
            public bool bNeedLoad = true;
            private UnityEngine.Object _resObj = null;
            public UnityEngine.Object ResObj
            {
                get { return _resObj; }
                set { _resObj = value; }
            }
        }

        private const string STR_ADDITION_KEY_TO_GUIMANAGER = "WindowType";

        private CUiBase m_iUIBase = null;
        private bool m_bOpen = false;
        private GuiPanel m_iPanel = null;
        private UIPanel m_aUIPanel = null;

        private List<NeedResourceInfo> m_ltNeedResource = new List<NeedResourceInfo>();

        protected abstract void InitializeForm(bool bSuccess);

        public GuiPanel ipanel
        {
            get { return m_iPanel; }
            set
            {
                m_iPanel = value;
            }
        }

        public bool DesignerOpen
        {
            get { return m_bOpen; }
        }

        public void SetUIBase(CUiBase iUIBase)
        {
            m_iUIBase = iUIBase;
        }

        virtual public void Init(System.Object iUIObj)
        {

        }

        virtual public void Destory()
        {
            CloseForm();
            m_ltNeedResource.Clear();
        }

        public void SetVisible(bool bVisible)
        {
            if (ipanel != null && m_bOpen)
            {
                ipanel.gameObject.SetActive(bVisible);
                if (bVisible)
                {
                    OnRefresh(ipanel.gameObject);
                }
            }
        }

        public void SetWindowDepth(int nDepth)
        {
            if (ipanel != null && m_aUIPanel != null)
            {
                m_aUIPanel.depth = nDepth;
            }
        }

        protected void AddNeedResInfo(E_ResourceType eResType, string strResPath)
        {
            NeedResourceInfo aResInfo = new NeedResourceInfo();
            aResInfo.strCacheToGuiManagerKey = strResPath + STR_ADDITION_KEY_TO_GUIMANAGER + m_iUIBase.UIWindowType().ToString();
            aResInfo.strPath = strResPath;
            aResInfo.eResType = eResType;
            m_ltNeedResource.Add(aResInfo);
        }

        protected NeedResourceInfo GetResObject(string strResPath)
        {
            foreach (NeedResourceInfo aResInfo in m_ltNeedResource)
            {
                if (aResInfo.strPath == strResPath)
                {
                    return aResInfo;
                }
            }
            return null;
        }

        //打开界面
        protected void OpenForm(bool bCacheToGuiManager = true)
        {
            if (m_bOpen)
            {
                return;
            }
            m_bOpen = true;
            GuiPanelManager panelManager = GameApp.Instance.GuiPanelManager;
            for (int i = 0; i < m_ltNeedResource.Count; i++)
            {
                NeedResourceInfo resourceInfo = m_ltNeedResource[i];
                if (null == resourceInfo)
                    continue;
                if (!string.IsNullOrEmpty(resourceInfo.strCacheToGuiManagerKey) && !panelManager.HasExistForm(resourceInfo.strCacheToGuiManagerKey))
                {
                    resourceInfo.bNeedLoad = true;
                    panelManager.LoadResource(resourceInfo.strPath, HandleUIResourceOK);
                }
                else
                {
                    resourceInfo.bNeedLoad = false;
                }
            }
            CheckAllNeedResInfoFinish();
        }

        private void HandleUIResourceOK(string path, UnityEngine.Object res, System.Object callbackObj)
        {
            if (!m_bOpen)
            {
                DebugTool.LogError("HandleUIResourceOK Error m_bOpen == False");
                return;
            }

            for (int i = 0; i < m_ltNeedResource.Count; ++i)
            {
                NeedResourceInfo aInfo = m_ltNeedResource[i];
                if (aInfo.strPath == path)
                {
                    if (res != null)
                    {
                        aInfo.ResObj = res;
                    }
                    else
                    {
                        if (aInfo.eResType == E_ResourceType.E_MainForm)
                        {
                            DebugTool.LogError("Down E_MainForm Error = " + path);
                            InitFinish(false);
                            return;
                        }
                        else
                        {
                            DebugTool.LogError("Down E_UIElement Error = " + path);
                            m_ltNeedResource.RemoveAt(i);
                        }
                    }
                    break;
                }
            }
            CheckAllNeedResInfoFinish();
        }

        private void CheckAllNeedResInfoFinish()
        {
            if (ipanel != null)
            {
                return;
            }
            bool bFinish = true;
            NeedResourceInfo aMainFormResInfo = null;

            foreach (NeedResourceInfo aInfo in m_ltNeedResource)
            {
                if (aInfo.ResObj == null && aInfo.bNeedLoad)
                {
                    bFinish = false;
                }
                if (aInfo.eResType == E_ResourceType.E_MainForm)
                {
                    aMainFormResInfo = aInfo;
                }
            }
            GuiPanelManager panelManager = GameApp.Instance.GuiPanelManager;
            if (bFinish && aMainFormResInfo != null)
            {
                if (!aMainFormResInfo.bNeedLoad)
                {
                    if (InitPanel(panelManager.GetFormFromCache(aMainFormResInfo.strCacheToGuiManagerKey)))
                    {
                        InitFinish(true);
                    }
                    else
                    {
                        InitFinish(false);
                    }
                }
                else
                {
                    DoLoadUIPrefab(aMainFormResInfo.ResObj, aMainFormResInfo.strCacheToGuiManagerKey);
                }
            }
        }

        private bool InitPanel(GameObject ui)
        {
            if (null == ui)
            {
                return false;
            }
            ui.SetActive(true);
            if (null == m_iPanel)
            {
                m_iPanel = ui.GetComponent<GuiPanel>();
            }

            if (null == m_aUIPanel)
            {
                m_aUIPanel = ipanel.gameObject.GetComponent<UIPanel>();
            }

            if (null == m_iPanel)
            {
                return false;
            }
            else
            {
                //TODO 挂一些资源类脚本
            }

            if (m_aUIPanel != null && m_iUIBase != null)
            {
                m_aUIPanel.depth = m_iUIBase.WindowDepth;
            }

            return true;
        }

        //加载完毕
        private void DoLoadUIPrefab(UnityEngine.Object prefab, string strFormPath)
        {
            if (m_iPanel != null)
            {
                DebugTool.LogError("Repeat to open the Form");
                return;
            }
            if (prefab == null)
            {
                DebugTool.LogError("can't open Form");
            }
            else
            {
                GameObject ui = (GameObject)GameObject.Instantiate(prefab);
                if (!InitPanel(ui))
                {
                    InitFinish(false);
                    return;
                }

                GameApp.Instance.GuiPanelManager.SetFormToCache(strFormPath, ui);
                // 
                GameObject anchor = GameApp.Instance.GuiPanelManager.UiRoot;
                if (anchor == null)
                    return;
                Vector3 vUISelfPos = ui.transform.localPosition;
                ui.transform.parent = anchor.transform;

                ui.gameObject.transform.localScale = Vector3.one;
                ui.transform.localPosition = vUISelfPos;
                InitFinish(true);
            }
        }

        void AddComponent()
        {
            if (string.IsNullOrEmpty(m_iPanel.m_strFormScript)
                 || isHaveSpace(m_iPanel.m_strFormScript))
                return;
            //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(m_iPanel.gameObject, "Assets/Script/U3DFrame/MVCView/View/UiDesignerBase.cs (293,9)", m_iPanel.m_strFormScript);
        }

        bool isHaveSpace(string String)
        {
            int idex = String.IndexOf(" ");
            if (idex >= 0)
                return true;

            return false;
        }


        public void CloseForm()
        {
            if (m_iPanel != null)
            {
                //隐藏，下次直接取
                ipanel.gameObject.SetActive(false);
                GameApp.Instance.GetManager<GuiPanelManager>(typeof(GuiPanelManager).ToString()).OnRestore(m_iPanel);
                m_iPanel = null;
            }

            m_bOpen = false;
        }

        public void OnRestore(GameObject aRestoreObj, bool bDestory = false)
        {
            if (aRestoreObj == null)
            {
                return;
            }
            GuiPanel aRestorePanel = aRestoreObj.GetComponent<GuiPanel>();
            if (aRestorePanel == null)
            {
                return;
            }

        }

        public void OnRefresh(GameObject aRefreshObj)
        {
            if (aRefreshObj == null)
            {
                return;
            }
            GuiPanel aRefreshPanel = aRefreshObj.GetComponent<GuiPanel>();
            if (aRefreshPanel == null)
            {
                return;
            }
        }
        //销毁cache的实例引用
        public void RemoveForm(string formName)
        {
            GuiPanelManager panelManager = GameApp.Instance.GetManager<GuiPanelManager>(typeof(GuiPanelManager).ToString());
            GameObject aObj = panelManager.GetFormFromCache(formName);
            if (null == aObj)
                return;
            CloseForm();
            GameObject.Destroy(aObj);
            panelManager.RemoveFormFromCache(formName);
        }

        protected UILabel GetLable(string LabName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrLable)
                return null;

            foreach (UILabel elable in epanle.m_arrLable)
            {
                if (null == elable || elable.name != LabName)
                    continue;

                return elable;
            }

            return null;
        }

        protected UIToggle GetToggle(string strName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrToggle)
                return null;

            foreach (UIToggle aToggle in epanle.m_arrToggle)
            {
                if (null == aToggle || aToggle.name != strName)
                    continue;

                return aToggle;
            }

            return null;
        }

        protected UIImageButton GetImageButton(string strBtnName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrImageBtn)
                return null;

            foreach (UIImageButton ebutton in epanle.m_arrImageBtn)
            {
                if (null == ebutton || ebutton.name != strBtnName)
                    continue;

                return ebutton;
            }

            return null;
        }

        protected UIButton GetButton(string strBtnName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrBtn)
                return null;

            foreach (UIButton ebutton in epanle.m_arrBtn)
            {
                if (null == ebutton || ebutton.name != strBtnName)
                    continue;

                return ebutton;
            }

            return null;
        }

        protected UISprite GetSprite(string strSpriteName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrUISprite)
                return null;

            foreach (UISprite esprite in epanle.m_arrUISprite)
            {
                if (null == esprite || esprite.name != strSpriteName)
                    continue;

                return esprite;
            }
            return null;
        }

        protected UITable GetTable(string strTableName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.gameObject.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrUITable)
                return null;

            foreach (UITable estable in epanle.m_arrUITable)
            {
                if (null == estable || estable.name != strTableName)
                    continue;

                return estable;
            }
            return null;
        }

        protected GameObject GetGameObject(string strObjectName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrGameObj)
                return null;

            foreach (GameObject esobject in epanle.m_arrGameObj)
            {
                if (null == esobject || esobject.name != strObjectName)
                    continue;

                return esobject;
            }
            return null;
        }

        protected UIGrid GetGrid(string strGridName, GameObject aFather = null)
        {
            GuiPanel aPanle = null;
            if (aFather == null)
            {
                aPanle = m_iPanel;
            }
            else
            {
                aPanle = aFather.GetComponent<GuiPanel>();
            }

            if (null == aPanle || null == aPanle.m_arrGrid)
                return null;

            foreach (UIGrid aGrid in aPanle.m_arrGrid)
            {
                if (null == aGrid || aGrid.name != strGridName)
                    continue;

                return aGrid;
            }
            return null;
        }

        protected UIScrollBar GetScrollBar(string scrollName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrScrollBar)
                return null;

            foreach (UIScrollBar escroll in epanle.m_arrScrollBar)
            {
                if (null == escroll || escroll.name != scrollName)
                    continue;

                return escroll;
            }
            return null;
        }

        protected UITexture GetUITexture(string strUITextureName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrUITexture)
                return null;

            foreach (UITexture aUITexture in epanle.m_arrUITexture)
            {
                if (null == aUITexture || aUITexture.name != strUITextureName)
                    continue;

                return aUITexture;
            }

            return null;
        }

        protected UIInput GetUIInput(string strUIInputName, GameObject eObj = null)
        {
            GuiPanel epanle = null;
            if (eObj == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = eObj.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrUIInput)
                return null;

            foreach (UIInput aUIInput in epanle.m_arrUIInput)
            {
                if (null == aUIInput || aUIInput.name != strUIInputName)
                    continue;

                return aUIInput;
            }

            return null;
        }

        protected UITextList GetUITextList(string strUITextListName, GameObject aFather = null)
        {
            GuiPanel epanle = null;
            if (aFather == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = aFather.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrUITexture)
                return null;

            foreach (UITextList aUITextList in epanle.m_arrUITextList)
            {
                if (null == aUITextList || aUITextList.name != strUITextListName)
                    continue;

                return aUITextList;
            }

            return null;
        }

        protected UIScrollView GetUIScrollView(string strUIScrollView, GameObject aFather = null)
        {
            GuiPanel epanle = null;
            if (aFather == null)
            {
                epanle = m_iPanel;
            }
            else
            {
                epanle = aFather.GetComponent<GuiPanel>();
            }

            if (null == epanle || null == epanle.m_arrScrollView)
                return null;

            foreach (UIScrollView aUIScrollView in epanle.m_arrScrollView)
            {
                if (null == aUIScrollView || aUIScrollView.name != strUIScrollView)
                    continue;

                return aUIScrollView;
            }

            return null;
        }

        protected void SetSpriteAtlasAndPic(UISprite aUISprite, string strPicNameInAtlas, string strAtlasPath, GameObject eobj = null)
        {
            UIAtlas aUIAtlas = null;

        }

        protected void SetSpriteAtlasAndPic(string strUISpriteName, string strPicNameInAtlas, string strAtlasPath, GameObject eobj = null)
        {
            UISprite aUISprite = GetSprite(strUISpriteName, eobj);
            if (aUISprite == null)
            {
                return;
            }

            SetSpriteAtlasAndPic(aUISprite, strPicNameInAtlas, strAtlasPath, eobj);
        }

        //动态设置图片;
        public void SetSpritePic(string SpriteName, string PicName, GameObject eobj = null)
        {
            UISprite esprite = GetSprite(SpriteName, eobj);
            if (esprite == null)
                return;

            //做名字筛选，省略后缀名
            if (string.IsNullOrEmpty(PicName))
                return;
            int idex = PicName.IndexOf(".");
            if (idex > 0)
            {
                string temPicName = PicName.Substring(0, idex);
                esprite.spriteName = temPicName;
            }
            else
            {
                esprite.spriteName = PicName;
            }
        }

        //刷新预设;
        protected void RefreshGameObject(string formName, GameObject agameObject)
        {

        }

        protected bool AddClickEvent(string strObjName, E_EventObjType eObjType, UIEventListener.VoidDelegate funcListener, GameObject aFather = null)
        {
            UIEventListener aEvent = AddEvent(strObjName, eObjType, aFather);
            if (aEvent != null)
            {
                aEvent.onClick = funcListener;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool AddClickEvent(GameObject eObject, UIEventListener.VoidDelegate ClickFunction)
        {
            if (eObject == null)
            {
                return false;
            }

            UIEventListener eEvent = AddEvent(eObject);
            if (eEvent == null)
                return false;

            eEvent.onClick = ClickFunction;
            return true;
        }

        protected UIEventListener AddEvent(string strObjName, E_EventObjType eObjType, GameObject aFather = null)
        {
            GameObject aNeedEventObj = null;
            switch (eObjType)
            {
                case E_EventObjType.EventObj_Button:
                    {
                        UIImageButton aImgBtn = GetImageButton(strObjName, aFather);
                        if (aImgBtn != null)
                        {
                            aNeedEventObj = aImgBtn.gameObject;
                        }
                        else
                        {
                            UIButton aBtn = GetButton(strObjName, aFather);
                            if (aBtn != null)
                            {
                                aNeedEventObj = aBtn.gameObject;
                            }
                        }
                        break;
                    }
                case E_EventObjType.EventObj_Label:
                    {
                        UILabel aLabel = GetLable(strObjName, aFather);
                        if (aLabel != null)
                        {
                            aNeedEventObj = aLabel.gameObject;
                        }
                        break;
                    }
                case E_EventObjType.EventObj_Texture:
                    {
                        UITexture aTexture = GetUITexture(strObjName, aFather);
                        if (aTexture != null)
                        {
                            aNeedEventObj = aTexture.gameObject;
                        }
                        break;
                    }
                case E_EventObjType.EventObj_Sprite:
                    {
                        UISprite aSprite = GetSprite(strObjName, aFather);
                        if (aSprite != null)
                        {
                            aNeedEventObj = aSprite.gameObject;
                        }
                        break;
                    }
                case E_EventObjType.EventObj_GameObj:
                    {
                        GameObject aObj = GetGameObject(strObjName, aFather);
                        if (aObj != null)
                        {
                            aNeedEventObj = aObj;
                        }
                        break;
                    }
            }

            if (aNeedEventObj == null)
            {
                return null;
            }

            return AddEvent(aNeedEventObj);
        }

        protected UIEventListener AddEvent(GameObject aObj)
        {
            if (aObj == null)
                return null;

            return UIEventListener.Get(aObj);
        }

        protected void SetButtonState(string buttonName, bool enabled = true, GameObject eobj = null)
        {
            UIImageButton aImageButton = GetImageButton(buttonName, eobj);
            if (aImageButton != null)
            {
                aImageButton.isEnabled = enabled;
                return;
            }

            UIButton aButton = GetButton(buttonName, eobj);
            if (aButton != null)
            {
                aButton.isEnabled = enabled;
                return;
            }
        }

        protected void SetBtnText(string strBtnName, string strTxt, GameObject aFather = null)
        {
            UIButton aBtn = GetButton(strBtnName, aFather);
            if (null != aBtn)
            {
                SetBtnText(aBtn.gameObject, strTxt);
            }
            else
            {
                UIImageButton aImgBtn = GetImageButton(strBtnName, aFather);
                if (null != aImgBtn)
                {
                    SetBtnText(aImgBtn.gameObject, strTxt);
                }
            }
        }

        protected void SetBtnText(GameObject aBtn, string strTxt)
        {
            if (null == aBtn)
            {
                return;
            }

            Transform aTransLabel = aBtn.transform.FindChild("Label");
            if (null == aTransLabel)
            {
                return;
            }

            UILabel aTxtLabel = aTransLabel.gameObject.GetComponent<UILabel>();
            if (null == aTxtLabel)
            {
                return;
            }

            aTxtLabel.text = strTxt;
        }

        protected void SetLableText(string LableName, string ShowInfo, GameObject eobj = null)
        {
            UILabel elable = GetLable(LableName, eobj);
            if (elable == null)
                return;

            elable.text = ShowInfo;
        }

        protected void SetScrollBarEvent(string scrollName, EventDelegate.Callback sbOnChange, GameObject eobj = null)
        {
            UIScrollBar escroll = GetScrollBar(scrollName, eobj);
            if (escroll == null)
                return;

            EventDelegate.Add(escroll.onChange, sbOnChange);
        }

        protected void SetSpriteFillAmount(string spriteName, float ratio, GameObject eobj = null)
        {
            UISprite esprite = GetSprite(spriteName, eobj);
            if (esprite == null)
                return;

            esprite.fillAmount = ratio;
        }

        protected void SetSpriteColor(string spriteName, Color acolor, GameObject eobj = null)
        {
            UISprite eSprite = GetSprite(spriteName, eobj);
            if (eSprite == null)
                return;

            eSprite.color = acolor;
        }

        protected void SetSpriteVisible(string spriteName, GameObject eobj = null, bool isVisible = true)
        {
            UISprite esprite = GetSprite(spriteName, eobj);
            if (esprite == null)
                return;

            esprite.gameObject.SetActive(isVisible);
        }

        protected GameObject AddUIElement(GameObject aFather, string strElementResPath, int nID = 0, string strYouNeed = "")
        {
            if (null == m_iPanel)
            {
                return null;
            }

            NeedResourceInfo aResInfo = GetResObject(strElementResPath);
            if (aResInfo == null || aResInfo.ResObj == null)
            {
                DebugTool.LogError("AddUIElement Error GetResObject Null");
                return null;
            }


            return null;
        }

        protected bool DelUIElement(GameObject aFather, int nIndexOrId, E_FindType eFindType = E_FindType.E_FindType_Index, bool bDestory = false)
        {

            return false;
        }

        protected int GetChildMaxIndex(GameObject aFather)
        {
            int nMaxIndex = -1;
            //for (int i = 0; i < aFather.transform.childCount; ++i)
            //{
            //    Transform aObj = aFather.transform.GetChild(i);
            //    if (aObj != null && aObj.gameObject.activeSelf)
            //    {
            //        ComponentCustomData aCustom = aObj.gameObject.GetComponent<ComponentCustomData>();
            //        if (aCustom != null && nMaxIndex < aCustom.m_nIndex)
            //        {
            //            nMaxIndex = aCustom.m_nIndex;
            //        }
            //    }
            //}
            return nMaxIndex;
        }

        //protected ComponentCustomData FindUIElement(GameObject aFather, int nIndexOrID, E_FindType eFindType)
        //{
        //    if (null == aFather || null == m_iPanel)
        //    {
        //        return null;
        //    }

        //    return FindUIElement(aFather, nIndexOrID, eFindType, false);
        //}

        //private ComponentCustomData FindUIElement(GameObject aFather, int nIndexOrID, E_FindType eFindType, bool bDeleWithRestoreIndex = false)
        //{
        //    if (null == aFather || null == m_iPanel)
        //    {
        //        return null;
        //    }

        //    if (!CheckFatherIsHave(aFather))
        //    {
        //        return null;
        //    }

        //    ComponentCustomData aCustomData = null;

        //    for (int i = 0; i < aFather.transform.childCount; ++i)
        //    {
        //        Transform aObj = aFather.transform.GetChild(i);
        //        if (aObj != null && aObj.gameObject.activeSelf)
        //        {
        //            ComponentCustomData aCustom = aObj.gameObject.GetComponent<ComponentCustomData>();
        //            if (null == aCustom)
        //            {
        //                continue;
        //            }

        //            if (eFindType == E_FindType.E_FindType_Index)
        //            {
        //                if (aCustom.m_nIndex == nIndexOrID && null == aCustomData)
        //                {
        //                    aCustomData = aCustom;
        //                }
        //                else if (bDeleWithRestoreIndex && aCustom.m_nIndex > nIndexOrID)
        //                {
        //                    aCustom.m_nIndex--;
        //                }
        //            }
        //            else
        //            {
        //                if (aCustom.m_nID == nIndexOrID)
        //                {
        //                    return aCustom;
        //                }
        //            }
        //        }
        //    }
        //    return aCustomData;
        //}

        private bool CheckFatherIsHave(GameObject aFather)
        {
            if (null == aFather || null == m_iPanel)
            {
                return false;
            }
            if (aFather == m_iPanel.gameObject)
            {
                return true;
            }
            bool bFindFather = false;
            for (int i = 0; i < m_iPanel.DynamicFather.Count; ++i)
            {
                if (m_iPanel.DynamicFather[i].aGameObj == aFather)
                {
                    bFindFather = true;
                    break;
                }
            }
            return bFindFather;
        }

        protected void SetOnPressEvent(string ObjString, E_EventObjType eObjType, UIEventListener.BoolDelegate PressFunction, GameObject aFather = null)
        {
            UIEventListener eEvent = AddEvent(ObjString, eObjType, aFather);
            if (eEvent != null)
            {
                eEvent.onPress = PressFunction;
            }
        }

        protected bool IsScrollViewCanMove(string strScrollViewName)
        {
            return IsScrollViewCanMove(GetUIScrollView(strScrollViewName));
        }

        protected bool IsScrollViewCanMove(UIScrollView aScrollView)
        {
            if (null == aScrollView)
            {
                return false;
            }
            if (!aScrollView.disableDragIfFits) return true;
            UIPanel mPanel = aScrollView.gameObject.GetComponent<UIPanel>();
            Vector4 clip = mPanel.finalClipRegion;
            Bounds b = aScrollView.bounds;

            float hx = (clip.z == 0f) ? Screen.width : clip.z * 0.5f;
            float hy = (clip.w == 0f) ? Screen.height : clip.w * 0.5f;

            if (aScrollView.canMoveHorizontally)
            {
                if (b.min.x < clip.x - hx) return true;
                if (b.max.x > clip.x + hx) return true;
            }

            if (aScrollView.canMoveVertically)
            {
                if (b.min.y < clip.y - hy) return true;
                if (b.max.y > clip.y + hy) return true;
            }
            return false;
        }

        /*
         * strIcon 策划编写在物体里的复合路径
         * strObjName 要设置图片的GameObject
         * aFather 这个GameObject所在的父节点，可以为null
         */
        protected bool SetIcon(string strObjName, string strIcon, GameObject aFather = null)
        {
            if (null == ipanel || string.IsNullOrEmpty(strIcon))
            {
                return false;
            }

            UISprite aSprite = GetSprite(strObjName, aFather);
            if (aSprite != null)
            {
                return SetIcon(aSprite.gameObject, strIcon);
            }

            UITexture aUITexture = GetUITexture(strObjName, aFather);
            if (aUITexture != null)
            {
                return SetIcon(aUITexture.gameObject, strIcon);
            }

            return false;
        }

        /*
         * strIcon 策划编写在物体里的复合路径
         * aObj 要设置图片的Gameobject
        */
        protected bool SetIcon(GameObject aObj, string strIcon)
        {
            if (null == aObj || null == ipanel || string.IsNullOrEmpty(strIcon))
            {
                return false;
            }

            string strPNG = ".png";
            string strPrefab = ".prefab";
            string strPath = "";
            string strName = "";

            if (strIcon.Contains(strPrefab))
            {
                UISprite aSprite = aObj.GetComponent<UISprite>();
                if (null == aSprite)
                {
                    return false;
                }

                int nPathLastIndex = strIcon.LastIndexOf("#");
                strPath = "";//GuiManage.UIAtlasPath + strIcon.Substring(0, nPathLastIndex);
                int nNameStartIndex = nPathLastIndex + 1;
                strName = strIcon.Substring(nNameStartIndex, strIcon.Length - nNameStartIndex);
                SetSpriteAtlasAndPic(aSprite, strName, strPath, null);
            }
            else if (strIcon.Contains(strPNG))
            {
                UITexture aUITexture = aObj.GetComponent<UITexture>();
                if (null == aUITexture)
                {
                    return false;
                }
                strPath = "";//GuiManage.UITexturePath + strIcon;
                SetUITexture(aUITexture, strPath);
            }

            return true;
        }

        private void SetUITexture(UITexture aUITexture, string strTexturePath)
        {

        }

        private void InitFinish(bool bSuccess)
        {
            InitializeForm(bSuccess);
            if (m_iUIBase != null)
            {
                m_iUIBase.DesignerLoadFinish(bSuccess);
            }
        }
    }
}

