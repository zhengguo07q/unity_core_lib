// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptManager.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using UnityEngine;


[CustomLuaClass]
public class ScriptManager : SingletonMono<ScriptManager>
{
    public delegate void ScriptTickDelegate(int progress);
    public delegate void ScriptCompleteDelegate();

    public ScriptTickDelegate OnScriptTick;
    public ScriptCompleteDelegate OnScriptComplete;

    private LuaSvr luaSvr;


    public override void Initialize()
    {
        LuaState.loaderDelegate += LuaLoader;

        luaSvr = new LuaSvr();
    }


    public void Startup()
    {
        luaSvr.init(ScriptLoadTick, ScriptLoadCompleted, LuaSvrFlag.LSF_DEBUG);
    }


    private void ScriptLoadTick(int progress)
    {
        if (OnScriptTick != null)
        {
            OnScriptTick(progress);
        }
    }


    private void ScriptLoadCompleted()
    {
        SetPath("");
        if (OnScriptComplete != null)
        {
            OnScriptComplete();
        }
    }


    private byte[] LuaLoader(string fn)
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


    public LuaState Env
    {
        get
        {
            return luaSvr.luaState;
        }
    }


    public void SetPath(string path)
    {
        luaSvr.SetObject("package.path", luaSvr.GetObject("package.path") + ";" + PathUtility.LuaPath + "/?.lua;");
    }


    public object DoStart(string path)
    {
        return luaSvr.start(path);
    }


    public object DoFile(string path)
    {
        try
        {
            return luaSvr.luaState.doFile(path);
        } 
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }


    public object CallMethod(LuaTable luaTable, string functionName, params object[] arguments)
    {
        if (luaTable == null)
            return null;

        LuaFunction luaFunction = luaTable[functionName] as LuaFunction;
        if (luaFunction == null)
            return null;

        if (arguments != null)
        {
            return luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
            return null;
        }
    }


    //直接资源文件夹获取， 如果发现没有， 则去包体里获取，这里的资源不走引用流程
    public T LoadScriptBehaviourFromStartup<T>(string resourcePath) where T : ScriptBehaviour
    {
        UnityEngine.Object resourceObject = ResourceLoader.LoadFromResources(resourcePath);
        GameObject gameObject = GameObject.Instantiate(resourceObject) as GameObject;
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(gameObject);
        return behaviour;
    }


    public ScriptBehaviour LoadScriptBehaviourFromResource(string scriptName)
    {
        return LoadScriptBehaviourFromResource<ScriptBehaviour>(scriptName);
    }


    public T LoadScriptBehaviourFromResource<T>(string resourcePath) where T :ScriptBehaviour
    {
		GameObject gameObject = ResourceLoader.Instantiate(resourcePath);
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(gameObject);
        return behaviour;
    }


    public ScriptBehaviour LoadScriptBehaviour(GameObject scriptGameObject)
    {
        return LoadScriptBehaviour<ScriptBehaviour>(scriptGameObject);
    }


    public  T LoadScriptBehaviour<T>(GameObject scriptGameObject) where T : ScriptBehaviour
    {
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(scriptGameObject);
        return behaviour;
    }


    public T WrapperScriptBehaviour<T>(GameObject gameObject, string scriptResourcePath = "", string scriptName=null) where T : ScriptBehaviour
    {
        GameObject resourceGo = null;
        if (string.IsNullOrEmpty(scriptResourcePath) == false)
        {
            resourceGo = gameObject;
        }
        else
        {
            resourceGo = GameObjectUtility.Find(scriptResourcePath, gameObject);
        }
        
        if (string.IsNullOrEmpty(scriptName) == false)
        {
            PrefabBinder prefabBinder = GameObjectUtility.GetIfNotAdd<PrefabBinder>(resourceGo);
            prefabBinder.scriptPath = scriptName;
        }
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(resourceGo);
        return behaviour;
    }


    //用来给LUA绑定， 直接返回table
    public LuaTable WrapperWindowControl(GameObject gameObject, string scriptName=null)
    {
        SupportWindowControl windowControl = WrapperScriptBehaviour<SupportWindowControl>(gameObject, null, scriptName);
        return windowControl.LuaTable;
    }


    public T LoadAndWrapperScriptBehaviour<T>(string resourcePath, string scriptName = null) where T : ScriptBehaviour
    {
        GameObject gameObject = ResourceLoader.Instantiate(resourcePath);
        return WrapperScriptBehaviour<T>(gameObject, null, scriptName);
    }


    public new static ScriptManager GetInstance()
    {
        return Instance;
    }
}

