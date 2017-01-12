using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum UIPriority
{
    /// <summary>
    /// 云雾散开
    /// </summary>
    CloudSplash,
    /// <summary>
    /// 扫荡奖励
    /// </summary>
    SweepReward ,
    /// <summary>
    /// 任务奖励
    /// </summary>
    MissionReward,
    /// <summary>
    /// 活动奖励
    /// </summary>
    ActivityReward,
    /// <summary>
    /// 升级
    /// </summary>
    RoleLevelup,
    /// <summary>
    /// 功能开放
    /// </summary>
    FunctionOpen,
    /// <summary>
    /// 镜头移动或人物行走
    /// </summary>
    CameraOrStep,
    /// <summary>
    /// 城市开放
    /// </summary>
    OpenCity,
    /// <summary>
    /// 成就奖励
    /// </summary>
    AchievementReward,
    /// <summary>
    /// 人物对白
    /// </summary>
    MissionDialogue,
    /// 推送礼包
    /// </summary>
    RechargeGift,
    /// <summary>
    /// 下一场景
    /// </summary>
    NextSceneAction,
    /// <summary>
    /// 引导
    /// </summary>
    Guide,
}

public class ActionPriority
{
    public UIPriority priority;
    public Action action;
    public GameObject go;

    public ActionPriority(UIPriority p, Action a, GameObject gameObject=null)
    {
        priority = p;
        action = a;
        go = gameObject;
    }
}

public class ActionPriorityList
{
    public static List<ActionPriority> list = new List<ActionPriority>();

    public static void Add(ActionPriority ap)
    {
        list.Add(ap);
        list.Sort(AscendingSort);
    }

    static int AscendingSort(ActionPriority a1, ActionPriority a2)
    {
        int p1 = (int)a1.priority;
        int p2 = (int)a2.priority;

        if (p1 > p2) return 1;
        if (p1 < p2) return -1;
        return 0;
    }
}

public class UIPriorityManager : SingletonMono<UIPriorityManager> 
{
    [HideInInspector]
    public bool busy;

    public ActionPriority Current;// if(Current != null && Current.priority == UIPriority.MissionDialogue)
    private List<ActionPriority> list = new List<ActionPriority>();

    public int ListCount
    {
        get
        {
            return list.Count;
        }
    }
    void Awake()
    {

        // * 如果你想表现队列在某个特定的动作完成后再进行，就这样！
    //    Messenger.AddListener(BigMapEvent.cloud_splash_finished.ToString(), StartWork);
    }


    public void Reset()
    {
        busy = false;
        list.Clear();
    }




    IEnumerator Loop()
    {
        while (true)
        {
            yield return null;


            if (!busy )
            {
                if (list.Count > 0)
                {
                    busy = true;
                    Current = list[0];
                    list[0].action.Invoke();
                    list.RemoveAt(0);
                }
                else
                {
                    Current = null;
                }
            }
        }
    }


    public void StartWork()
    {
        list = ActionPriorityList.list;
        StartCoroutine(Loop());
    }
    public void StopWork()
    {
        Current = null;
        StopAllCoroutines();
    }

    void OnDestroy()
    {
      //  Messenger.RemoveListener(BigMapEvent.cloud_splash_finished.ToString(), StartWork);
    }
}
