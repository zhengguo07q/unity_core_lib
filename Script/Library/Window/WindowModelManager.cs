// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowModelManager.cs
//  Creator 	:  
//  Date		: 2016-7-8
//  Comment		: 需要自己配的有， 灯光，模型, 渲染贴图， 传递UITexture进来， 同一时间段UI层可使用的渲染贴图是有限制的， 一般默认使用最后4层
//                模型一般比较大， 所以模型需要异步加载, 模型本身是可以更换的, 可以通过继承WindowModel来进行
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public abstract class WindowModel : WindowControl
{
    public int layerDefinition;
    public string resourcePath;
    protected GameObject resourceGo;

    public GameObject modeHolder;
    protected Camera modelCamera;
    protected RenderTexture targetTextrue;
    public UITexture modelTexture;


    public override void Initialize()
    {
        modeHolder = transform.Find("ModelRoot").gameObject;
        TransformUtility.SetLocalPositionZ(transform, 1000f);
        TransformUtility.SetLocalPositionY(modeHolder.transform, -1f);  //现在我们的模型需要定位到-1
        modelCamera = GetComponentInChildren<Camera>() as Camera;
        modelCamera.cullingMask = 1 << layerDefinition;

        targetTextrue = modelCamera.targetTexture;
        modelTexture.mainTexture = targetTextrue;

        ApplyLayer();
    }


    public void ApplyLayer()
    {
        LayerUtility.SetLayer(gameObject, layerDefinition);
        LayerUtility.ResetShader(gameObject);

        if(modelCamera!=null)
            modelCamera.cullingMask = 1 << layerDefinition;
    }


    public override void Dispose()
    {
        targetTextrue.Release();
    }
}


public class ReplaceWindowModel : WindowModel
{
    protected Asset asset;


    public void SetReplaceModelPath(string path)
    {
        resourcePath = path;
        gameObject.name = resourcePath;
        AssetLoader.Instance.AsyncLoad(resourcePath, CallbackModelResourceCompleteImpl);
    }


    protected virtual void CallbackModelResourceCompleteImpl(Asset _asset)
    {
        if (_asset == null)
            return;

        asset = _asset;
        asset.AddRef();

        CallbackModelResourceComplete(asset.mainObject);
    }


    protected virtual void CallbackModelResourceComplete(UnityEngine.Object gameobject)
    {
        GameObject modelObject = Instantiate(gameobject) as GameObject;
        GameObjectUtility.ClearChildGameObject(modeHolder, true);
        GameObjectUtility.AddGameObject(modeHolder, modelObject);

        ApplyLayer();
    }


    public void SetTextureSize(int value)
    {
        modelTexture.width = value;
        modelTexture.height = value;
    }


    public override void Dispose()
    {
        base.Dispose();

        if (asset == null)
            return;
        asset.ReleaseRef();
        asset = null;
    }
}


public class WindowModelManager
{
    public static Dictionary<int, WindowModel> windowModelList = new Dictionary<int, WindowModel>();
    public static int[] layerDefintions = {26, 27, 28, 29, 30, 31};


    public T AddModel<T>(UITexture modelTexture, int width=512, int height=512, int antiAliasing=2) where T : WindowModel
    {
        int layerId = GetUnusedLayerId();
        if (layerId == -1)
        {
            throw new Exception("too much window model :" + GetModelString());
        }

        GameObject modelGameobject = CreateModel(width, height, antiAliasing);
        GameObjectUtility.AddGameObjectFix(modelTexture.gameObject, modelGameobject);
        modelGameobject.transform.localScale = new Vector3(360, 360, 360);

        T windowModel = GameObjectUtility.GetIfNotAdd<T>(modelGameobject);
        windowModel.modelTexture = modelTexture;
        windowModel.layerDefinition = layerId;
        windowModel.Initialize();

        windowModelList.Add(layerId, windowModel);

        return windowModel;
    }


    public void RemoveModel(WindowModel model)
    {
        windowModelList.Remove(model.layerDefinition);
        GameObjectUtility.DestoryGameObject(model.gameObject);
    }


    public int GetUnusedLayerId()
    {
        WindowModel model;
        for (int i = 0; i < layerDefintions.Length; i++)
        {
            int layer = layerDefintions[i];
            if (windowModelList.TryGetValue(layer, out model) == false)
                return layer;
        }
        return -1;
    }


    public string GetModelString()
    {
        string message = "modelPath :";
        Dictionary<int, WindowModel>.Enumerator enumerator = windowModelList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, WindowModel> pair = enumerator.Current;
            message += pair.Value.resourcePath;
            message += pair.Key;
            message += "     ";
        }
        return message;
    }


    private GameObject CreateModel(int width, int height, int antiAliasing=2)
    {
        GameObject replaceModel = new GameObject("ReplaceModel");
        GameObject cameraModel = new GameObject("Camera");
        GameObject modelRoot = new GameObject("ModelRoot");
        GameObjectUtility.AddGameObject(replaceModel, cameraModel);
        GameObjectUtility.AddGameObject(replaceModel, modelRoot);

        Camera modelCamera = cameraModel.AddMissingComponent<Camera>();
        modelCamera.clearFlags = CameraClearFlags.SolidColor;
        modelCamera.orthographic = true;
        modelCamera.orthographicSize = 1;
        modelCamera.farClipPlane = 100;
        modelCamera.nearClipPlane = -100;
        modelCamera.backgroundColor = new Color(0, 0, 0, 0);
        modelCamera.orthographicSize = WindowUtility.GetAutoAdpateSize();


        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        renderTexture.name = "modelRenderTexture";
        renderTexture.antiAliasing = antiAliasing;
        modelCamera.targetTexture = renderTexture;

        return replaceModel;
    }


    private static readonly WindowModelManager Instance = new WindowModelManager();
    public static WindowModelManager GetInstance()
    {
        return Instance;
    }
}