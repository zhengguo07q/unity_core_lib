// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowQueue.cs
//  Creator 	: 
//  Date		: 2016-9-11
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using UnityEngine;


public enum WindowSourceType
{
    wstGangShop,
    wstPirateShop,
    wstBigSail,
    wstBigSailCity,
    wstCruciataPanel,
    wstBigSailPanel,
    wstConquestPanel,
    wstCardBagPanel,
    wstEscortPanel,
    wstScoopDiamondPanel,
    wstPyramidMapPanel,
    wstColosseumPanel,
    wstVipShopPanel,
    wstCruciataShadowPanel,
    wstVipPanel,
    wstActivityPanel,
    wstGangPanel,
    wstStorePanel,
    wstHouseShopPanel,
    wstCitySweepPanel,
}


public enum WindowEventType 
{                         
    wetWindow,
    wetScene,
}


public interface WindowSrcBase
{
    void FlushToFront();
}


public class Output_gotoData
{

}


public interface WindowDestBase
{
    void SetGotoData(Output_gotoData gotoData);
}


public class WindowQueuePath
{
    public WindowEventType EventType; //执行的事件类型
    public string pathName;    //执行的事件参数
    public WindowRes window;
    public bool lastWindow;
    public bool IsGotoData;     //是否需要设置跳转数据
    public SceneType NextSceneDefine;
    public SceneType LastSceneDefine;
    public bool IsAuto;
}


public class WindowQueue
{
    public Queue<WindowQueuePath> CacheQueuePathList { set; get; }
    public Queue<WindowQueuePath> QueuePathList;   //必须要执行的事件
    public List<WindowQueuePath> ConditionList { set; get; }    //关闭时候的条件
    public Output_gotoData GotoData { set; get; }

    private bool isStartExecute = false;
    private bool isExecuted = false;

    public WindowQueue()
    {
        CacheQueuePathList = new Queue<WindowQueuePath>();
        QueuePathList = new Queue<WindowQueuePath>();
        ConditionList = new List<WindowQueuePath>();
    }


    public void AddQueuePath(WindowQueuePath queuePath)
    {
        CacheQueuePathList.Enqueue(queuePath);
    }


    public void AddConditionPath(WindowQueuePath queuePath)
    {
        ConditionList.Add(queuePath);
    }

    public bool IsExecute()
    {
        return isStartExecute;
    }


    public void Reset()
    {
        isStartExecute = true;
        isExecuted = false;
        QueuePathList.Clear();
        CollectionUtility.CloneQueue<Queue<WindowQueuePath>, WindowQueuePath>(CacheQueuePathList, ref QueuePathList);
    }


    public WindowQueuePath GetNextPath()
    {
        return QueuePathList.Peek();
    }


    //检查上一个执行路径是否完成
    public bool CheckLastPath(WindowEventType eventType, WindowEvent windowEvt= WindowEvent.weNull, WindowRes windowRes = null, SceneEvent sceneEvt = SceneEvent.seNull, SceneBase sceneBase = null)
    {
        if (QueuePathList.Count < 1)
            return false;

        WindowQueuePath lastQueuePath = QueuePathList.Peek();

        if (lastQueuePath.EventType == WindowEventType.wetWindow && windowEvt == WindowEvent.weClose && lastQueuePath.window == windowRes)  //上一个窗口载入完成
        {
            return true;
        }
        else if(lastQueuePath.EventType == WindowEventType.wetScene && sceneEvt == SceneEvent.seInto && lastQueuePath.NextSceneDefine == sceneBase.CurrentSceneResource.sceneType)    //上一个场景载入完成
        {
            return true;
        }

        return false;
    }


    //执行下一个路径
    public void ExecuteNextPath()
    {
        if (isExecuted)
        {
            QueuePathList.Dequeue();
        }

        WindowQueuePath nextQueuePath = null;

        if (QueuePathList.Count >= 1)
        {
            nextQueuePath = QueuePathList.Peek();
        }

        if (nextQueuePath != null)
        {
            if (nextQueuePath.EventType == WindowEventType.wetWindow)  //上一个窗口载入完成
            {
                WindowBase window = WindowStack.Instance.OpenWindow(nextQueuePath.window);
                WindowDestBase windowDest = window as WindowDestBase;
                if (nextQueuePath.IsGotoData == true && windowDest != null && GotoData != null)
                {
                    windowDest.SetGotoData(GotoData);
                }
            }
            else if (nextQueuePath.EventType == WindowEventType.wetScene)    //上一个场景载入完成
            {
           //     WindowQueueManager.Instance.SceneEventController.EnterScene(nextQueuePath.NextSceneDefine);
            }
        }

        isExecuted = true;
    }


    //检查当前的条件
    public void CheckConditionList(WindowEventType eventType, WindowEvent windowEvt = WindowEvent.weNull, WindowRes windowRes = null , SceneEvent sceneEvt = SceneEvent.seNull, SceneBase sceneBase = null)
    {
        WindowQueuePath queuePath;
        for (int i = 0; i < ConditionList.Count; i++)
        {
            queuePath = ConditionList[i];
            if (queuePath.EventType == eventType && queuePath.EventType == WindowEventType.wetScene)
            {
                if (sceneEvt == SceneEvent.seInto && queuePath.NextSceneDefine == sceneBase.CurrentSceneResource.sceneType && queuePath.LastSceneDefine == sceneBase.LastSceneResource.sceneType)
                {
                    WindowQueueManager.Instance.ResetGoto();
                }
            }
            else if (queuePath.EventType == eventType && queuePath.EventType == WindowEventType.wetWindow)
            {
                if (queuePath.window == windowRes && windowEvt == WindowEvent.weClose)
                {
                    WindowQueueManager.Instance.ResetGoto();
                }
            }
        }
    }
}


public class WindowQueueListener : WindowListener , SceneListener
{

    public void Execute(WindowEvent evt, WindowBase window)
    {
        WindowQueue windowQueue = WindowQueueManager.Instance.CurrentWindowQueue;
        if (windowQueue == null || windowQueue.IsExecute() == false)
            return;

        if (windowQueue.CheckLastPath(WindowEventType.wetWindow, evt, window.GetWindowRes()))
        {
            windowQueue.ExecuteNextPath();
        }
        windowQueue.CheckConditionList(WindowEventType.wetWindow, evt, window.GetWindowRes());
    }


    public void Execute(SceneEvent sceneEvt, SceneBase sceneBase)
    {
        WindowQueue windowQueue = WindowQueueManager.Instance.CurrentWindowQueue;
        if (windowQueue == null || windowQueue.IsExecute() == false)
            return;

        if(windowQueue.CheckLastPath(WindowEventType.wetScene, WindowEvent.weNull, null, sceneEvt, sceneBase))
        {
            windowQueue.ExecuteNextPath();
        }
        windowQueue.CheckConditionList(WindowEventType.wetScene, WindowEvent.weNull, null, sceneEvt, sceneBase);
    }

}


public class WindowQueueCache
{
    private Dictionary<WindowRes, WindowBase> cacheWindowBaseDict = new Dictionary<WindowRes, WindowBase>();
    public List<WindowObjectContainer> cacheBaseChildList = new List<WindowObjectContainer>();
    private List<WindowBase> cacheSortList = new List<WindowBase>();
    private List<DomamolDialogBase> cacheDomamolDialogList = new List<DomamolDialogBase>();
    private WindowBase cacheWindowBase;

    private DomamolDialogBase cacheLastDomamoDialog;
    private int invalidLayer = 25; //这个层是失效的， 不渲染
    private int nguiLayer = LayerMask.NameToLayer("NGUI");


    public void PushCurrentWindow()
    {
        ClearLastCache();

        cacheWindowBase = WindowStack.Instance.CurrentTopWindow;

        Dictionary<WindowRes, WindowBase> srcCollect = WindowStack.Instance.GetBaseDict();
        CollectionUtility.CloneDictionary<Dictionary<WindowRes, WindowBase>, WindowRes, WindowBase>(srcCollect, ref cacheWindowBaseDict);
        srcCollect.Clear();

        CollectionUtility.CloneList<List<WindowObjectContainer>, WindowObjectContainer>(WindowObjectContainer.BaseChildList, ref cacheBaseChildList);
        WindowObjectContainer.BaseChildList.Clear();

        List<WindowBase> sortList = WindowStack.Instance.GetSorter().GetSortList();
        CollectionUtility.CloneList<List<WindowBase>, WindowBase>(sortList, ref cacheSortList);
        sortList.Clear();

        cacheLastDomamoDialog = DomamolDialogBase.LastDomamoDialog;
        DomamolDialogBase.LastDomamoDialog = null;

        CollectionUtility.CloneList<List<DomamolDialogBase>, DomamolDialogBase>(DomamolDialogBase.DomamolDialogList, ref cacheDomamolDialogList);
        DomamolDialogBase.DomamolDialogList.Clear();

        Dictionary<WindowRes, WindowBase>.Enumerator cloneEnumerator = cacheWindowBaseDict.GetEnumerator();
        while (cloneEnumerator.MoveNext())
        {
            GameObject windowGo = cloneEnumerator.Current.Value.gameObject;

            GameObjectUtility.AddGameObject(WindowBase.windowBackRoot, windowGo);
            LayerUtility.SetLayer(windowGo, invalidLayer);
        }

        
    }


    public void PullLastWindow()
    {
        WindowStack.Instance.CurrentTopWindow = cacheWindowBase;

        Dictionary<WindowRes, WindowBase> srcCollect = WindowStack.Instance.GetBaseDict();
        CollectionUtility.CloneDictionary<Dictionary<WindowRes, WindowBase>, WindowRes, WindowBase>(cacheWindowBaseDict, ref srcCollect);

        CollectionUtility.CloneList<List<WindowObjectContainer>, WindowObjectContainer>(cacheBaseChildList, ref WindowObjectContainer.BaseChildList);

        List<WindowBase> sortList = WindowStack.Instance.GetSorter().GetSortList();
        CollectionUtility.CloneList<List<WindowBase>, WindowBase>(cacheSortList, ref sortList);

        cacheLastDomamoDialog = DomamolDialogBase.LastDomamoDialog;
        

        CollectionUtility.CloneList<List<DomamolDialogBase>, DomamolDialogBase>(cacheDomamolDialogList, ref DomamolDialogBase.DomamolDialogList);
        cacheDomamolDialogList.Clear();

        Dictionary<WindowRes, WindowBase>.Enumerator cloneEnumerator = cacheWindowBaseDict.GetEnumerator();

        while (cloneEnumerator.MoveNext())
        {
            GameObject windowGo = cloneEnumerator.Current.Value.gameObject;

            GameObjectUtility.AddGameObject(WindowBase.WindowRoot, windowGo);
            LayerUtility.SetLayer(windowGo, nguiLayer);

        }

        //出现同帧设置不生效，需要延迟一帧避免
  //      TimerTaskManager.Instance.Register(ApplyWindowModelLayer, 0.05f); 

        ClearLastCache();
    }


    private void ApplyWindowModelLayer()
    {
        Dictionary<int, WindowModel>.Enumerator modelEnumerator = WindowModelManager.windowModelList.GetEnumerator();
        while (modelEnumerator.MoveNext())
        {
            modelEnumerator.Current.Value.ApplyLayer();
        }
    }


    private void ClearLastCache()
    {
        cacheWindowBaseDict.Clear();
        cacheBaseChildList.Clear();
        cacheSortList.Clear();
        cacheDomamolDialogList.Clear();
        cacheWindowBase = null;
        cacheDomamolDialogList.Clear();
        cacheLastDomamoDialog = null;
    }


    public void ClearCacheAndWindow()
    {
        List<GameObject> backGoList = GameObjectUtility.GetChildGameObject(WindowBase.windowBackRoot);
        for (int i = 0; i < backGoList.Count; i++)
        {
            GameObject backGo = backGoList[i];
            if (backGo == null)
                continue;
            GameObjectUtility.DestoryGameObject(backGo);
        }

        ClearLastCache();
    }
}


public class WindowQueueManager
{
    protected Dictionary<WindowSourceType, WindowQueue> windowSourceQueue = new Dictionary<WindowSourceType, WindowQueue>();

    public WindowQueue CurrentWindowQueue { set; get; }
    public WindowQueueCache queueCache;
    protected WindowQueueListener queueListener;

    public SceneEventBase SceneEventController { set; get; }


    public void Initialize()
    {
        queueListener = new WindowQueueListener();
        queueCache = new WindowQueueCache();

        WindowStack.Instance.AddListener(queueListener);
        SceneEventController.AddListener(queueListener);
    }


    public void Goto(WindowSourceType sourceType, Output_gotoData gotoData=null)
    {
        if (CurrentWindowQueue != null)
        {
            throw new Exception("Current wondiw queue exists!");
        }
        WindowQueue windowQueue;
        if (windowSourceQueue.TryGetValue(sourceType, out windowQueue) == false)
        {
            throw new Exception("Not register window source type!");
        }

        queueCache.PushCurrentWindow();

        CurrentWindowQueue = windowQueue;
        CurrentWindowQueue.Reset();
        CurrentWindowQueue.GotoData = gotoData;
        CurrentWindowQueue.ExecuteNextPath();
    }


    public void ResetGoto()
    {
        queueCache.PullLastWindow();

        WindowBase window = WindowStack.Instance.CurrentTopWindow;
        DomamolDialogBase.AddMask();
        if (window != null)
        {
            WindowSrcBase windowBack = window as WindowSrcBase;
            if (windowBack != null)
            {
                windowBack.FlushToFront();
            }
        }
        CurrentWindowQueue = null;
    }


    public void Clear()
    {
        queueCache.ClearCacheAndWindow();
    }


    public void Register(WindowSourceType sourceType, WindowQueue windowQueue)
    {
        if (windowSourceQueue.ContainsKey(sourceType))
        {
            throw new Exception("ready register window source type!");
        }
        windowSourceQueue.Add(sourceType, windowQueue);
    }


    public readonly static WindowQueueManager Instance = new WindowQueueManager();
}
