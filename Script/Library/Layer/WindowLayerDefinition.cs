// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowLayerDefinition.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;
using System;
using SLua;

[CustomLuaClass]
public enum WindowLayerDefinition
{
    wldBackgroundLayer      = 10000,
    wldSceneTreasure        = 11000,
    wldSceneBigSail         = 12000,
    wldSceneRelic           = 13000,
    wldSceneCityLayer       = 14000,
    wldSceneRole            = 14100,
    wldFogOfWarLayer        = 15000,
    wldSceneCityGroupLayer  = 16000,
    wldMainUILayer          = 17000,


    wldSceneWindowLayer     = 20000,
    wldSceneEffectLayer     = 21000,
    wldExploitLayer         = 22000,
    wldUILayer              = 23000,
    wldChatSwapLayer        = 24000,    
    wldGuideLayer           = 30000,
    wldEffectLayer          = 35000,
    wldDiglogLayer          = 100000,
    wldTooltipLayer         = 110000,
    wldCloudSplashLayer     = 120000,
    wldLoadingLayer         = 130000,
    wldAlertLayer           = 140000,
    wldReconnectLayer       = 150000,

};


[CustomLuaClass]
public class LayerHolder
{
    private static Dictionary<WindowLayerDefinition, LayerHolder> holderDict = new Dictionary<WindowLayerDefinition, LayerHolder>();

    private string layerName;
    private GameObject gameObject;
    private WindowLayerDefinition layerDefinition;


    public static GameObject GetLayerRootObject(WindowLayerDefinition layerDefinition)
    {
        LayerHolder holder;
        if (holderDict.TryGetValue(layerDefinition, out holder))
        {
            return holder.gameObject;
        }
        return null;
    }


    public static void ClearLayerObject(WindowLayerDefinition layerDefinition)
    {
        LayerHolder holder;
        if (holderDict.TryGetValue(layerDefinition, out holder))
        {
            GameObject layerObject = holder.gameObject;
            GameObjectUtility.ClearChildGameObject(layerObject);
        }
    }


    public static void BuildHolder(GameObject root)
    {
        LayerHolder layerHolder;
        WindowLayerDefinition layerDefinition;

        GameObject scene2d = new GameObject("Scene2d");
        GameObjectUtility.AddGameObject(root, scene2d);

        Array layerDefs = Enum.GetValues(typeof(WindowLayerDefinition));
        for (int i = 0; i < layerDefs.Length; i++)
        {
            layerDefinition = (WindowLayerDefinition)layerDefs.GetValue(i);
            string layerName = Enum.GetName(typeof(WindowLayerDefinition), layerDefinition);

            GameObject layerGameObject = new GameObject();
            layerGameObject.name = layerName;
            GameObjectUtility.AddGameObject(scene2d, layerGameObject);

            layerHolder = new LayerHolder();
            layerHolder.gameObject = layerGameObject;
            layerHolder.layerName = layerName;
            layerHolder.layerDefinition = layerDefinition;

            holderDict.Add(layerDefinition, layerHolder);
        }
    }


    public static void SwapLayer(WindowLayerDefinition srcLayerDef, WindowLayerDefinition destLayerDef)
    {
        GameObject destGo = GetLayerRootObject(destLayerDef);
        if (GameObjectUtility.IsExistsChildGameObject(destGo) == true)
        {
            throw new Exception("Layer definition child object is not null : " + destLayerDef.ToString());
        }

        GameObject srcGo = GetLayerRootObject(srcLayerDef);
        List<GameObject> srcChildList = GameObjectUtility.GetChildGameObject(srcGo);
        GameObject srcChildGo;
        for (int i = 0; i < srcChildList.Count; i++)
        {
            srcChildGo = srcChildList[i];
            if (srcChildGo == null)
                continue;

            WindowLayer.Apply(srcChildGo, destLayerDef);
            GameObjectUtility.AddGameObject(LayerHolder.GetLayerRootObject(destLayerDef), srcChildGo);
        }
    }
}
