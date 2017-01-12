// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: MatTexProxy.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class MatTexProxy : MonoBehaviour
{
    private Asset[] assetArray;
    private Material[] materials;


    public static void SetTex(MeshRenderer render, int index, string path)
    {
        MatTexProxy matTexProxy = render.GetComponent<MatTexProxy>();
        if(matTexProxy == null)
        {
            matTexProxy = render.gameObject.AddComponent<MatTexProxy>();
            matTexProxy.Wrap(render);
        }
        matTexProxy.SetTexture(index, path);
    }


    public static void SetTex(SkinnedMeshRenderer render, int index, string path)
    {
        MatTexProxy matTexProxy = render.GetComponent<MatTexProxy>();
        if (matTexProxy == null)
        {
            matTexProxy = render.gameObject.AddComponent<MatTexProxy>();
            matTexProxy.Wrap(render);
        }
        matTexProxy.SetTexture(index, path);
    }


    private void Wrap(MeshRenderer render)
    {
        if (render.materials == null)
            return;
        if (render.materials.Length < 1)
            return;

        materials = render.materials;
        assetArray = new Asset[render.materials.Length];
    }


    private void Wrap(SkinnedMeshRenderer skinRender)
    {
        if (skinRender.materials == null)
            return;
        if (skinRender.materials.Length < 1)
            return;

        materials = skinRender.materials;
        assetArray = new Asset[skinRender.materials.Length];
    }


    private void SetTexture(int index, string path)
    {
        if (materials == null || materials.Length == 0)
            return;
        if (index < 0)
            return;
        if (index >= materials.Length)
            return;

        Asset at = AssetLoader.Instance.SyncLoad(path);
        if (at == null)
            return;

        Asset asset = assetArray[index];
        if(asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }

        assetArray[index] = at;
        asset = assetArray[index];
        asset.AddRef();
        materials[index].mainTexture = asset.mainObject as Texture;
    }


    void OnDestroy()
    {
        if (assetArray == null)
            return;
        for(int i = 0;i < assetArray.Length;i++)
        {
            Asset asset = assetArray[i];
            if(asset != null)
            {
                asset.ReleaseRef();
                asset = null;
            }
        }
        assetArray = null;
    }
}
