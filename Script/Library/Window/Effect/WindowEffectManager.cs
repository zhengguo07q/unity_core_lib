// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowEffectManager.cs
//  Creator 	:  
//  Date		: 2015-2-2
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public enum UIEffectType
{
    uetCloseNull,
    uetOpen,
    uetOpenNull,
    uetOpenSecond,
}


public class WindowEffectManager
{
    public static readonly WindowEffectManager Instance = new WindowEffectManager();
     

    private WindowEffectManager(){}


    public WindowEffect GetWindowEffect(WindowBase windowBase, UIEffectType effectType)
    {
        GameObject gameObject = windowBase.gameObject;
        WindowEffect effect;
        switch (effectType)
        {
            case UIEffectType.uetOpen:
                effect = GameObjectUtility.GetIfNotAdd<WindowOpenScaleEffect>(gameObject);
                break;
            case UIEffectType.uetOpenNull:
                effect = GameObjectUtility.GetIfNotAdd<WindowOpenNullEffect>(gameObject);
                break;
            case UIEffectType.uetOpenSecond:
                effect = GameObjectUtility.GetIfNotAdd<WindowOpenSecondScaleEffect>(gameObject);
                break;
            case UIEffectType.uetCloseNull:
                effect = GameObjectUtility.GetIfNotAdd<WindowCloseNullEffect>(gameObject);
                break;
            default:
                effect = GameObjectUtility.GetIfNotAdd<WindowEffect>(gameObject);
                break;
        }

        effect.Window = windowBase;
        return effect;
    }
}

