// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: Download.cs
//  Creator 	: 
//  Date		: 
//  Comment		: 下载器
// ***************************************************************


using System;
using System.Collections.Generic;
using BestHTTP;
using System.IO;
using System.Diagnostics;

public struct AssetDownloadUnit
{
    public string srcUrl;
    public string storagePath;
    public string customId;
    public double downloaded;
    public bool resumeDownload;
};


public enum AssetDownErrorCode
{
    adecInitial,
    adecQueued,
    adecProcessing,
    adecFinished,
    adecError,
    adecAborted,
    adecConnectionTimedOut,
    adecTimedOut,
    adecErrorData,
};


public struct AssetDownloadError
{
    public AssetDownErrorCode code;
    public string message;
    public string customId;
    public string url;
};


public class AssetDownloadProgressData
{
    public AssetDownloader downloader;
    public string customId;
    public string url;
    public string path;
    public string name;
    public double downloaded;
    public double totalToDownload;
};


public class AssetDownloader
{
    public static Logger log = LoggerFactory.GetInstance().GetLogger(typeof(AssetDownloader));
    public const string TEMP = ".temp";

    public delegate void ErrorCallback(AssetDownloadError err);
    public delegate void ProgressCallback(double total, double downloaded, string url, string customId);
    public delegate void SuccessCallback(string srcUrl, string storagePath, string customId);

    public ErrorCallback onError;
    public ProgressCallback onProgress;
    public SuccessCallback onSuccess;

    public int totalWaitToDownload = 0;


    public void SetConnectionTimeout()
    {
        HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(10);
        HTTPManager.RequestTimeout = TimeSpan.FromSeconds(30);
    }


    public void BatchDownloadAsync(string storagePath, Dictionary<string, AssetDownloadUnit> units, string batchId)
    {
        ClearLastTempFile(storagePath);

        totalWaitToDownload = units.Count;
        foreach (var item in units)
        {
            AssetDownloadUnit unit = item.Value;
            GroupBatchDownload(unit, batchId);
        }
    }


    private void GroupBatchDownload(AssetDownloadUnit unit, string batchId)
    {
        AssetDownloadProgressData d = new AssetDownloadProgressData();
        PrepareDownload(unit.srcUrl, unit.storagePath, unit.customId, unit.downloaded, ref d);
        HTTPRequest request = new HTTPRequest(new Uri(unit.srcUrl), OnCallBack);
        request.Tag = d;
        request.DisableCache = true;
        if (batchId == AssetConstants.BATCH_UPDATE_ID)
        {
            request.UseStreaming = true;
            request.OnProgress = OnDownloadProgress;
        }
        else
        {
            request.UseStreaming = false;
        }
        HTTPManager.SendRequest(request);
    }


    public void DownloadAsync(string srcUrl, string storagePath, string customId)
    {
        AssetDownloadProgressData data = new AssetDownloadProgressData();
        log.Debug("downloadAsync : " + srcUrl + "  storagePath  : " + storagePath);
        PrepareDownload(srcUrl, storagePath, customId, 0, ref data);
        HTTPRequest request = BestHTTP.HTTPManager.SendRequest(srcUrl, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, true, OnCallBack);
        request.Tag = data;
    }


    private void PrepareDownload(string srcUrl, string storagePath, string customId, double totalToDownload, ref AssetDownloadProgressData data)
    {
        data.downloader = this;
        data.customId = customId;
        data.totalToDownload = totalToDownload;
        data.url = srcUrl;
        data.name = Path.GetFileName(storagePath);
        data.path = Path.GetDirectoryName(storagePath) + "/";
    }


    private void DoActionWhenDownLoaded(AssetDownloadProgressData data, HTTPResponse response)
    {
        if (data.customId == AssetConstants.VERSION_ID || data.customId == AssetConstants.MANIFEST_ID)
        {
            string json = response.DataAsText;
            FileUtility.WriteFile(data.path + data.name, json);
            onSuccess.Invoke(data.url, data.path + data.name, data.customId);
        }
        else
        {
            DoActionProcessing(data, response.GetStreamedFragments());
            if (response.IsStreamingFinished)
            {
                FileUtility.RenameFile(data.path, data.name + TEMP, data.name);
                if (data.downloaded != data.totalToDownload)
                {
                    AssetDownloadError err = new AssetDownloadError();
                    err.customId = data.customId;
                    err.code = AssetDownErrorCode.adecErrorData;
                    err.message = "Download size error ! need : " + data.totalToDownload + " curr downloaded :" + data.downloaded;
                    onError.Invoke(err);
                    return;
                }

                onSuccess.Invoke(data.url, data.path + data.name, data.customId);
            }
        }
    }


    private void DoActionProcessing(AssetDownloadProgressData data, List<byte[]> fragments)
    {
        if (fragments == null || fragments.Count == 0) return;
        for (int i = 0; i < fragments.Count; i++)
        {
            data.downloaded += fragments[i].Length;
        }
        FileUtility.WriteFileStream(data.path, data.name + TEMP, fragments);
    }


    private void OnCallBack(HTTPRequest request, HTTPResponse response)
    {
        AssetDownloadError err;
        AssetDownloadProgressData data = (AssetDownloadProgressData)request.Tag;
        if (response != null)
        {
            switch (request.State)
            {
                case HTTPRequestStates.Processing:
                    DoActionProcessing(data, response.GetStreamedFragments());
                    break;
                case HTTPRequestStates.Finished:
                    if (response.IsSuccess)
                    {
                        DoActionWhenDownLoaded(data, response);
                    }
                    else
                    {
                        string status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                    response.StatusCode,
                                                    response.Message,
                                                    response.DataAsText);
                        err = new AssetDownloadError();
                        err.customId = data.customId;
                        err.code = AssetDownErrorCode.adecError;
                        err.message = status;
                        onError.Invoke(err);
                    }

                    break;
                case HTTPRequestStates.Error:
                    err = new AssetDownloadError();
                    err.customId = data.customId;
                    err.code = AssetDownErrorCode.adecError;
                    err.message = "Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception");
                    onError.Invoke(err);
                    request = null;
                    break;
                case HTTPRequestStates.Aborted:
                    err = new AssetDownloadError();
                    err.customId = data.customId;
                    err.code = AssetDownErrorCode.adecAborted;
                    err.message = "Request Aborted!";
                    onError.Invoke(err);
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                    err = new AssetDownloadError();
                    err.customId = data.customId;
                    err.code = AssetDownErrorCode.adecConnectionTimedOut;
                    err.message = "Connection Timed Out!";
                    onError.Invoke(err);
                    break;
                case HTTPRequestStates.TimedOut:
                    err = new AssetDownloadError();
                    err.customId = data.customId;
                    err.code = AssetDownErrorCode.adecTimedOut;
                    err.message = "Processing the request Timed Out!";
                    onError.Invoke(err);
                    break;
            }
        }
        else
        {
            err = new AssetDownloadError();
            err.code = AssetDownErrorCode.adecError;
            err.customId = data.customId;
            err.message = "Request fail! " + request.CurrentUri;
            onError.Invoke(err);
        }


    }


    private void OnDownloadProgress(HTTPRequest request, int downloaded, int length)
    {
        AssetDownloadProgressData data = (AssetDownloadProgressData)request.Tag;
        onProgress.Invoke(length, downloaded, data.url, data.customId);
    }


    private void ClearLastTempFile(string storagePath)
    {
        log.Debug("delete last update temp file start.");
        string[] tempFileList = Directory.GetFiles(storagePath, "*.ab.temp", SearchOption.AllDirectories);
        for (int i = 0; i < tempFileList.Length; i++)
        {
            string tempFile = tempFileList[i];
            if (("project.manifest.temp").Equals(Path.GetFileName(tempFile)))
                continue;
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
                log.Debug("delete temp file : " + tempFile);
            }
        }
        log.Debug("delete last update temp file end.");
    }
}

