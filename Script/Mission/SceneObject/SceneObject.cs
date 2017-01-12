// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SceneObject.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public enum SceneObjectType
{
    sotCastle           = 1,
    sotTroop            = 2,
    sotStaticObject     = 3,
    sotItem             = 4,
    sotAll              = 5,
}


public class SceneObjectRoot
{
    public GameObject ParentGameObject;

    private SceneObjectRoot()
    {
        ParentGameObject = new GameObject("SceneObjectRoot");
        GameObject.DontDestroyOnLoad(ParentGameObject);
    }

    public readonly static SceneObjectRoot Instance = new SceneObjectRoot();
}


public class SceneObject : MonoBehaviour
{
    public long Id { set; get; }

    public Vector3 Position { set; get; }

    public Vector3 Scale { set; get; }

    public bool isUpdateRenderer = false;

    public static T CreateSceneObject<T>(long id) where T : SceneObject
    {
        GameObject sceneObjectObject = new GameObject(typeof(T).Name);
        sceneObjectObject.transform.parent = SceneObjectRoot.Instance.ParentGameObject.transform;
        T instance = sceneObjectObject.AddComponent<T>();
        instance.Id = id;
        return instance;
    }
}

