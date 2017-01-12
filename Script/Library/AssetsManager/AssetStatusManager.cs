// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetConfDownload.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 状态机流程：
//                  1,载入上一次没有处理完的临时配置文件， 放入到临时数据里面
//                  2,载入包体里的配置文件，载入缓存中的配置文件，对比这两个配置文件的版本号， 找出比较新的放入到local缓存配置里
//                  3,下载version，确定是否需要更新最新的配置文件
//                  4,下载project, 保存为temp, 但是内容放在remote, 确定需要下载的asset列表
//                  5,上一次还没有处理完成的临时的资源文件，如果不比远程的最新的低，就处理上一次的临时的文件， 不然则处理最新的资源, 并把远程的赋予给临时
//                  6,下载下来的资源，成功则设置临时资源为成功，失败则放入失败列表。
//                  7,下载完成后
// ***************************************************************


using SLua;
using System.Collections.Generic;


[CustomLuaClass]
public enum AssetState
{
    asUnchecked,
    asRestart,              //重启下载
    asPreDownloadVersion,   //准备下载version
    asDownloadingVersion,   //下载version
    asLoadedVersion,        //version已经被下载
    asCheckPackage,         //检查是否需要下载包
    asPreDownloadManifest,  //预下载manifest
    asDownloadingManifest,  //正在下载manifest
    asLoadedManifest,       //manifest已经被下载
    asPreUpdate,            //预处理更新
    asNeedUpdate,           //需要更新
    asAllowUpdate,          //允许更新
    asUpdating,             //正在更新
    asUpdateToData,         //更新到日期
    asUpdateFailure,        //失败的更新
    asPreDownloadFailure,   //失败检查
    asDownloadingFailure,   //下载失败
    asPreUnZip,             
    asUnziping,
    asUnziped,
};


[CustomLuaClass]
public class AssetStatusManager
{
    protected static Logger log = LoggerFactory.GetInstance().GetLogger(typeof(AssetStatusManager));

    public AssetConfProject packageConfProject;
    public AssetConfProject localConfProject = new AssetConfProject();
    public AssetConfProject remoteConfProject = new AssetConfProject();
    public AssetConfProject tempConfProject = new AssetConfProject();

    public string cacheVersionPath;
    public string cacheManifestPath;
    public string tempManifestPath;
    public string remoteManifestPath;

    protected string storagePath;

    private string urlPath;

    private AssetState updateState;

    protected AssetResultHandler resultHandler;

    private List<string> compressedFiles = new List<string>();

    private int repeatCount = 0;


    public void Initialize(string _urlPath, string manifestUrl, string _storagePath)
    {
        resultHandler = AssetResultHandler.Instance;
        urlPath = _urlPath;
        storagePath = _storagePath;
        AssetUtility.MakeSureDirectory(ref storagePath);
        cacheVersionPath = storagePath + AssetConstants.VERSION_FILENAME;
        cacheManifestPath = storagePath + AssetConstants.MANIFEST_FILENAME;
        tempManifestPath = storagePath + AssetConstants.TEMP_MANIFEST_FILENAME;
        remoteManifestPath = storagePath + AssetConstants.Manifest_Filename_Remote;
        LoadLocalManifest(manifestUrl);
        LoadTempManifest();
    }


    public AssetState UpdateState
    {
        set
        {
            updateState = value;
            log.Debug("Next UpdateState : " + updateState.ToString());
            switch (updateState)
            {
                case AssetState.asUnchecked:
                    break;
                case AssetState.asPreDownloadVersion:
                    CheckVersion();
                    break;
                case AssetState.asDownloadingVersion:
                    DownloadVersion();
                    break;
                case AssetState.asLoadedVersion:
                    ParseVersion();
                    break;
                case AssetState.asCheckPackage:
                    CheckPackageUpdate();
                    break;
                case AssetState.asPreDownloadManifest:
                    CheckManifest();
                    break;
                case AssetState.asDownloadingManifest:
                    DownloadManifest();
                    break;
                case AssetState.asLoadedManifest:
                    ParseManifest();
                    break;
                case AssetState.asPreUpdate:
                    resultHandler.DispatchUpdateEvent(AssetEventCode.aecNewVersionFound);
                    CheckAssetList();
                    break;
                case AssetState.asNeedUpdate: //检查允许更新
                    resultHandler.DispatchUpdateEvent(AssetEventCode.aecCheckAllowUpdate);
                    break;
                case AssetState.asAllowUpdate:
                    DownloadAssetList();
                    break;
                case AssetState.asUpdating:
                    break;
                case AssetState.asUpdateToData:
                    resultHandler.DispatchUpdateEvent(AssetEventCode.aecAlreadyUpToDate);
                    UpdateState = AssetState.asPreUnZip;
                    break;
                case AssetState.asUpdateFailure:
                    break;  
                case AssetState.asPreDownloadFailure:
                    CheckUpdateFailure();
                    break;
                case AssetState.asDownloadingFailure:
                    DownloadFailureAssets();
                    break;
                case AssetState.asPreUnZip:
                    PrepareZipFile();
                    break;
                case AssetState.asUnziping:
                    DecompressZipFile();
                    break;
                case AssetState.asUnziped:
                    AssetDownloadComplete();
                    break;
            }
        }
        get
        {
            return updateState;
        }
    }


    protected void LoadTempManifest()
    {
        tempConfProject.LoadAndParseFile(tempManifestPath);
        if (!tempConfProject.IsLoaded())
        {
            FileUtility.DeleteFile(tempManifestPath);
        }
    }


    protected void LoadLocalManifest(string manifestUrl)
    {
        AssetConfProject cachedManifest = null;
        
        //载入缓存的配置
        if (FileUtility.IsFileExist(cacheManifestPath))
        {
            log.Debug("load cache manifest file : " + cacheManifestPath);
            cachedManifest = new AssetConfProject();
            cachedManifest.LoadAndParseFile(cacheManifestPath);

            if (!cachedManifest.IsLoaded())
            {
                log.Debug("Parse error cacheManifestPath : " + cacheManifestPath);
                FileUtility.DeleteFile(cacheManifestPath);
                cachedManifest = null;
            }
        }

        //载入本地的配置
        log.Debug("Load local manifest in app package" + manifestUrl);
        localConfProject.LoadAndParseFile(manifestUrl);
        if (localConfProject.IsLoaded())
        {
            packageConfProject = localConfProject;
            if (cachedManifest != null)
            {
                if (localConfProject.ConfVersionInfo.resourcePatch > cachedManifest.ConfVersionInfo.resourcePatch) //检查当前缓存版本是否低于包体资源版本
                {
                    log.Debug("Remove storagePath rease :  " + cachedManifest.ConfVersionInfo.resourcePatch);
                    // Recreate storage, to empty the content
                    FileUtility.DeleteDirectory(storagePath);
                    FileUtility.CreateDirectory(storagePath);
                    cachedManifest = null;
                }
                else
                {
                    localConfProject = cachedManifest;
                }
            }
            PrepareLocalManifest();
        }

        if (!localConfProject.IsLoaded())
        {
            UnityEngine.Debug.Log("AssetsManagerEx : No local manifest file found error.\n");
            resultHandler.DispatchUpdateEvent(AssetEventCode.aceErrorNoLocalManifest);
        }
    }
    

    protected void PrepareLocalManifest()
    {
        localConfProject.PrependSearchPaths();
        UpdateState = AssetState.asPreDownloadVersion;
    }


    private void CheckVersion()
    {
        if (repeatCount < AssetConstants.Max_Repeat_Count_Version)
        {
            repeatCount++;
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecVersionNotice, repeatCount.ToString());
            UpdateState = AssetState.asDownloadingVersion;
        }
        else
        {
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecVersionDownloadError);
            UpdateState = AssetState.asRestart;
        }
    }


    protected void DownloadVersion()
    {
        string versionUrl = urlPath + localConfProject.packageUrl + localConfProject.remoteVersionUrl;

        if (!string.IsNullOrEmpty(versionUrl))
        {
            resultHandler.downloader.DownloadAsync(versionUrl, cacheVersionPath, AssetConstants.VERSION_ID);
        }
    }


    protected void ParseVersion()
    {
        repeatCount = 0;
        remoteConfProject.LoadAndParseFile(cacheVersionPath);

        if (!remoteConfProject.IsLoaded())
        {
            log.Debug("AssetsManagerEx : Fail to parse version file, step skipped\n");//这里要改
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecVersionParseError);
            UpdateState = AssetState.asPreDownloadManifest;
        }
        else
        {
            if (localConfProject.ConfVersionInfo.IsUpdatePackage(remoteConfProject.ConfVersionInfo))
            {
                UpdateState = AssetState.asCheckPackage;
            }
            else if (localConfProject.ConfVersionInfo.IsUpdateResource(remoteConfProject.ConfVersionInfo))
            {
                UpdateState = AssetState.asPreDownloadManifest;
            }
            else
            {
                UpdateState = AssetState.asUpdateToData;
            }
        }
    }


    private void CheckPackageUpdate()
    {
        resultHandler.DispatchUpdateEvent(AssetEventCode.aceErrorPackage, remoteConfProject.packageBodyURI);
        UpdateState = AssetState.asRestart;
    }


    private void CheckManifest()
    {
        if (repeatCount < AssetConstants.Max_Repeat_Count_Manifest)
        {
            repeatCount++;
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecManifestNotice, repeatCount.ToString());
            UpdateState = AssetState.asDownloadingManifest;
        }
        else
        {
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecErrorDownloadManifest);
            UpdateState = AssetState.asRestart;
        }
    }


    protected void DownloadManifest()
    {
        string manifestUrl = urlPath + localConfProject.packageUrl + localConfProject.remoteManifestUrl;
        if (!string.IsNullOrEmpty(manifestUrl))
        {
            resultHandler.downloader.DownloadAsync(manifestUrl, remoteManifestPath, AssetConstants.MANIFEST_ID);
        }
    }


    protected void ParseManifest()
    {
        repeatCount = 0;
        remoteConfProject.LoadAndParseFile(remoteManifestPath);

        if (!remoteConfProject.IsLoaded())
        {
            log.Debug("AssetsManagerEx : Error parsing manifest file\n");
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecErrorParseManifest);
        }
        else
        {
            if (localConfProject.ConfVersionInfo.IsUpdateResource(remoteConfProject.ConfVersionInfo))
            {
                UpdateState = AssetState.asPreUpdate;
            }
            else
            {
                UpdateState = AssetState.asUpdateToData;
            }
        }
    }


    private void ValidateAssetList()
    {
        Dictionary<string, AssetConfDiff> diffMap = localConfProject.GenDiff(remoteConfProject);

    }


    protected void CheckAssetList()
    {
        compressedFiles.Clear();
        if (tempConfProject.IsLoaded() && tempConfProject.ConfVersionInfo.IsUseTempResource(remoteConfProject.ConfVersionInfo))
        {
            tempConfProject.GenResumeAssetsList(urlPath, ref resultHandler.downloadUnits, ref resultHandler.downloadedSize, ref resultHandler.totalSize);
            resultHandler.totalWaitToDownload = resultHandler.totalToDownload = resultHandler.downloadUnits.Count;
            UpdateState = AssetState.asNeedUpdate;
        }
        else
        {
            tempConfProject = remoteConfProject;
            Dictionary<string, AssetConfDiff> diffMap = localConfProject.GenDiff(remoteConfProject);
            if (diffMap.Count == 0)
            {
                UpdateState = AssetState.asUpdateToData;
            }
            else
            {
                foreach (var it in diffMap)
                {
                    AssetConfDiff diff = it.Value;

                    if (diff.type == AssetDiffType.adtDeleted)
                    {
                        FileUtility.DeleteFile(storagePath + diff.asset.path);
                    }
                    else
                    {
                        string path = diff.asset.path;
                        // Create path
                        FileUtility.CreateDirectory(storagePath + System.IO.Path.GetDirectoryName(path));
                        AssetDownloadUnit unit;
                        unit.customId = it.Key;
                        unit.srcUrl = urlPath + remoteConfProject.packageUrl + path;
                        unit.storagePath = storagePath + AssetUtility.GetNotVersionFileName(path);
                        unit.resumeDownload = false;
                        unit.downloaded = diff.asset.size;
                        resultHandler.downloadUnits.Add(unit.customId, unit);
                        if (!resultHandler.downloadedSize.ContainsKey(it.Key))
                        {
                            resultHandler.downloadedSize.Add(it.Key, 0);
                            resultHandler.totalSize += diff.asset.size;
                        }
                    }
                }

                resultHandler.totalWaitToDownload = resultHandler.totalToDownload = resultHandler.downloadUnits.Count;
                UpdateState = AssetState.asNeedUpdate;
            }
        }
    }


    private void DownloadAssetList()
    {
        UpdateState = AssetState.asUpdating;
        resultHandler.downloader.BatchDownloadAsync(storagePath, resultHandler.downloadUnits, AssetConstants.BATCH_UPDATE_ID);
    }


    public void CheckUpdateFailure()
    {
        if (resultHandler.failedUnits.Count == 0 && resultHandler.totalWaitToDownload == 0)
        {
            UpdateState = AssetState.asPreUnZip;
            return;
        }
        if (repeatCount < AssetConstants.Max_Repeat_Count_Update)
        {
            repeatCount++;
            UpdateState = AssetState.asDownloadingFailure;
        }
        else
        {
            resultHandler.DispatchUpdateEvent(AssetEventCode.aecUpdateFailure);
            UpdateState = AssetState.asRestart;
        }
    }


    public void DownloadFailureAssets()
    {
        log.Debug(string.Format("AssetsManager : Start update {0} failed assets.\n", resultHandler.failedUnits));
        Dictionary<string, AssetDownloadUnit> copyFailedUnits = new Dictionary<string, AssetDownloadUnit>(resultHandler.failedUnits);
        resultHandler.failedUnits.Clear();

        resultHandler.downloadUnits.Clear();
        resultHandler.downloadUnits = copyFailedUnits;
        resultHandler.downloader.BatchDownloadAsync(storagePath, resultHandler.downloadUnits, AssetConstants.BATCH_UPDATE_ID);
    }


    protected void PrepareZipFile()
    {
        if (remoteConfProject.Assets != null)
        {
            Dictionary<string, AssetConf>.Enumerator enumerator = remoteConfProject.Assets.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssetConf asset = enumerator.Current.Value;
                if (asset.compressed)
                {
                    compressedFiles.Add(storagePath + AssetUtility.GetNotVersionFileName(asset.path));
                }
            }
        }
        UpdateState = AssetState.asUnziping;
    }


    protected void DecompressZipFile()
    {
        for (int i = 0; i < compressedFiles.Count; i++)
        {
            string zipfile = compressedFiles[i];
            if (FileUtility.IsFileExist(zipfile) == false)
                continue;

            if (!ZipUtility.UnZip(zipfile))
            {
                resultHandler.DispatchUpdateEvent(AssetEventCode.aecErrorDecompress, "", "Unable to decompress file " + zipfile);
            }
            FileUtility.DeleteFile(zipfile);
        }
        compressedFiles.Clear();
        UpdateState = AssetState.asUnziped;
    }


    private void AssetDownloadComplete()
    {
        if (tempConfProject != null && tempConfProject.IsLoaded() == true) //如果进行了更新则使local=temp,用来做位置检测用
        {
            localConfProject = tempConfProject;
        }
        tempConfProject = null;

        resultHandler.DispatchUpdateEvent(AssetEventCode.aecUpdateFinished);
    }


    public readonly static AssetStatusManager Instance = new AssetStatusManager();
}

