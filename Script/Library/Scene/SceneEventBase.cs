// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: IGameController.cs
//  Creator 	:  
//  Date		: 2016-10-22
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public enum SceneType
{
    sdNull,
    sdStartup,
    sdLogin,
    sdBigMap,
    sdBattle,
    sdHomestead,
};


[CustomLuaClass]
public enum SceneEvent
{
    seNull,
    seCreate,
    seExit,
    seDestory,
    seInto,
};


[CustomLuaClass]
public class SceneResource
{
    public SceneType sceneType;
    public string name;
    public string scriptPath;
    public string loadScriptPath;
    public bool isFirst;
};


public interface SceneListener
{
    void Execute(SceneEvent evt, SceneBase sceneBase);
};


public interface SceneBase
{
    SceneResource LastSceneResource { get; }
    SceneResource CurrentSceneResource { get; }
};


public interface SceneEventBase
{
    void AddListener(SceneListener listener);
    void RemoveListener(SceneListener listener);
    void EnterScene(SceneResource sceneRes);

    SceneBase GetSceneInstance();
};

