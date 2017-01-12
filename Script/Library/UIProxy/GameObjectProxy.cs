// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: GameObjectProxy.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class GameObjectProxy : MonoBehaviour {

    Asset asset;

    private Vector3 localPos;
    private Vector3 localScale;

    private string curPath = string.Empty;

	public static void Add(GameObject obj, Asset asset)
	{
		GameObjectProxy proxy = obj.GetComponent<GameObjectProxy>();

		if(proxy != null)
		{
			throw new System.Exception("already has gameobjectproxy");
		}

		proxy = obj.AddComponent<GameObjectProxy>();
		proxy.SetAsset(asset);
	}

	public void SetAsset(Asset asset)
	{
		this.asset = asset;
		AddRef();
	}

	void AddRef()
	{
		if(this.asset != null)
		{
			this.asset.AddRef();
		}
	}

	void ReleaseRef()
	{
		if(this.asset != null)
		{
			this.asset.ReleaseRef();
			this.asset = null;
		}
	}

	void OnDestroy()
	{
		ReleaseRef();
	}

    public void PrintAsset()
    {
        if(asset != null)
        {
            Debug.LogError(asset.name);
        }
    }
}
