// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: TextureProxy.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class TextureProxy : MonoBehaviour
{
    public string texName;
    public bool isAsyncLoad = true;
    private Asset asset;
    UITexture tex;


    UITexture Tex
    {
        get
        {
            if(tex == null)
            {
                tex = GetComponent<UITexture>();
            }
            return tex;
        }
    }


    void Awake()
    {
        SetTexture();
    }


    void OnDestroy()
    {
        if (asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
    }


    public void SetTextureName(string texname)
    {
        if (string.IsNullOrEmpty(texname))
            return;

        if (texname == this.texName)
            return;

        if (asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
        this.texName = texname;
        SetTexture();
    }


    void SetTexture()
    {
        if (string.IsNullOrEmpty(texName))
            return;

        if (!isAsyncLoad)
        {
            LoadDone(AssetLoader.Instance.SyncLoad(texName));
        }
        else
        {
            AssetLoader.Instance.AsyncLoad(texName, LoadDone);
        }
    }


    void LoadDone(Asset asset)
    {
        if (asset == null)
            return;

        if (asset.name != this.texName)
            return;

        this.Tex.mainTexture = (Texture)asset.mainObject;
        asset.AddRef();
        this.asset = asset;
    }


    public static TextureProxy GetProxy(UITexture uiTex)
    {
        if (uiTex == null) return null;
        TextureProxy texProxy = uiTex.GetComponent<TextureProxy>();
        if(texProxy == null)
        {
            texProxy = uiTex.gameObject.AddComponent<TextureProxy>();
        }
        return texProxy;
    }


    public static TextureProxy GetProxy(GameObject obj)
    {
        UITexture tex = obj.GetComponent<UITexture>();
        if (tex == null) return null;
        return GetProxy(tex);
    }


    public static void SetProxyTexture(GameObject obj, string texName)
    {
        UITexture tex = obj.GetComponent<UITexture>();
        if (tex == null) return;
        SetProxyTexture(tex, texName);
    }


    public static void SetProxyTexture(UITexture tex, string texName)
    {
        TextureProxy proxy = GetProxy(tex);
        proxy.SetTextureName(texName);
    }


    public static void SetTextureSync(UITexture tex, string texName)
    {
        TextureProxy proxy = GetProxy(tex);
        proxy.isAsyncLoad = false;
        proxy.SetTextureName(texName);
    }
}