// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ClassFileName.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NGUIUtility
{

    public static void AdjustmentPanelDepth(GameObject panelGo, int panelBaseDepth)
    {
        UIPanel[] panels = panelGo.GetComponentsInChildren<UIPanel>(true);

        List<UIPanel> panelList = panels.ToList<UIPanel>();
        panelList.Sort(new ComparPriority());

        for (int i = 0; i < panelList.Count; i++)
        {
            UIPanel panel = panelList[i];
            panel.depth = panelBaseDepth++;
        }
    }



    public class ComparPriority : IComparer<UIPanel>
    {
        public int Compare(UIPanel x, UIPanel y)
        {
            return x.depth.CompareTo(y.depth);
        }

    }
}

