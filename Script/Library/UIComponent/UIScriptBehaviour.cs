// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIScriptBehaviour.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using SLua;


[CustomLuaClass]
public class UIScriptBehaviour : WindowControl
{
    protected bool isStart = false;
    protected bool isInvalidate = false;

    protected object mData;
    protected override void Start()
    {
        base.Start();
        isStart = true;
        enabled = false;
        //如果需要更新界面数据，马上更新
        Initialize();
        Validate();
    }


    protected virtual void CommitData()
    {

    }


    public virtual void Validate()
    {
        if (isInvalidate)
        {
            isInvalidate = false;
            CommitData();
        }
    }


    public virtual void InvalidNow()
    {
        //已经初始化完成，直接更新数据
        if (isStart)
        {
            CommitData();
            return;
        }

        //需要更新数据
        isInvalidate = true;
    }


    public virtual object Data
    {
        set
        {
            mData = value;
            InvalidNow();
        }
        get
        {
            return mData;
        }
    }
}

