// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowLayer.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class WindowLayer
{

    public static void Apply(GameObject go, WindowLayerDefinition layerDefintion )
    {
        int layerNumber = (int)layerDefintion;
        NGUIUtility.AdjustmentPanelDepth(go, layerNumber);
    }


    public static LuaTable Builder(string scriptResourcePath, WindowLayerDefinition definition)
    {
        WindowLayerBaseImpl scriptBehaviour = ScriptManager.Instance.LoadScriptBehaviourFromResource<WindowLayerBaseImpl>(scriptResourcePath);
        Apply(scriptBehaviour.gameObject, definition);
        return scriptBehaviour.LuaTable;
    }


    public static T Builder<T>(string scriptResourcePath, WindowLayerDefinition definition) where T : ScriptBehaviour
    {
        T scriptBehaviour = ScriptManager.Instance.LoadScriptBehaviourFromResource<T>(scriptResourcePath);
        Apply(scriptBehaviour.gameObject, definition);
        return scriptBehaviour;
    }


    public static T Builder<T>(GameObject scriptGameObject, WindowLayerDefinition definition) where T : ScriptBehaviour
    {
        T scriptBehaviour = ScriptManager.Instance.LoadScriptBehaviour<T>(scriptGameObject);
        Apply(scriptBehaviour.gameObject, definition);
        return scriptBehaviour;
    }


    public static T BuilderInStartup<T>(string scriptResourcePath, WindowLayerDefinition definition) where T : ScriptBehaviour
    {
        T scriptBehaviour = ScriptManager.Instance.LoadScriptBehaviourFromStartup<T>(scriptResourcePath);
        Apply(scriptBehaviour.gameObject, definition);
        return scriptBehaviour;
    }
}

