// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SupportSceneInstance.cs
//  Creator 	: zg
//  Date		: 2016-11-23
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SupportSceneInstance : SceneInstance
{
    protected LuaFunction initializeFunc;
    protected LuaFunction disposeFunc;
    protected LuaFunction putLoadResourceFunc;
    protected LuaFunction enterSceneFunc;


    protected override void CacheLuaFunction()
    {
        base.CacheLuaFunction();
        initializeFunc = ScriptHelper.GetFunction(LuaTable, "initialize");
        disposeFunc = ScriptHelper.GetFunction(LuaTable, "dispose");
        putLoadResourceFunc = ScriptHelper.GetFunction(LuaTable, "putLoadResource");
        enterSceneFunc = ScriptHelper.GetFunction(LuaTable, "enterScene");
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


    protected override void PutLoadResource()
    {
        base.PutLoadResource();
        if (putLoadResourceFunc != null)
        {
            putLoadResourceFunc.call(LuaTable);
        }
    }

    public override void EnterScene()
    {
        base.EnterScene();
        if (enterSceneFunc != null)
        {
            enterSceneFunc.call(LuaTable);
        }
    }
}

