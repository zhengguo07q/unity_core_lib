// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowRes.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: 
// ***************************************************************

using SLua;
using System;


[CustomLuaClass]
public class WindowRes
{
    public Type windowClazz;
    public string resourcePath;
    public UIEffectType openEffect = UIEffectType.uetOpen;
    public UIEffectType closeEffect = UIEffectType.uetCloseNull;
    public int Id;


    public WindowRes(Type _windowClazz, string _resourcePath=null, int _id=0, UIEffectType _openEffect = UIEffectType.uetOpen, UIEffectType _closeEffect = UIEffectType.uetCloseNull)
    {
        this.windowClazz = _windowClazz;
        this.resourcePath = _resourcePath;
        this.Id = _id;
        this.openEffect = _openEffect;
        this.closeEffect = _closeEffect;
    }


    public WindowRes(string _resourcePath = null, int _id = 0, UIEffectType _openEffect = UIEffectType.uetOpen, UIEffectType _closeEffect = UIEffectType.uetCloseNull)
    {
        this.windowClazz = typeof(SupportWindowScript);
        this.resourcePath = _resourcePath;
        this.Id = _id;
        this.openEffect = _openEffect;
        this.closeEffect = _closeEffect;
    }
}


