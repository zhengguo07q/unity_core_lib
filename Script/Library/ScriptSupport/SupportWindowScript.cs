// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SupportWindowScript.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SupportWindowScript : DomamolDialogBase
{
    protected LuaFunction initializeFunc;
    protected LuaFunction disposeFunc;


    protected override void CacheLuaFunction()
    {
        base.CacheLuaFunction();
        initializeFunc = ScriptHelper.GetFunction(LuaTable, "initialize");
        disposeFunc = ScriptHelper.GetFunction(LuaTable, "dispose");
    }


    protected override void Initialize()
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

