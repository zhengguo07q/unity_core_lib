// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetLog.cs
//  Creator 	:  
//  Date		: 2016-11-29
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Text;


public class NetLog
{
    public static System.Action<string> logExtra;

    public static void Error(params object[] objs)
    {
        string str = GetString(objs);
        Debug.LogError(str);
    }


    public static void Info(params object[] objs)
    {
        string str = GetString(objs);
        Debug.Log(str);
    }


    private static string GetString(params object[] objs)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < objs.Length; i++)
        {
            sb.Append(objs[i]);
        }
        return sb.ToString();
    }
}

