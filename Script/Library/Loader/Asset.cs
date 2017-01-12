// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: Asset.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public enum AssetGCType
{
    actForever, //常驻内存
    actRefCounted, //引用计数
    actOnce,
    actCustom,
}


public class Asset
{
    public AssetBundle assetBundle;
    public float createTime;
    public AssetGCType gcType;
    public Object mainObject;

    public string name;     //路径加实际名字
    private int refCount;   //引用计数
    private bool refUsed = false;


    public Asset(string name, AssetBundle assetBundle)
    {
        this.name = name;
        this.assetBundle = assetBundle;
        this.createTime = Time.time;
        this.gcType = AssetJudge.GetGCType(this.name);
    }


    public string GetShortName()
    {
        return Path.GetFileNameWithoutExtension(name);
    }


    public void Load()
    {
        LoadMainObject();
    }


    public void LoadMainObject()
    {
        if (AssetJudge.IsScene(this.name))
        {
            return;
        }
        this.mainObject = assetBundle.mainAsset;

        if (this.mainObject == null)
        {
            string contentName = Path.GetFileNameWithoutExtension(name);
            this.mainObject = assetBundle.LoadAsset(contentName);
        }

#if UNITY_EDITOR
        GameObject mainGo = this.mainObject as GameObject;
        if (mainGo != null)
        {
            LayerUtility.ResetShader(mainGo);
        }
#endif
    }


    public T GetFromObject<T>() where T : Component
    {
        GameObject gObj = this.mainObject as GameObject;
        T ret = null;
        if (gObj != null)
        {
            ret = gObj.GetComponent<T>();
        }
        return ret;
    }


    public void AddRef()
    {
    	refUsed = true;
        refCount++;
    }


    public void ReleaseRef()
    {
        refCount--;
        if (refCount < 0)
        {
            Debug.LogError("资源引用减1出错, refcount < 0, 资源："+ name);
        }
    }


    public bool IsUnused()
    {
        if (gcType == AssetGCType.actOnce && refCount == 1)
            return true;
        else if (gcType == AssetGCType.actCustom)
            return IsUnusedScene();
        if (refCount > 0)
            return false;
        return true;
    }


    private bool IsUnusedScene()
    {
        bool unused = true;
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (scene.name == GetShortName())
                unused = false;
        }
        return unused;
    }


    public bool IsRefUsed()
    {
		return refUsed;
    }


    public void Destroy(bool isClear)
    {
        if (assetBundle != null)
        {
			assetBundle.Unload(isClear);
            assetBundle = null;
        }
    }


    public override string ToString()
    {
        return name + " " + refCount + "  ";
    }
}


public class AssetList
{
    public List<Asset> assetList = new List<Asset>();

    public Asset Get(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        for (int i = 0; i < assetList.Count; i++)
        {
            if (assetList[i].name == name)
            {
                return assetList[i];
            }
        }

        return null;
    }


    public bool Exsits(string name)
    {
        Asset asset = Get(name);
        if (asset == null) return false;
        return true;
    }


    public Asset Put(Asset asset)
    {
        Asset asset2 = Get(asset.name);
        if (asset2 == null)
        {
            assetList.Add(asset);
            return asset;
        }
        else
        {
            Debug.LogError("asset already exists, name:" + asset.name);
            return asset2;
        }
    }


    public void PutWithoutCheck(Asset asset)
    {
        assetList.Add(asset);
    }


    public void PrintDebug()
    {
        StringBuilder desc = new StringBuilder(); ;
        for (int i = 0; i < assetList.Count; i++)
        {
            Asset asset = assetList[i];
            desc.Append(asset.ToString());
        }
        //Debug.LogError("AssetList Count : " + assetList.Count + "    List :  " + desc.ToString());
    }


    public void PrintNeverUseRef()
    {
		StringBuilder desc = new StringBuilder();
        int count = 0;
        for (int i = 0; i < assetList.Count; i++)
        {
            Asset asset = assetList[i];
            if(asset.gcType == AssetGCType.actRefCounted)
            {
            	if(!asset.IsRefUsed())
            	{
                    count++;
                    desc.Append(asset.ToString());
            	}
            }
        }
        Debug.LogError("never user refcount : " + count + "    List :  " + desc.ToString());
    }

	
    public void PrintRefNotZero()
    {
        StringBuilder desc = new StringBuilder();
        int count = 0;
        for (int i = 0; i < assetList.Count; i++)
        {
            Asset asset = assetList[i];
            if (asset.gcType == AssetGCType.actRefCounted)
            {
                if (!asset.IsUnused())
                {
                    count++;
                    desc.Append(asset.ToString());
                }
            }
        }
        Debug.LogError("used assets count : " + count + "    List :  " + desc.ToString());
    }
}