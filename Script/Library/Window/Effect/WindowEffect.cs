// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowEffect.cs
//  Creator 	:  
//  Date		: 2015-2-2
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class WindowEffect : MonoBehaviour
{
    public WindowBase Window { set; get; }


    public virtual void Initialize()
    {}


    public virtual void Execute()
    {}


    public virtual void Complete()
    {}
}


public class WindowCloseNullEffect : WindowEffect
{

    public override void Execute()
    {
        base.Execute();
        WindowStack.Instance.OnCloseWindow(Window);
    }


    public override void Complete()
    {
        base.Complete();
    }
}


public class WindowOpenNullEffect : WindowEffect
{
    public override void Execute()
    {
        base.Execute();
        Complete();
    }


    public override void Complete()
    {
        base.Complete();
        WindowStack.Instance.OnOpenWindow(Window);
    }

}


public class WindowOpenScaleEffect : WindowEffect
{
    public float duration = 0.3f;


    public override void Execute()
    {
        base.Execute();
        //TweenAlpha tweenAlpha = GameObjectUtility.GetIfNotAdd<TweenAlpha>(gameObject);
        //tweenAlpha.from = 0.5f;
        //tweenAlpha.to = 1;
        //tweenAlpha.method = UITweener.Method.Linear;
        //tweenAlpha.style = UITweener.Style.Once;
        //tweenAlpha.duration = duration;
        //tweenAlpha.SetOnFinished(Complete);
        Complete();
    }


    public override void Complete()
    {
        base.Complete();
        WindowStack.Instance.OnOpenWindow(Window);
    }
}


public class WindowOpenSecondScaleEffect : WindowEffect
{
    private Vector3 upVector3 = new Vector3(0, 200);
    public Vector3 originalPosition;


    public override void Initialize()
    {
        base.Initialize();
        //originalPosition = transform.localPosition;                 //默认位置
        //transform.localScale = new Vector3(0.2f, 0.2f);             //本地缩放
    }


    public override void Execute()
    {
        base.Execute();

        FastLuaUtility.PlayAudio("10003");

        //TweenPosition.Begin(gameObject, 0.2f, originalPosition);
        //TweenScale tweenScale = TweenScale.Begin(gameObject, 0.2f, Vector3.one);
        //tweenScale.SetOnFinished(Complete);
        Complete();
    }


    public override void Complete()
    {
        base.Complete();

        WindowStack.Instance.OnOpenWindow(Window);
    }
}
