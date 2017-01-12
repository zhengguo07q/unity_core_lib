// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SceneInstance.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SceneInstance : WindowControl, SceneBase
{
    public SceneResource LastSceneResource { get; set; }
    public SceneResource CurrentSceneResource { get; set; }
    public ListLoader LoadLoader { set; get; }


    public override void Initialize()
    {
        base.Initialize();
        LoadLoader = ListLoader.CreateInstance(gameObject);
        PutLoadResource();
        LoadLoader.SetCallback(CallbackLoadLoaderComplete);
        LoadLoader.Load();
    }


    protected virtual void PutLoadResource()
    {

    }


    protected void CallbackLoadLoaderComplete(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            SupportWindowControl windowControl = ScriptManager.Instance.WrapperScriptBehaviour<SupportWindowControl>(gameObject, null,  CurrentSceneResource.loadScriptPath);
            windowControl.LuaTable["loginInstance"] = this;
            windowControl.Initialize();
        }
        else
        {
            SupportWindowControl windowControl = ScriptManager.Instance.LoadAndWrapperScriptBehaviour<SupportWindowControl>(resourceName, CurrentSceneResource.loadScriptPath);
            windowControl.Initialize();
        }
    }


    public virtual void EnterScene()
    {

    }


    public virtual void AfterEnterScene()
    {
        UIPriorityManager.Instance.StartWork();
        SceneController.Instance.Notify(SceneEvent.seInto, this);
    }


    public virtual void ExitScene()
    {
        UIPriorityManager.Instance.StopWork();
    }


    public virtual void DestoryScene()
    {

    }
}

