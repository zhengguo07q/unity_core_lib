// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LuaFastUtility.cs
//  Creator 	:  
//  Date		: 2016-12-17
//  Comment		: 
// ***************************************************************


using SLua;


public class LuaFastUtility
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
}

