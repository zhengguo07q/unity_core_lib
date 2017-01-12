// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: GameObjectUtility.cs
//  Creator 	:  
//  Date		: 2016-3-11
//  Comment		: 
// ***************************************************************

using SLua;
using System;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class GameObjectUtility
{

    public static T FindAndGet<T>(string path, GameObject parentGo=null) where T  : MonoBehaviour
    {
        GameObject bindGo = Find(path, parentGo);
        if (bindGo == null)
        {
            throw new Exception("gameobject utility find error , path : " + path);
        }
        return bindGo.GetComponent<T>();
    }


    public static T FindAndAdd<T>(string path, GameObject parentGo = null) where T : MonoBehaviour
    {
        GameObject bindGo = Find(path, parentGo);
        if (bindGo == null)
        {
            throw new Exception("gameobject utility find error , path : " + path);
        }
        return bindGo.AddComponent<T>();
    }


    public static T GetIfNotAdd<T>(GameObject bindGo) where T : MonoBehaviour
    {
        T component = bindGo.GetComponent<T>();
        if(component == null)
            component = bindGo.AddComponent<T>();
        return component;
    }


    public static GameObject Find(string path, GameObject parentGo = null, bool isThrow=true)
    {
        GameObject go = null;
        if (parentGo == null)
        {
            go = GameObject.Find(path);
        }
        else
        {
            Transform trans = parentGo.transform.Find(path);
            if (trans != null)
            {
                return trans.gameObject;
            }
        }
        if (isThrow == true && go == null)
        {
            throw new Exception("gameobject utility find error , path : " + path);
        }
        return go;
    }


    public static T FindAndAddChild<T>(string path, GameObject parentGo = null, string childName=null) where T : MonoBehaviour
    {
        GameObject bindGo = Find(path, parentGo);
        if (bindGo == null)
        {
            throw new Exception("gameobject utility find error , path : " + path);
        }
        GameObject childGo = new GameObject();
        if (childName != null)
        {
            childGo.name = childName;
        }
        childGo.transform.parent = bindGo.transform;
        childGo.transform.localPosition = Vector3.zero;
        childGo.transform.localScale = Vector3.one;
        childGo.transform.localRotation = Quaternion.identity;
        return childGo.AddComponent<T>();
    }


    public static GameObject AddGameObject(GameObject parentGo, GameObject childGo)
    {
        childGo.transform.parent = parentGo.transform;
        childGo.transform.localPosition = Vector3.zero;
        childGo.transform.localScale = Vector3.one;
        childGo.transform.localRotation = Quaternion.identity;
        return childGo;
    }


    static public GameObject AddGameObjectPrefab(GameObject parent, GameObject prefab)
    {
        GameObject go;
        if (prefab.activeSelf == true)
        {
            go = GameObject.Instantiate(prefab) as GameObject;
        }
        else
        {
            prefab.SetActive(true);
            go = GameObject.Instantiate(prefab) as GameObject;
            prefab.SetActive(false);
        }
        
        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }


    public static GameObject AddGameObjectFix(GameObject parentGo, GameObject childGo)
    {
        childGo.transform.parent = parentGo.transform;
        return childGo;
    }


    public static T CreateGameObject<T>(string name=null) where T : MonoBehaviour
    {
        GameObject go = new GameObject();
        if (string.IsNullOrEmpty(name) == false)
        {
            go.name = name;
        } 
        T t = go.AddComponent<T>();
        return t;
    }


    public static GameObject CreateNullGameObject(GameObject parentGo, string name=null)
    {
        GameObject go = new GameObject();
        if (string.IsNullOrEmpty(name) == false)
        {
            go.name = name;
        }
        go.transform.parent = parentGo.transform;
        return go;
    }


    public static void RemoveGameObject(GameObject childGo)
    {
        childGo.transform.parent = null;
    }


    public static void DestoryGameObject(GameObject go, bool isImmediate = false)
    {
        if (isImmediate == false)
        {
            UnityEngine.Object.DestroyObject(go);
        }
        else
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }


    public static void ClearChildGameObject(GameObject parentGo, bool isImmediate=false)
    {
        Transform parentTrans = parentGo.transform;
        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Transform childTrans = parentTrans.GetChild(i);
            DestoryGameObject(childTrans.gameObject, isImmediate);
        }
    }


    public static bool IsExistsChildGameObject(GameObject parentGo)
    {
        if (parentGo.transform.childCount != 0)
            return true;
        return false;
    }


    public static List<GameObject> GetChildGameObject(GameObject parentGo)
    {
        List<GameObject> childList = new List<GameObject>();
        Transform trans;
        for (int i = 0; i < parentGo.transform.childCount; i++)
        {
            trans = parentGo.transform.GetChild(i);
            childList.Add(trans.gameObject);
        }
        return childList;
    }
}

