// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SupportUIItemRenderer.cs
//  Creator 	:  
//  Date		: 2016-11-23
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SupportUIItemRenderer : UIItemRenderer
{
    protected LuaFunction initializeFunc;
    protected LuaFunction disposeFunc;
    protected LuaFunction commitDataFunc;


    protected override void CacheLuaFunction()
    {
        base.CacheLuaFunction();
        initializeFunc = ScriptHelper.GetFunction(LuaTable, "initialize");
        disposeFunc = ScriptHelper.GetFunction(LuaTable, "dispose");
        commitDataFunc = ScriptHelper.GetFunction(LuaTable, "commitData");
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


    protected override void CommitData()
    {
        base.CommitData();
        if (commitDataFunc != null)
        {
            commitDataFunc.call(LuaTable);
        }
    }
}

