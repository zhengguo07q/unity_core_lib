// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ListLoader.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: List Or Queue ?
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class ListLoader :MonoBehaviour
{
    public static Logger log = LoggerFactory.GetInstance().GetLogger(typeof(ListLoader));
    private List<string> waitList;
    private List<AssetLoadTask> loadTaskList;
    private int loadTotal = 0;
    private int loadCount = 0;
    private float combProgress;
    private string lastWait;
    private bool isStart = false;
    private List<Asset> assetList = new List<Asset>();
    private Action<string> callbackLoadComplete;
 

    public static ListLoader CreateInstance(GameObject parent)
    {
        ListLoader loader = GameObjectUtility.FindAndAddChild<ListLoader>("", parent, "ListLoader");
        loader.waitList = new List<string>();
        loader.loadTaskList = new List<AssetLoadTask>();
        return loader;
    }


    public void SetCallback(Action<string> _callbackLoadComplete)
    {
        callbackLoadComplete = _callbackLoadComplete;
        isStart = false;
        lastWait = null;
        loadCount = loadTotal = 0;

    }


    public void PutWaitLoad(string waitLoadResource)
    {
        waitList.Add(waitLoadResource);
    }


    public void Load()
    {
        log.Debug("启动装载: " + waitList.Count);
        if (waitList.Count == 0 && callbackLoadComplete != null)
        {
            CallbackLastResource(null);
            return;
        }
    
        loadTotal = loadCount = waitList.Count;
        for (int i = 0; i < waitList.Count; i++)
        {
            AssetLoadTask loadTask = AssetLoader.Instance.AsyncLoad(waitList[i]);
            if (loadTask == null)
            {
                loadCount--;
            }
            else
            {
                loadTaskList.Add(loadTask);
            }
            if (i == waitList.Count - 1)
            {
                lastWait = waitList[i];
            }
        }
        waitList.Clear();
        isStart = true;
    }


    void Update()
    {
        if (isStart == false)
            return;

        float currentCombProgress = 0;

        List<AssetLoadTask> removeTaskList = new List<AssetLoadTask>();
        for (int i = 0; i < loadTaskList.Count; i++)
        {
            AssetLoadTask loadTask = loadTaskList[i];
            if (loadTask.IsDone())
            {
                AssetLoadTask.LoadState loadState = loadTask.GetLoadState();
                if (loadState == AssetLoadTask.LoadState.Success)
                {
                    Asset asset = AssetLoader.Instance.GetAsset(loadTask.path);
                    asset.AddRef();
                    assetList.Add(asset);
                    log.Debug("加载完成资源: " + loadTask.path);
                }
                else
                {
                    log.Error("加载完成资源失败: " + loadTask.path + " 失败原因:" + loadTask.failReason);
                }
                loadCount--;
                removeTaskList.Add(loadTask);
            }
            else
            {
                currentCombProgress += loadTask.progress * 1 / loadTotal;
            }
        }

        for (int j = 0; j < removeTaskList.Count; j++)
        {
            loadTaskList.Remove(removeTaskList[j]);
        }

        combProgress = (loadTotal - loadCount)*1.0f / loadTotal + currentCombProgress;

        if (loadCount == 0 && callbackLoadComplete != null)
        {
            if (lastWait != null)
            {
                isStart = false;
                combProgress = 1;
                log.Debug("载入完成所有资源, 启动完成 : " + lastWait);
                CallbackLastResource(lastWait);
            }
        }
    }


    public float Progress
    {
        get{ return combProgress; }
    }


    public void CallbackLastResource(string path)
    {
        callbackLoadComplete(path);
    }


    public void Dispose()
    {
        if(assetList != null)
        {
            for (int i = 0; i < assetList.Count; i++)
            {
                Asset asset = assetList[i];
                asset.ReleaseRef();
            }
            assetList = null;
        }
        callbackLoadComplete = null;
    }
}

