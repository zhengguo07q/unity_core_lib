// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: DebugUtility.cs
//  Creator 	: zg 
//  Date		: 2016-9-21
//  Comment		: 工具
// ***************************************************************


using System;
using System.Diagnostics;


public class DebugUtility
{
    [Conditional("Debug")]
    public static void Assert(bool condition, string msg=null, bool isThrow=true)
    {
        if (!condition)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            if (msg != null)
            {
                Logger.defaultLogger.Warn(msg);
            }
            if (isThrow)
            {
                throw new Exception(msg);
            }
        }
    }

}

