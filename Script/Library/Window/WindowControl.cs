// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowControl.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class WindowControl : ScriptBehaviour
{
    public virtual void Initialize()
    { }


    public new GameObject gameObject { get { return _gameObject; } }


    public new Transform transform { get { return _transform; } }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        Dispose();
    }


    public override void Dispose()
    { }
}

