// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SingletonMono.cs
//  Creator 	: zg
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class SingletonObject
{
    public GameObject ParentGameObject;

    private SingletonObject()
    {
        ParentGameObject = new GameObject("Manager");
        GameObject.DontDestroyOnLoad(ParentGameObject);
    }

    public readonly static SingletonObject Instance = new SingletonObject();
}


[CustomLuaClass]
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                go.transform.parent = SingletonObject.Instance.ParentGameObject.transform;
                instance = go.AddComponent<T>();
            }
            return instance;
        }
    }


    public static T GetInstance()
    {
        return Instance;
    }


    public void Instantiate() { }


    void Awake()
    {
        Initialize();
    }


    void OnDestroy()
    {
        Destroy();
    }


    virtual public void Initialize()
    {
    }


    virtual public void Destroy()
    {
    }
}
