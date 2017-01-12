// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowStack.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CustomLuaClass]
public enum WindowEvent
{
    weNull,
    weOpen,
    weOpenAfter,
    weClose,
    weCloseBefore,
}


public interface WindowListener
{
    void Execute(WindowEvent evt, WindowBase window);

}


[CustomLuaClass]
public class WindowStack
{
    private Dictionary<WindowRes, WindowBase> windowBaseDict = new Dictionary<WindowRes, WindowBase>();     //存储的已经创建的窗口
    private WindowSorter sorter = new WindowSorter();
    private List<WindowListener> listeners = new List<WindowListener>();
    public WindowBase CurrentTopWindow { set; get; }


    public void AddListener(WindowListener listener)
    {
        listeners.Add(listener);
    }


    public void RemoveListener(WindowListener listener)
    {
        listeners.Remove(listener);
    }


    public void ClearListener()
    {
        listeners.Clear();
        
    }


    public void NotifyListener(WindowBase windowBase, WindowEvent evt)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            WindowListener listener = listeners[i];
            if (listener == null)
                continue;
            listener.Execute(evt, windowBase);
        }
    }


    public void ResetStackWindow()
    {
        windowBaseDict.Clear();
        sorter.Reset();
    }


    public void OnOpenWindow(WindowBase windowBase)
    {
        if (windowBase.isAfterComplete == false)   //这是为了进行分帧优化，分帧调用完成后， UI才会主动发起打开完成事件
        {
            NotifyListener(windowBase, WindowEvent.weOpenAfter);
        }
    }


    public void OnCloseWindow(WindowBase windowBase)
    {
        windowBase.gameObject.SetActive(false);
        NotifyListener(windowBase, WindowEvent.weClose);

    }


    public void AfterCreateWindow(WindowBase windowBase)
    {
        NotifyListener(windowBase, WindowEvent.weOpen);
    }


    public void BeforeDestroyWindow(WindowBase windowBase)
    {
        NotifyListener(windowBase, WindowEvent.weCloseBefore);

        windowBase.RemoveSelfFromParent();
        sorter.RemoveWindow(windowBase);
        windowBaseDict.Remove(windowBase.GetWindowRes());

        GameObjectUtility.DestoryGameObject(windowBase.gameObject);
        AssetLoader.Instance.ClearAssets();
    }


    public WindowBase GetWindow(WindowRes resId)
    {
        WindowBase window;
        windowBaseDict.TryGetValue(resId, out window);
        return window;
    }


    public LuaTable GetLuaWindow(WindowRes resId)
    {
        WindowBase windowBase = GetWindow(resId);
        if (windowBase != null)
            return windowBase.LuaTable;
        return null;
    }


    public WindowBase OpenWindow(WindowRes resId, WindowBase parentWindow = null, bool isHideParent = false)
    {
        float openStartTime = Time.realtimeSinceStartup;
        WindowBase window;
        if (windowBaseDict.TryGetValue(resId, out window) == false)
        {
            window = CreateWindow(resId);

            windowBaseDict.Add(resId, window);
        }

        if (window.IsShow == false)
        {
            if (parentWindow != null)
            {
                parentWindow.AddChildWindow(window);
                window.ParentWindow = parentWindow;
            }
            else
            {
                WindowBase.BaseChildList.Add(window);
            }
            sorter.AddWindow(window);
            sorter.AdjustmentWindow();

            window.Open(isHideParent);
        }

        window.openFuncTime = Time.realtimeSinceStartup - openStartTime;
        CurrentTopWindow = window;
        return window;
    }


    public void OpenLuaWindow(WindowRes resId, LuaTable parentWindow, bool isHideParent)
    {
        WindowBase parent = null;
        if (parentWindow != null)
        {
            parent = (WindowBase)parentWindow["this"];
        }
        WindowBase window = OpenWindow(resId, parent, isHideParent);
    }


    public WindowBase CreateWindow(WindowRes resId)
    {
        WindowBase window = null;
        float resourceLoadStartTime = Time.realtimeSinceStartup;
		GameObject resourceGo = ResourceLoader.Instantiate(resId.resourcePath);
        float resourceLoadTime = Time.realtimeSinceStartup - resourceLoadStartTime;
        resourceGo.SetActive(false);
        LayerUtility.SetLayer(resourceGo, 5);
        GameObjectUtility.AddGameObject(WindowBase.WindowRoot, resourceGo);
        window = resourceGo.AddComponent(resId.windowClazz) as WindowBase;
        window.resourceLoadTime = resourceLoadTime;
        window.SetWindowRes(resId);

        AfterCreateWindow(window);
        return window;
    }


    public bool IsWindowExist(WindowRes resId)
    {
        WindowBase windowBase;
        return windowBaseDict.TryGetValue(resId, out windowBase);
    }


    public bool IsWindowExist()
    {
        if (windowBaseDict.Count > 0)
            return true;
        return false;
    }


    public void DisableWindow(WindowRes resId)
    {
        WindowBase window;
        if (windowBaseDict.TryGetValue(resId, out window) == true)
        {
            window.SetDisable();
        }
    }


    public void CloseWindow(WindowRes resId)
    {
        WindowBase window;
        if (windowBaseDict.TryGetValue(resId, out window) == true)
        {
            window.Close();
        }
    }


    public void DestoryAll()
    {
        List<WindowBase> windowList = windowBaseDict.Values.ToList<WindowBase>();
        for (int i = 0; i < windowList.Count; i++)
        {
            WindowBase windowBase = windowList[i];
            if (windowBase == null)
                continue;

            windowBase.Destory();
        }

        ResetStackWindow();
    }


    public Dictionary<WindowRes, WindowBase> GetBaseDict()
    {
        return windowBaseDict;
    }


    public WindowSorter GetSorter()
    {
        return sorter;
    }


    public readonly static WindowStack Instance = new WindowStack();
}

