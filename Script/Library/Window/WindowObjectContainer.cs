// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowObjectContainer.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections.Generic;


[CustomLuaClass]
public class WindowObjectContainer :ScriptBehaviour
{
    public static List<WindowObjectContainer> BaseChildList = new List<WindowObjectContainer>();

    public WindowObjectContainer ParentWindow { get; set; }
    private List<WindowObjectContainer> childList = new List<WindowObjectContainer>();

    protected WindowRes windowRes;


    public List<WindowObjectContainer> ChildList { get { return childList; } }


    public void RemoveSelfFromParent()
    {
        if (ParentWindow != null)
        {
            ParentWindow.RemoveChildWindow(this);
        }
        else
        {
            BaseChildList.Remove(this);
        }
    }


    public WindowObjectContainer GetChildWindow(WindowRes resId)
    {
        WindowObjectContainer windowBase;
        for (int i = 0; i < childList.Count; i++)
        {
            windowBase = childList[0];
            if (windowBase == null)
                continue;

            if (windowBase.windowRes == resId)
                return windowBase;
        }
        return null;
    }


    public void AddChildWindow(WindowObjectContainer childWindow)
    {
        childList.Add(childWindow);
    }


    public void RemoveChildWindow(WindowObjectContainer childWindow)
    {
        childList.Remove(childWindow);
    }


    protected List<WindowObjectContainer> GetSiblingWindow()
    {
        List<WindowObjectContainer> siblingList;

        if (ParentWindow != null)
        {
            siblingList = ParentWindow.childList;
        }
        else
        {
            siblingList = BaseChildList;
        }

        return siblingList;
    }


    protected bool IsTopContainer()
    {
        if (ParentWindow == null)
            return true;
        return false;
    }


    public void RemoveAll()
    {
        childList.Clear();
    }
}

