// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LuaScriptSvr.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using LuaInterface;
using SLua;
using UnityEngine;

[CustomLuaClass]
public class LuaScriptSvr
{
    public LuaState luaState;
    static LuaSvrGameObject lgo;  
    int errorReported = 0;


    public LuaScriptSvr()
    {
        LuaState.loaderDelegate += luaLoader;
        luaState = new LuaState();
        LuaObject.init(luaState.L);

        GameObject go = new GameObject("LuaSvrProxy");
        lgo = go.AddComponent<LuaSvrGameObject>();
        GameObject.DontDestroyOnLoad(go);
        lgo.state = luaState;
        lgo.onUpdate = this.tick;

        LuaTimer.reg(luaState.L);
        LuaCoroutine.reg(luaState.L, lgo);
        Helper.reg(luaState.L);       
    }


    void tick()
    {
        if (LuaDLL.lua_gettop(luaState.L) != errorReported)
        {
            Debug.LogError("Some function not remove temp value from lua stack. You should fix it.");
            errorReported = LuaDLL.lua_gettop(luaState.L);
        }

        luaState.checkRef();
        LuaTimer.tick(Time.deltaTime);
    }


    byte[] luaLoader(string fn)
    {
        string script = "";
        if (fn.EndsWith(".lua"))
        {
            script = PathUtility.LuaPath + "/" + fn;
        }
        else
        {
            fn = fn.Replace(".", "/");
            script = PathUtility.LuaPath + "/" + fn + ".lua";
        }
        byte[] bytes = LuaFileCache.Instance.LoadFile(script);
        return bytes;
    }


    public object this[string path]
    {
        get
        {
            return luaState.getObject(path);
        }
        set
        {
            luaState.setObject(path, value);
        }
    }
}
