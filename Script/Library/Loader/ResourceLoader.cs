// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ResourceLoader.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class ResourceLoader
{
    public static Logger logger = LoggerFactory.Instance.GetLogger(typeof(ResourceLoader));

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        return Load(path) as T;
    }

    public static UnityEngine.Object Load(string path)
    {
        Asset asset = AssetLoader.Instance.SyncLoad(path);
        if (asset != null)
            return asset.mainObject;
        return null;
    }

    public static string LoadText(string path)
    {
        Asset asset = AssetLoader.Instance.SyncLoad(path);
        if (asset == null)
            return string.Empty;

        asset.AddRef();
        string ret = (asset.mainObject as TextAsset).text;
        asset.ReleaseRef();
        return ret;
    }

    public static byte[] LoadByte(string path)
    {
        Asset asset = AssetLoader.Instance.SyncLoad(path);
        if (asset == null)
            return null;

        asset.AddRef();
        byte[] ret = (asset.mainObject as TextAsset).bytes;
        asset.ReleaseRef();
        //AssetLoader.Instance.ClearAsset(path);
        return ret;
    }

    public static UnityEngine.Object LoadFromResources(string path)
    {
        return UnityEngine.Resources.Load(path);
    }

    public static GameObject Instantiate(string path)
    {
		return Instantiate(path, null, null);
    }

	public static GameObject Instantiate(string path, Transform parent)
	{
		return Instantiate(path, null, parent);
	}

	public static GameObject Instantiate(string path, GameObject parent)
	{
		return Instantiate(path, parent, null);
	}

	private static GameObject Instantiate(string path, GameObject parent, Transform parentTrans)
	{
		Asset asset = AssetLoader.Instance.SyncLoad(path);
        if(asset == null)
            return null;

		GameObject obj = (GameObject)GameObject.Instantiate(asset.mainObject);
		GameObjectProxy.Add(obj, asset);

		if(parent != null)
		{
			obj.transform.parent = parent.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.transform.localRotation = Quaternion.identity;
		}
		else
		{
			if(parentTrans != null)
			{
				obj.transform.parent = parentTrans;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				obj.transform.localRotation = Quaternion.identity;
			}
		}
		return obj;
	}
}

