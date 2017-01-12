// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowBase.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: 窗口机制重新调整规则
//                1窗口存在多层级窗口， 比如说1级窗口， 2级窗口， 3级窗口等。
//                2处于最顶层级的窗口， 会默认关闭之前打开的所有的最顶层级窗口堆栈， 比如说原来打开了1， 2， 3  这样一个窗口层级， 现在又打开了一个新的1， 则原来的123全部自动关闭
//                3如果在存在多个兄弟窗口， 则兄弟窗口会被自动隐藏。 以后会被自动打开。比如说现在打开了1， 新打开了多个2层窗口， 21， 22...则打开22 会自动隐藏21， 关闭22会自动重新打开21  ？？？这个是否需要去除， 这个与窗口堆栈功能有重复
//                4打开子窗口的时候， OpenWindow(, bool isHideParent) 新加入是否需要隐藏父面板， 默认为不隐藏、
//                5加入分帧优化
// ***************************************************************


using SLua;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class WindowBase : WindowObjectContainer
{
    private static GameObject uiRoot;
    public static GameObject windowRoot;
    public static GameObject windowBackRoot;
    protected WindowEffect openEffect;
    protected WindowEffect closeEffect;
    public int sortId = -1;
    public int baseDepth = -1;
    protected Transform trans;
    protected bool isShow = false;
    public bool isDestory = false;
    public bool isAfterComplete = false;
    protected UIButton backButton;

    public float initializeFuncTime;
    public float openFuncTime;
    public float showFuncTime;
    public float resourceLoadTime;


    public void SetWindowRes(WindowRes _windowRes)
    {
        this.windowRes = _windowRes;
        openEffect = WindowEffectManager.Instance.GetWindowEffect(this, windowRes.openEffect);
        closeEffect = WindowEffectManager.Instance.GetWindowEffect(this, windowRes.closeEffect);
    }


    public WindowRes GetWindowRes()
    {
        return this.windowRes;
    }


    protected override void Awake()
    {
        base.Awake();

        trans = gameObject.transform;
        float initializeStartTime = Time.realtimeSinceStartup;
        Initialize();
        initializeFuncTime = Time.realtimeSinceStartup - initializeStartTime;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        Dispose();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        openEffect.Execute();
        float showFuncStartTime = Time.realtimeSinceStartup;
        Show();
        showFuncTime = Time.realtimeSinceStartup - showFuncStartTime;
        isShow = true;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        Hide();
        isShow = false;
        if (isDestory == true)
        {
            WindowStack.Instance.BeforeDestroyWindow(this);
        }
    }


    protected virtual void Initialize()
    {
        backButton = Find("btn_back", false) as UIButton;
        if (backButton != null)
        {
            UIEventListener.Get(backButton.gameObject).onClick += Close;
        }
    }


    protected void Close(GameObject go)
    {
        DomamolDialogBase self = this as DomamolDialogBase;
        self.Close();
    }


    protected virtual void Show()
    {

    }


    protected virtual void Hide()
    {

    }


    public override void Dispose()
    {
        base.Dispose();
        if (backButton != null)
        {
            UIEventListener.Get(backButton.gameObject).onClick -= Close;
        }
    }


    public virtual void AfterComplete() //分帧优化
    {
        WindowStack.Instance.OnOpenWindow(this);
    }


    public bool IsShow{ get{ return isShow; } }


    public static GameObject WindowRoot
    {
        get
        {
            if (uiRoot == null)
            {
                uiRoot = GameObject.Find("UI Root");
                if (uiRoot == null)
                {
                    uiRoot = GameObject.Instantiate(ResourceLoader.LoadFromResources("Prefabs/UI Root")) as GameObject;
                    GameObject.DontDestroyOnLoad(uiRoot);
                    uiRoot.name = "UI Root";
                }
            }
            if (windowRoot == null)
            {
                windowRoot = new GameObject("Window");
                GameObjectUtility.AddGameObject(uiRoot, windowRoot);
                windowRoot.transform.localScale = Vector3.one;

                windowBackRoot = new GameObject("WindowBack");
                GameObjectUtility.AddGameObject(uiRoot, windowBackRoot);
                windowBackRoot.transform.localScale = Vector3.one;
            }
            return windowRoot;
        }
    }


    public Vector3 GetWindowPosition()
    {
        return trans.localPosition;
    }


    public void SetLocalPosition(Vector3 position)
    {
        this.trans.localPosition = position;
    }


    public void Open(bool isHideParent=false)
    {
        if (isShow == true)
            return;

        CloseLastTopWindow();                                               //其他窗口是顶层窗口处理方式

        if (isHideParent == true)
        {
            DisableParent();                                                //其他窗口不是顶层处理方式
        }
        
        openEffect.Initialize();
        this.gameObject.SetActive(true);
    }


    public void Close()
    {
        FastLuaUtility.PlayAudio("10001");

        if (isShow == false)
            return;

        Destory();

        OpenParent();
    }


    protected void CloseLastTopWindow()
    {
        if (IsTopContainer() == false)
            return;

        for (int i = 0; i < BaseChildList.Count; i++)
        {
            WindowBase windowBase = BaseChildList[i] as WindowBase;
            if (windowBase == null || windowBase == this)
                continue;
            windowBase.Destory();
        }
    }


    public void OpenNextSibing()                                                //打开可能存在的兄弟
    {
        List<WindowObjectContainer> siblingList = GetSiblingWindow();                  
        if (siblingList.Count >= 1)
        {
            WindowBase windowBase = siblingList[siblingList.Count - 1] as WindowBase;
            if (windowBase == null)
                return;
            windowBase.Open();
        }
    }


    public void OpenParent()
    {
        if (ParentWindow == null)
            return;
        WindowBase windowBase = ParentWindow as WindowBase;
        windowBase.Open();
    }


    public void DisableSibing()
    {
        List<WindowObjectContainer> siblingList = GetSiblingWindow();
        for (int i = 0; i < siblingList.Count; i++)                         //打开之前需要隐藏所有的兄弟
        {
            WindowBase windowBase = siblingList[i] as WindowBase;
            if (windowBase == null)
                continue;

            windowBase.SetDisable();                                         //只调用关闭效果， 隐藏不销毁
        }
    }


    public void DisableParent()
    {
        if (ParentWindow == null)
            return;
        WindowBase windowBase = ParentWindow as WindowBase;
        if (windowBase == null)
            return;

        windowBase.SetDisable();
    }


    public void SetDisable()
    {
        if (isShow == false)
            return;

        closeEffect.Initialize();
        closeEffect.Execute();
    }


    public void Destory()
    {
        if (gameObject.activeSelf == false)
        {
            WindowStack.Instance.BeforeDestroyWindow(this);
        }
        else
        {
            isDestory = true;
            closeEffect.Initialize();
            closeEffect.Execute();
        }

        DestoryChildWindow();
    }


    public void DestoryChildWindow()
    {
        WindowBase window;
        for (int i = 0; i < ChildList.Count; i++)
        {
            window = ChildList[i] as WindowBase;
            if (window == null)
                continue;

            window.Destory();
        }
    }
}

