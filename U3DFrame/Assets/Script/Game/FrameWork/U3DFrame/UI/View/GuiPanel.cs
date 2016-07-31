using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum TwiButtonEventStyle
{
	Onclick = 1,
	OnHover = 2,
	Ondrag = 3	
}

public class ObjInfo
{
    public GameObject aGameObj;
    public string strPathName;
}

public class GuiPanel : MonoBehaviour 
{
	public UIImageButton[] m_arrImageBtn;
    public UIButton[] m_arrBtn;
	public UISprite[] m_arrUISprite;
	public UILabel[] m_arrLable;
	public UIScrollBar[] m_arrScrollBar;
    public UITexture[] m_arrUITexture;
    public UIToggle[] m_arrToggle;
	public string m_strFormScript = "";
	public GameObject[] m_arrGameObj;
    public UITextList[] m_arrUITextList = null;
    public UITable[] m_arrUITable;
    public UIInput[] m_arrUIInput;
    public UIScrollView[] m_arrScrollView;
    public UIGrid[] m_arrGrid;
	public int Idex;

    private List<ObjInfo> m_ltDynamicFather = new List<ObjInfo>();
    private List<ObjInfo> m_ltDynamicChild = new List<ObjInfo>();
    private List<UISprite> m_ltDynamicUISpriteAtlas = new List<UISprite>();
	private List<UITexture> m_ltDynamicUITexture = new List<UITexture>();

    public List<ObjInfo> DynamicFather
    {
        get
        {
            return m_ltDynamicFather;
        }
    }

    public List<ObjInfo> DynamicChild
    {
        get
        {
            return m_ltDynamicChild;
        }
    }

    public List<UISprite> DynamicUISpriteAtlas
    {
        get
        {
            return m_ltDynamicUISpriteAtlas;
        }
    }

	public List<UITexture> DynamicUITexture
	{
		get
		{
			return m_ltDynamicUITexture;
		}
	}
}
