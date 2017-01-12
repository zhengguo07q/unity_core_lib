// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: PathUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;
using System.Text;
using UnityEngine;


[CustomLuaClass]
public class PathUtility
{
    public static void Initialize()
    {
        StreamingAssetsPath = Application.streamingAssetsPath;
        PersistentDataPath = Application.persistentDataPath;
        LuaPath = StreamingAssetsPath + "/LuaScript";
    }


    public static string StreamingAssetsPath { set; get; }

    public static string PersistentDataPath { set; get; }

    public static string LuaPath { get; set; }


    public static string GetHierachyPath(GameObject obj)
    {
        string ret = string.Empty;
        if (obj == null)
            return ret;

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Insert(0, obj.name);

        Transform tranParent = obj.transform.parent;
        while (tranParent != null)
        {
            stringBuilder.Insert(0, tranParent.gameObject.name + "/" + ret);
            tranParent = tranParent.parent;
        }
        ret = stringBuilder.ToString();
        return ret;
    }
}

