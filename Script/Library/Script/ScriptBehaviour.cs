// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptBehaviour.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections;
using System.IO;
using UnityEngine;


[CustomLuaClass]
public class ScriptBehaviour : ScriptMessageHandler
{

    protected PrefabBinder binder;
    private LuaTable luaTable;

    private LuaFunction awakeFunc;
    private LuaFunction startFunc;
    private LuaFunction updateFunc;
    private LuaFunction fixedUpdateFunc;
    private LuaFunction lateUpdateFunc;
    private LuaFunction onEnableFunc;
    private LuaFunction onDisableFunc;
    private LuaFunction OnDestroyFunc;


    protected GameObject _gameObject;
    protected Transform _transform;

    public LuaTable LuaTable
    {
        get { return luaTable; }
    }


    public void SetPrefabBinder(PrefabBinder prefabBinder)
    {
        binder = prefabBinder;

        if (binder != null)
        {
            binder.Initialize();
            binder.AddButtonListener(ClickButtonDelegete);
            LoadScript();
        }
    }


    public Object Find(string name, bool isThrow=true)
    {
        if (binder == null)
            return null;

        return binder.Find(name, isThrow);
    }


    public System.Object FindConstant(string name)
    {
        if (binder == null)
            return null;

        return binder.FindConstant(name);
    }


    private void LoadScript()
    {
        if (string.IsNullOrEmpty(binder.scriptPath))
            return;

        NewLuaObject();

        RegistLuaVariable();
        CacheLuaFunction();
    }


    protected virtual void NewLuaObject()
    {
        try
        {
            string className = Path.GetFileNameWithoutExtension(binder.scriptPath);
            className = className.Substring(0, 1).ToUpper() + className.Substring(1);

            LuaTable clazz = ScriptManager.Instance.Env[className] as LuaTable;

#if UNITY_EDITOR
            if (clazz != null)
            {
                ScriptManager.Instance.Env[className] = null;
                clazz = null;
            }
#endif

            if (clazz == null)
            {
                ScriptManager.Instance.DoFile(binder.scriptPath);
            }
            clazz = ScriptManager.Instance.Env[className] as LuaTable;

            luaTable = ScriptHelper.CallFunction(clazz, "new", false) as LuaTable;
        }
        catch (System.Exception e)
        {
            FastLuaUtility.Traceback();
            Debug.LogError("load lua file failure : " + binder.scriptPath);
        }
    }


    protected virtual void CacheMonoObject()
    {
        _gameObject = gameObject;
        _transform = transform;
    }


    protected virtual void CacheLuaFunction()
    {
        awakeFunc = ScriptHelper.GetFunction(luaTable, "awake");
        startFunc = ScriptHelper.GetFunction(luaTable, "start");
        updateFunc = ScriptHelper.GetFunction(luaTable, "update");
        fixedUpdateFunc = ScriptHelper.GetFunction(luaTable, "fixedUpdate");
        lateUpdateFunc = ScriptHelper.GetFunction(luaTable, "lateUpdate");
        onEnableFunc = ScriptHelper.GetFunction(luaTable, "onEnable");
        onDisableFunc = ScriptHelper.GetFunction(luaTable, "onDisable");
        OnDestroyFunc = ScriptHelper.GetFunction(luaTable, "onDestroy");
    }


    protected virtual void RegistLuaVariable()
    {
        LuaTable["this"] = this;
        LuaTable["transform"] = transform;
        LuaTable["gameObject"] = gameObject;
        LuaTable["varCache"] = binder.LuaVariableCache;
        LuaTable["constCache"] = binder.LuaKeyValueCache;
    }


    public void ClickButtonDelegete(GameObject go)
    {
        UIWidgetContainer button = GameObjectUtility.FindAndGet<UIButton>("", go);
        if(button == null)
            button = GameObjectUtility.FindAndGet<UIToggle>("", go);
        string btnName = binder.FindReverseButton(button);
        ScriptHelper.CallFunction(luaTable, "clickButton", luaTable, go, btnName);
    }


    protected virtual void Awake()
    {
        CacheMonoObject();

        binder = gameObject.GetComponent<PrefabBinder>();
        SetPrefabBinder(binder);

        if (awakeFunc != null)
            awakeFunc.call(LuaTable);
    }


    protected virtual void Start()
    {
        if (startFunc != null)
            startFunc.call(LuaTable);
    }


    protected virtual void Update()
    {
        if (updateFunc != null)
            updateFunc.call(LuaTable);
    }


    protected virtual void FixedUpdate()
    {
        if (fixedUpdateFunc != null)
            fixedUpdateFunc.call(LuaTable);
    }


    protected virtual void LateUpdate()
    {
        if (lateUpdateFunc != null)
            lateUpdateFunc.call(LuaTable);
    }


    protected virtual void OnEnable()
    {
        if (onEnableFunc != null)
            onEnableFunc.call(LuaTable);
    }


    protected virtual void OnDisable()
    {
        if (onDisableFunc != null)
            onDisableFunc.call(LuaTable);
    }


    protected virtual void OnDestroy()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc.call(LuaTable);
    }


    protected void RunCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        StartCoroutine(DoCoroutine(instruction, luaFunction, arguments));
    }


    protected void CancelCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        StopCoroutine(DoCoroutine(instruction, luaFunction, arguments));
    }


    private IEnumerator DoCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        yield return instruction;
        if (arguments != null)
        {
            luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
        }
    }


    protected void LuaInvoke(float delayTime, LuaFunction luaFunction, params object[] arguments)
    {
        StartCoroutine(DoInvoke(delayTime, luaFunction, arguments));
    }


    private IEnumerator DoInvoke(float delayTime, LuaFunction luaFunction, params object[] arguments)
    {
        yield return new WaitForSeconds(delayTime);

        if (arguments != null)
        {
            luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
        }
    }


    protected void CallMethod(string functionName, params object[] arguments)
    {
        ScriptManager.Instance.CallMethod(LuaTable, functionName, arguments);
    }


    public virtual void Dispose()
    {
        binder.RemoveButtonListener(ClickButtonDelegete);
    }
}

