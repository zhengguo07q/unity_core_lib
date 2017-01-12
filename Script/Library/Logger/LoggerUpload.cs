// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LoggerUpload.cs
//  Creator 	: zhangbihai 
//  Date		: 2016-8-23
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Collections;


public class LoggerUpload : SingletonMono<LoggerUpload>
{

    //public void UpdateLogFile(C2001_GC_UPLOAD_LOGS data)
    //{
    //    if (data.url != null)
    //        StartCoroutine(RealUpdateLogFile(data));
    //}


    //private IEnumerator RealUpdateLogFile(C2001_GC_UPLOAD_LOGS value)
    //{
    //    Debug.Log("上传开始，请稍等...");
    //    byte[] content = LoggerReport.Instance.GetLogFileByte();

    //    WWWForm form = new WWWForm();
    //    string timeStr = value.time.ToString();
    //    form.AddField("time", timeStr);
    //    string playerIdStr = value.playerId.ToString();
    //    form.AddField("playerId", playerIdStr);
    //    string signStr = value.sign.ToString();
    //    form.AddField("sign", signStr);
    //    form.AddBinaryData("file", content, ".log", "multipart / form - data");
    //    yield return null;
    //    WWW www = new WWW(value.url, form);
    //    yield return www;

    //    LoggerReport.Instance.DeleteFile();

    //    if (www.error == null)
    //    {
    //        Debug.Log("return:" + www.text);
    //        Debug.Log("上传成功");
    //    }
    //    else
    //    {
    //        Debug.Log("上传失败，error：" + www.error);
    //    }
    //}
}