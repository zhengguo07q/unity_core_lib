// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SceneController.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class SceneController : SingletonMono<SceneController>, SceneEventBase
{
    protected static Logger log;

    private List<SceneListener> sceneListeners = new List<SceneListener>();
    private SceneInstance lastSceneInstance;
    private SceneInstance sceneInstance;
    public SceneResource sceneRes;
    public WindowRes LastSceneWindow;

    public override void Initialize()
    {
        log = LoggerFactory.GetInstance().GetLogger(typeof(SceneController));
    }


    public void AddListener(SceneListener listener)
    {
        sceneListeners.Add(listener);
    }


    public void RemoveListener(SceneListener listener)
    {
        sceneListeners.Remove(listener);
    }


    public void Notify(SceneEvent evt, SceneBase instance)
    {
        for (int i = 0; i < sceneListeners.Count; i++)
        {
            SceneListener listener = sceneListeners[i];
            if (listener == null)
                continue;
            listener.Execute(evt, instance);
        }
    }


    public void EnterScene(SceneResource sceneRes)
    {
        log.Debug("EnterScene " + sceneRes.sceneType.ToString() + " name : " + sceneRes.name);

        if (sceneInstance != null)
        {
            Notify(SceneEvent.seExit, sceneInstance);
            sceneInstance.ExitScene();
            lastSceneInstance = sceneInstance;
            DelayDestoryScene();
        }
        CreateScene(sceneRes);
    }


    private void DelayDestoryScene()
    {
        if (lastSceneInstance != null)
        {
            Notify(SceneEvent.seDestory, lastSceneInstance);
            lastSceneInstance.DestoryScene();
            lastSceneInstance.Dispose();
        }

        GameObjectUtility.DestoryGameObject(lastSceneInstance.gameObject);

        AssetLoader.Instance.ClearAssets();
        Resources.UnloadUnusedAssets();
    }


    private void CreateScene(SceneResource _sceneRes)
    {
        this.sceneRes = _sceneRes;

        GameObjectUtility.ClearChildGameObject(gameObject, true);
        GameObject sceneGo = GameObjectUtility.CreateNullGameObject(gameObject, sceneRes.sceneType.ToString());

        sceneInstance = ScriptManager.Instance.WrapperScriptBehaviour<SupportSceneInstance>(sceneGo, null, sceneRes.scriptPath);
        sceneInstance.CurrentSceneResource = this.sceneRes;
        sceneInstance.Initialize();

        if (lastSceneInstance != null)
        {
            sceneInstance.LastSceneResource = lastSceneInstance.CurrentSceneResource;
        }
    }


    public SceneBase GetSceneInstance()
    {
        return sceneInstance;
    }


    public LuaTable GetSceneTable()
    {
        if (sceneInstance != null)
        {
            return sceneInstance.LuaTable;
        }
        return null;
    }


    public new static SceneController GetInstance()
    {
        return Instance;
    }
}