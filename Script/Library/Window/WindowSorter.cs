// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowSorter.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;


public class WindowSorter
{
    private List<WindowBase> sortList = new List<WindowBase>();
    private int lastSortId = 1;


    private void SortChildWindow(WindowObjectContainer parentWindow)
    {
        List<WindowObjectContainer> childList = parentWindow.ChildList;
        if (childList == null || childList.Count == 0)
            return;

        WindowBase childWindow;
        for (int i = 0; i < childList.Count; i++)
        {
            childWindow = childList[i] as WindowBase;
            if (childWindow == null)
                continue;

            childWindow.sortId = GetLastSortId();

            SortChildWindow(childWindow);
        }
    }


    private void SortWindow()
    {
        lastSortId = 0;

        for (int i = 0; i < sortList.Count; i++)
        {
            WindowBase window = sortList[i];
            if (window == null)
                continue;

            if (window.ParentWindow == null)
            {
                window.sortId = GetLastSortId();
                SortChildWindow(window);
            }
        }
    }


    public void AdjustmentWindow()
    {
        SortWindow();
        ApplyAdjustment();
    }


    private void ApplyAdjustment()
    {
        int baseDepth = (int)WindowLayerDefinition.wldUILayer;
        int panelDepth = 0;
        for (int i = 0; i < sortList.Count; i++)
        {
            WindowBase window = sortList[i];
            if (window == null)
                continue;

            panelDepth = baseDepth + window.sortId * 100;

            AdjustmentWindowDepth(window, panelDepth);
        }
    }


    private void AdjustmentWindowDepth(WindowBase window, int panelDepth)
    {
        NGUIUtility.AdjustmentPanelDepth(window.gameObject, panelDepth);
    }


    private int GetLastSortId()
    {
        lastSortId++;
        return lastSortId;
    }


    public void AddWindow(WindowBase window)
    {
        this.sortList.Add(window);
    }


    public void RemoveWindow(WindowBase window)
    {
        this.sortList.Remove(window);
    }


    public List<WindowBase> GetSortList()
    {
        return sortList;
    }


    public void Reset()
    {
        sortList.Clear();
    }
}


