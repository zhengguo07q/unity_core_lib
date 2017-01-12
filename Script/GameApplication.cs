// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: GameApplication.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class GameApplication : MonoBehaviour
{
    private GameProgress progeressLayer;


    private void Start ()
    {
        InitializeRoot();
        InitializeVisualeObject();
        Startup();
    }


    private void InitializeRoot()
    {
        GameObject uiRoot = GameObject.Find("UI Root") as GameObject;
        UnityEngine.GameObject.DontDestroyOnLoad(uiRoot);

        LayerHolder.BuildHolder(uiRoot);
    }


    private void InitializeVisualeObject()
    {
        WindowLayerBaseImpl windowLayer = WindowLayer.BuilderInStartup<WindowLayerBaseImpl>("UI/Startup/StartupBackgroundPanel", WindowLayerDefinition.wldBackgroundLayer);
        GameObjectUtility.AddGameObject(LayerHolder.GetLayerRootObject(WindowLayerDefinition.wldBackgroundLayer), windowLayer.gameObject);

        progeressLayer = WindowLayer.BuilderInStartup<GameProgress>("UI/Startup/StartupProgressPanel", WindowLayerDefinition.wldLoadingLayer);
        GameObjectUtility.AddGameObject(LayerHolder.GetLayerRootObject(WindowLayerDefinition.wldLoadingLayer), progeressLayer.gameObject);
        progeressLayer.Initialize();
    }


    private void Startup()
    {
        PathUtility.Initialize();
        ScriptManager.GetInstance().OnScriptTick += InitializeLuaTick;
        ScriptManager.GetInstance().OnScriptComplete += InitializeLuaComplete;
        ScriptManager.GetInstance().Startup();
    }


    private void InitializeLuaTick(int progress)
    {
        progeressLayer.SetProgressTxt("启动脚本引擎", "", 0f);
    }


    private void InitializeLuaComplete()
    {
        ScriptManager.GetInstance().DoStart("application.lua");
    }
}
