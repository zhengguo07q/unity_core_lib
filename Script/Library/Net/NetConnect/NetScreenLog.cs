// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetScreenLog.cs
//  Creator 	:  
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;
using UnityEngine;


public class NetScreenLog
{
    public List<string> logStr = new List<string>();

    public void Add(string str)
    {
        lock (logStr)
        {
            logStr.Add(str);
        }
    }


    public void Clear()
    {
        lock(logStr)
        {
            logStr.Clear();
        }
    }


    public void GUIUpdate()
    {
        lock (logStr)
        {
            if (logStr.Count > 0)
            {
                if (GUILayout.Button("Clear", GUILayout.Height(30)))
                {
                    Clear();
                }
                for (int i = 0; i < logStr.Count;i++ )
                {
                    GUILayout.Label(logStr[i], GUILayout.Height(20));
                }
            }
        }
    }
}

