// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetLoader.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;
using System;
using SLua;


public class AssetLoadTask
{
    public string path;
    public List<Action<Asset>> callbackList = new List<Action<Asset>>();
    public List<Action<UnityEngine.Object>> callbackList2 = new List<Action<UnityEngine.Object>>();
    public float progress;
    public WWW www;
    public AssetBundle assetBundle;
    public AssetBundleCreateRequest bundleCreateRequest;
    public LoadState state;

    public string failReason = string.Empty;

    public enum LoadState
    {
        NotStartLoad,
        WWWStart,
        Success, 
        Failed,
    }

    public void Update()
    {
        if (state == LoadState.NotStartLoad)
        {
            string readPath = AssetUtil.GetAssetWWWPath(path);
            state = LoadState.WWWStart;
            www = new WWW(readPath);
        }
        else if (state == LoadState.WWWStart)
        {
            if (www.isDone)
            {
                if (www.error != null)
                {
                    state = LoadState.Failed;
                    failReason = www.error;
                    Debug.LogError("progress:" + progress + " path:" + path + " failed:" + failReason);
                }
                else
                {
                   	WWWLoadDoneDeal();                
                }
            }
            else
            {
                this.progress = www.progress;
            }
        }
    }


    private void WWWLoadDoneDeal()
    {
        state = LoadState.Success;
        progress = 1;
        if (AssetLoader.Instance.ContainAsset(path))
        {
            state = LoadState.Failed;
            failReason = "asset already exsits";
            return;
        }

        assetBundle = www.assetBundle;
        if (assetBundle == null)
        {
            state = LoadState.Failed;
            failReason = "error cannot not create assetbundle";
        }
    }


    public bool IsDone()
    {
        if (state == LoadState.Success || state == LoadState.Failed)
        {
            return true;
        }

        return false;
    }


    public LoadState GetLoadState()
    {
        return state;
    }


    public void Add(Action<Asset> callback)
    {
        callbackList.Add(callback);
    }


    public void Add(Action<UnityEngine.Object> callback)
    {
        callbackList2.Add(callback);
    }


    public static AssetLoadTask Find(List<AssetLoadTask> loadList, string path)
    {
        for(int i = 0;i < loadList.Count;i++)
        {
            if(loadList[i].path == path)
            {
                return loadList[i];
            }
        }
        return null;
    }
}


public class AssetJudge
{
    private static List<string> neverDestroyList;
    public static  List<string> NeverDestroyList
    {
        get
        {
            if(neverDestroyList == null)
            {
                neverDestroyList = new List<string>();
                neverDestroyList.Add("Atlas/Common/CommonV7BgAtlas");
                neverDestroyList.Add("Atlas/Common/CommonV7BtnAtlas");
                neverDestroyList.Add("Atlas/Common/CommonV7IconAtlas");
                neverDestroyList.Add("Atlas/Common/CommonAtlasV7Bg2");
                neverDestroyList.Add("UI/Reconnect/ReConnectPanel");
                neverDestroyList.Add("UI/Tooltip/TooltipPanel");
                neverDestroyList.Add("UI/AlertTip/AlertTipPanel");
                neverDestroyList.Add("Prefabs/Common/WindowMask");
            }
            return neverDestroyList;
        }
    }

    private static List<string> sceneTypes;

    public static List<string> SceneTypes
    {
        get
        {
            if (sceneTypes == null)
            {
                sceneTypes = new List<string>();
                sceneTypes.Add("Maps/Scenes");
                sceneTypes.Add("Maps/Extand/Maps");
            }
            return sceneTypes;
        }
    }


    public static bool IsScene(string path)
    {
        string directory = path.Substring(0, path.LastIndexOf('/'));
        if (SceneTypes.Contains(directory))
        {
            return true;
        }
        return false;
    }


    private static List<string> onceList;
    public static List<string> OnceList
    {
        get
        {
            if (onceList == null)
            {
                onceList = new List<string>();
                onceList.Add("Texture/bigmap2");
            }
            return onceList;
        }
    }


    public static bool IsOnceList(string path)
    {
        string directory = path.Substring(0, path.LastIndexOf('/'));
        if (OnceList.Contains(directory))
        {
            return true;
        }
        return false;
    }


    public static AssetGCType GetGCType(string path)
    {
        if (NeverDestroyList.Contains(path))  //常驻内存 的
        {
            return AssetGCType.actForever;
        }
        if (IsOnceList(path))
        {
            return AssetGCType.actOnce;
        }
        if (IsScene(path))
        {
            return AssetGCType.actCustom;
        }
        return AssetGCType.actRefCounted;
    }
}


[CustomLuaClass]
public class AssetLoader : SingletonMono<AssetLoader> {
 
    AssetList assetCacheList = new AssetList();

    List<AssetLoadTask> loadTaskList = new List<AssetLoadTask>();

    private Logger _logger = null;

    private Logger logger
    {
    	get
    	{
    		if(_logger == null)
    		{
				_logger = LoggerFactory.Instance.GetLogger(typeof(AssetLoader));
    		}
    		return _logger;
    	}
    }

    private float loadTime = 0;


    public override void Initialize()
    {
        base.Initialize();
    }


	void Update()
	{
        LoadingCheck();
	}


	void OnGUI()
	{
	//	if(GUI.Button(new Rect(Screen.width/2, 0, 100, 50), "Test"))
	//	{
    //        string str = System.Text.Encoding.ASCII.GetString( ResourceLoader.LoadByte("Texture/configxml") );
    //        Debug.LogError(str);
	//	}
	}


    private void LoadingCheck()
    {
        for (int i = 0; i < loadTaskList.Count; )
        {
            loadTaskList[i].Update();
            if (loadTaskList[i].IsDone())
            {
                AssetLoadTask loadTask = loadTaskList[i];
                loadTaskList.RemoveAt(i);

                if(loadTask.GetLoadState() == AssetLoadTask.LoadState.Success)
                {
                    DealLoadTaskDone(loadTask);
                }
                else
                {
                    Asset asset = assetCacheList.Get(loadTask.path);
                    if(asset != null) //同步已加载成功
                    {
                        LoadTaskCallback(loadTask, asset);
                    }
                }
            }
            else
            {
                i++;
            }
        }
    }


    public bool ContainAsset(string path)
    {
        if(assetCacheList.Exsits(path))
        {
            return true;
        }
        return false;
    }


	public Asset GetAsset(string path)
    {
        return this.assetCacheList.Get(path);
    }
	
	
    private void DealLoadTaskDone(AssetLoadTask loadTask)
    {
        if (loadTask.GetLoadState() == AssetLoadTask.LoadState.Failed)
        {
            logger.Error("file read failed, " + loadTask.path + " error:" + loadTask.failReason);
            return;
        }

        string assetName = loadTask.path;

        Asset asset = new Asset(assetName, loadTask.assetBundle);
        asset.Load();

        if (!assetCacheList.Exsits(assetName))
        {
            assetCacheList.PutWithoutCheck(asset);
        }
        else
        {
            //异步加载未完成，已经有同步加载此资源了
            //Debug.Log("asset already exists, name:" + asset.name);
            asset.Destroy(true); //已经存在了就放弃资源
            asset = assetCacheList.Get(assetName);
        }

        LoadTaskCallback(loadTask, asset);
    }


    private void LoadTaskCallback(AssetLoadTask loadTask, Asset asset)
    {
        SafeCallBack(loadTask.callbackList, asset);
        SafeCallBack(loadTask.callbackList2, asset.mainObject);
    }


    private void SafeCallBack(Action<Asset> callbackList, Asset asset)
    {
        if (callbackList == null)
            return;

        try
        {
            if (callbackList.Method.IsStatic == true)
                callbackList(asset);
            else
            {
                if(callbackList.Target != null && !callbackList.Target.Equals(null))
                {
                    callbackList(asset);
                }
            }
        }
        catch(Exception e)
        {
            string errormsg = "load callback wrong, method:" + callbackList.Method.Name;
            if(callbackList.Target != null)
            {
                errormsg += ", target:" + callbackList.Target.ToString();
            }
            errormsg += ", errormsg:" + e.Message;
            errormsg += ", stack:" + e.StackTrace;
            logger.Error(errormsg);
        }
    }


    private void SafeCallBack(Action<UnityEngine.Object> callbackList, UnityEngine.Object asset)
    {
        if (callbackList == null)
            return;

        try
        {
            if (callbackList.Method.IsStatic == true)
                callbackList(asset);
            else
            {
                if (callbackList.Target != null && !callbackList.Target.Equals(null))
                {
                    callbackList(asset);
                }
            }
        }
        catch (Exception e)
        {
            string errormsg = "load callback wrong, method:" + callbackList.Method.Name;
            if (callbackList.Target != null)
            {
                errormsg += ", target:" + callbackList.Target.ToString();
            }
            errormsg += ", errormsg:" + e.Message;
            errormsg += ", stack:" + e.StackTrace;
            logger.Error(errormsg);
        }
    }


    private void SafeCallBack(List<Action<Asset>> callbackList, Asset asset)
    {
        if (callbackList == null)
            return;

        for(int i = 0;i < callbackList.Count;i++)
        {
            SafeCallBack(callbackList[i], asset);
        }
    }


    private void SafeCallBack(List<Action<UnityEngine.Object>> callbackList, UnityEngine.Object asset)
    {
        if (callbackList == null)
            return;

        for (int i = 0; i < callbackList.Count; i++)
        {
            SafeCallBack(callbackList[i], asset);
        }
    }


    public AssetLoadTask AsyncLoad(string name)
    {
        return AsyncLoad(name, null, null);
    }


    public AssetLoadTask AsyncLoad(string name, Action<Asset> callback)
    {
        return AsyncLoad(name, callback, null);
    }


    private AssetLoadTask AsyncLoad(string name, Action<UnityEngine.Object> callback)
    {
        return AsyncLoad(name, null, callback);
    }


    private AssetLoadTask AsyncLoad(string name, Action<Asset> callbackAsset, Action<UnityEngine.Object> callbackObject)
    {
        Asset asset = assetCacheList.Get(name);

        if (asset == null) //缓存区无此资源
        {
            AssetLoadTask loadTask = AssetLoadTask.Find(loadTaskList, name);
            if (loadTask == null)
            {
                loadTask = new AssetLoadTask();
                loadTask.path = name;
                loadTask.Add(callbackAsset);
                loadTask.Add(callbackObject);
                loadTaskList.Add(loadTask);
            }
            else
            {
                loadTask.Add(callbackAsset);
            }
            return loadTask;
        }
        else
        {
            SafeCallBack(callbackAsset, asset);
            SafeCallBack(callbackObject, asset.mainObject);
            return null;
        }
    }


    public Asset SyncLoad(string name)
    {
        Asset asset = assetCacheList.Get(name);
        if (asset != null)
            return asset;

        string path = AssetUtil.GetAssetAbsolutePath(name);

        
        if (!FileUtility.IsFileExist(path))
        {
            Debug.LogError("文件不存在，路径：" + path);
            return null;
        }

        byte[] data = FileUtility.GetBytes(path);
        if (data != null && data.Length > 0)
        {
            AssetUtil.DecryptData(data);
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(data);
            if (assetBundle == null)
            {
                throw new Exception("assetbundle 解析出错 " + name);
            }

            Asset newAsset = new Asset(name, assetBundle);
            newAsset.Load();

            this.assetCacheList.PutWithoutCheck(newAsset);

            return newAsset;
        }
        else
        {
            throw new Exception("文件内容有错" + name);
        }
        
    }


    public void ClearAsset(string name, bool isClear=true)
    {
        List<Asset> list = this.assetCacheList.assetList;
        for (int i = 0; i < list.Count; )
        {
            Asset asset = list[i];

            if (asset.name == name)
            {
                list.RemoveAt(i);
                asset.Destroy(isClear);
            }
            else
            {
                i++;
            }
        }
    }


    public void ClearAssets()
    {
        List<Asset> list = this.assetCacheList.assetList;
        for(int i = 0;i < list.Count;)
        {
            Asset asset = list[i];
            if (asset.gcType == AssetGCType.actForever)
            {
                i++;
                continue;
            }

            if (asset.IsUnused())
            {
                if (asset.gcType == AssetGCType.actRefCounted || asset.gcType == AssetGCType.actCustom)
                {
                    asset.Destroy(true);
                }
                else if (asset.gcType == AssetGCType.actOnce)
                {
                    asset.Destroy(false);
                }
                list.RemoveAt(i);
            }
            i++;
        }
    }


    public new static AssetLoader GetInstance()
    {
        return Instance;
    }
}
