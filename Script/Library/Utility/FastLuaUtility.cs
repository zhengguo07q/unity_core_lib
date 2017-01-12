// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: FastLuaUtility.cs
//  Creator 	:  
//  Date		: 2016-12-17
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;

public static class FastLuaUtility
{
    public static void PlayAudio(string key)
    {
        LuaTable singletonTb = ScriptManager.Instance.Env["singleton"] as LuaTable;
        ScriptHelper.CallFunction(singletonTb["audioPlay"] as LuaTable, "playAudio", singletonTb["audioPlay"], key);
    }


    public static string GetPrint_t(LuaTable table)
    {
        LuaTable debugTb = ScriptManager.Instance.Env["debugUtility"] as LuaTable;
        return ScriptHelper.CallFunction(debugTb, "getPrintTable", table) as string;
    }


    public static void Print(this LuaTable table)
    {
        LuaFunction function = ScriptManager.Instance.Env["print"] as LuaFunction;
        function.call(table);
    }


    public static void ScreenUtility()
    {
        LuaTable debugTb = ScriptManager.Instance.Env["screenDebugUtility"] as LuaTable;
        ScriptHelper.CallFunction(debugTb, "onGui", false);
    }


    public static void Traceback()
    {
        LuaTable debugTb = ScriptManager.Instance.Env["debug"] as LuaTable;
        string traceInfo = ScriptHelper.CallFunction(debugTb, "traceback", false) as string;
        Debug.LogError(traceInfo);
    }
}

