// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SceneObjectManager.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;


public class SceneObjectManager
{
    private Dictionary<long, SceneObject> sceneObjects = new Dictionary<long, SceneObject>();


    public SceneObject SpawnSceneObject(SceneObjectType sceneObjectType, int objectId)
    {
        SceneObject sceneObject = null;
        switch (sceneObjectType)
        {
            case SceneObjectType.sotCastle:
                sceneObject = SceneObject.CreateSceneObject<CastleObject>(objectId);
                break;
            case SceneObjectType.sotTroop:
                sceneObject = SceneObject.CreateSceneObject<TroopObject>(objectId);
                break;
            case SceneObjectType.sotStaticObject:
                sceneObject = SceneObject.CreateSceneObject<StaticObject>(objectId);
                break;
            case SceneObjectType.sotItem:
                sceneObject = SceneObject.CreateSceneObject<ItemObject>(objectId);
                break;
            default:
                throw new Exception("error scene object type : " + sceneObjectType.ToString() + " objectId : " + objectId);
        }

        return sceneObject;
    }


    public void FindSceneObjectById(long objectId)
    {

    }


    public void FindSceneObjectByType(SceneObjectType sceneObjectType)
    {

    }


    public void FindNesestObject()
    {

    }


    public void Foreach(SceneObjectType sceneObjectType, Func<Object> action)
    {

    }


    public void DestorySceneObject()
    {

    }


    public static readonly SceneObjectManager Instance = new SceneObjectManager();
}

