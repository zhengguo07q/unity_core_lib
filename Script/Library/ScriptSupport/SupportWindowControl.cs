// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SupportWindowControl.cs
//  Creator 	: zg
//  Date		: 2016-11-23
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SupportWindowControl : WindowControl
{
    protected LuaFunction initializeFunc;
    protected LuaFunction disposeFunc;


    protected override void CacheLuaFunction()
    {
        base.CacheLuaFunction();
        initializeFunc = ScriptHelper.GetFunction(LuaTable, "initialize");
        disposeFunc = ScriptHelper.GetFunction(LuaTable, "dispose");
    }


    public override void Initialize()
    {
        base.Initialize();
        if (initializeFunc != null)
        {
            initializeFunc.call(LuaTable);
        }
    }


    public override void Dispose()
    {
        base.Dispose();
        if (disposeFunc != null)
        {
            disposeFunc.call(LuaTable);
        }
    }
}

