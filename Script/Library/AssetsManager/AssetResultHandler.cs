// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetResultManager.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************



using SLua;
using System.Collections.Generic;
using System.Linq;


[CustomLuaClass]
public enum AssetEventCode
{
    aceErrorPackage,
    aceErrorNoLocalManifest,
    aecVersionNotice,
    aecVersionDownloadError,
    aecVersionParseError,
    aecManifestNotice,
    aecErrorDownloadManifest,
    aecErrorParseManifest,
    aecNewVersionFound,
    aecAlreadyUpToDate,
    aecCheckAllowUpdate,
    aecUpdateProgression,
    aecAssetUpdated,
    aecErrorUpdating,
    aecUpdateFinished,
    aecUpdateFailure,
    aecErrorDecompress
};


[CustomLuaClass]
public struct AssetResultProgressItem
{
    public float percent;
    public float percentByFile;
    public string assetId;
    public int totalToDownload;
    public double totalDownloaded;
    public double totalSize;
};


[CustomLuaClass]
public class AssetResultHandler
{
    public delegate void EventAssetsManager(AssetResultHandler am, AssetEventCode code, AssetResultProgressItem item, string message);
    public EventAssetsManager onCallBack = null;

    protected static Logger log = LoggerFactory.GetInstance().GetLogger(typeof(AssetResultHandler));

    public Dictionary<string, AssetDownloadUnit> downloadUnits = new Dictionary<string, AssetDownloadUnit>();
    public Dictionary<string, AssetDownloadUnit> failedUnits = new Dictionary<string, AssetDownloadUnit>();
    public Dictionary<string, double> downloadedSize = new Dictionary<string, double>();

    public AssetStatusManager statusManager;
    public AssetDownloader downloader = new AssetDownloader();
    private AssetResultProgressItem progressItem = new AssetResultProgressItem();

    public float percent = 0;
    public float percentByFile = 0;
    public int sizeCollected = 0;
    public double totalSize = 0;
    public double totalDownloaded = 0;
    public int totalToDownload = 0;
    public int totalWaitToDownload = 0;
    public double fromSaveSize;


    public AssetResultHandler()
    {
        statusManager = AssetStatusManager.Instance;
        downloader.SetConnectionTimeout();
        downloader.onError += OnError;
        downloader.onProgress += OnProgress;
        downloader.onSuccess += OnSuccess;
    }


    public void AddCallback(EventAssetsManager callback)
    {
        onCallBack += callback;
    }


    protected virtual void OnError(AssetDownloadError error)
    {
        if (error.customId == AssetConstants.VERSION_ID)
        {
            statusManager.UpdateState = AssetState.asPreDownloadVersion;
        }
        else if (error.customId == AssetConstants.MANIFEST_ID)
        {
            statusManager.UpdateState = AssetState.asPreDownloadManifest;
        }
        else
        {
            bool unitIt = downloadUnits.ContainsKey(error.customId);
            if (unitIt)
            {
                AssetDownloadUnit unit = downloadUnits[error.customId];
                failedUnits.Add(unit.customId, unit);
            }
            DispatchUpdateEvent(AssetEventCode.aecErrorUpdating, error.customId, error.message);

            if (failedUnits.Count != 0 && failedUnits.Count == totalWaitToDownload)  //最后一个是错误下载的时候触发
            {
                statusManager.UpdateState = AssetState.asPreDownloadFailure;
            }
        }
    }


    protected virtual void OnProgress(double total, double downloaded, string url, string customId)
    {
        if (customId == AssetConstants.VERSION_ID || customId == AssetConstants.MANIFEST_ID)
        {
            percent = (float)(100 * downloaded / total);
            DispatchUpdateEvent(AssetEventCode.aecUpdateProgression, customId);
            return;
        }
        else
        {
            bool found = false;
            double totalDownloaded = 0;
            string[] keys = downloadedSize.Keys.ToArray<string>();
            foreach (var key in keys)
            {
                var val = downloadedSize[key];
                if (key == customId)
                {
                    downloadedSize[key] = downloaded;
                    val = downloaded;
                    found = true;
                }
                totalDownloaded += val;
            }
            if (!found)
            {
                statusManager.tempConfProject.SetAssetDownloadState(customId, AssetDownloadState.adsDownloading);
                downloadedSize.Add(customId, downloaded);
                totalSize += total;
                sizeCollected++;
            }
            this.totalDownloaded = totalDownloaded;
            if (statusManager.UpdateState == AssetState.asUpdating || statusManager.UpdateState == AssetState.asAllowUpdate)
            {
                float currentPercent = (float)(100 * totalDownloaded / totalSize);
                if ((int)currentPercent != (int)percent)
                {
                    percent = currentPercent;
                    DispatchUpdateEvent(AssetEventCode.aecUpdateProgression, customId);
                }   
            }
        }
    }


    protected virtual void OnSuccess(string srcUrl, string storagePath, string customId)
    {
        if (customId == AssetConstants.VERSION_ID)
        {
            statusManager.UpdateState = AssetState.asLoadedVersion;
        }
        else if (customId == AssetConstants.MANIFEST_ID)
        {
            statusManager.UpdateState = AssetState.asLoadedManifest;
        }
        else
        {
            Dictionary<string, AssetConf> assets = statusManager.tempConfProject.Assets;
            if (assets.ContainsKey(customId))
            {
                AssetConf assetConf = assets[customId];
                statusManager.tempConfProject.SetAssetDownloadState(customId, AssetDownloadState.adsSuccessed);
                fromSaveSize += assetConf.size;
                if (fromSaveSize > 3 * 1024 * 1024)
                {
                    statusManager.tempConfProject.SaveToFile(statusManager.tempManifestPath);
                    fromSaveSize = 0;
                }
            }

            if (downloadUnits.ContainsKey(customId))
            {
                totalWaitToDownload--;

                percentByFile = 100 * (float)(totalToDownload - totalWaitToDownload) / totalToDownload;
                DispatchUpdateEvent(AssetEventCode.aecUpdateProgression, "");
            }

            DispatchUpdateEvent(AssetEventCode.aecAssetUpdated, customId);

            if (failedUnits.ContainsKey(customId))
            {
                failedUnits.Remove(customId);
            }

            if (failedUnits.Count != 0 && failedUnits.Count == totalWaitToDownload)         //最后一个是成功的时候的触发
            {
                statusManager.UpdateState = AssetState.asPreDownloadFailure;
            }
            if (totalWaitToDownload == 0 && failedUnits.Count == 0)
            {
                statusManager.tempConfProject.SaveToFile(statusManager.cacheManifestPath);  //更新完成之后要改名保存
                statusManager.UpdateState = AssetState.asUpdateToData;
            }
        }
    }


    public void DispatchUpdateEvent(AssetEventCode code, string message = "", string assetId = "")
    {
        if (onCallBack != null)
        {
            progressItem.assetId = assetId;
            progressItem.totalToDownload = totalToDownload;
            progressItem.percent = percent;
            progressItem.percentByFile = percentByFile;
            progressItem.totalDownloaded = totalDownloaded;
            progressItem.totalSize = totalSize;
            onCallBack(this, code, progressItem, message);
        }
    }


    public readonly static AssetResultHandler Instance = new AssetResultHandler();
}

