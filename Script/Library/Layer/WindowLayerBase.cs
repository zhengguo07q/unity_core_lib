// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowLayerBase.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public abstract class WindowLayerBase : ScriptBehaviour
{
    public abstract void Initialize();


    public new GameObject gameObject { get { return _gameObject; } }


    public new Transform transform { get { return _transform; } }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        Dispose();
    }


    public abstract void Dispose();
}


[CustomLuaClass]
public class WindowLayerBaseImpl : WindowLayerBase
{
    public override void Dispose()
    {

    }

    public override void Initialize()
    {

    }
}
