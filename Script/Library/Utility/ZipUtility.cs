// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ZipUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

public class ZipUtility
{
    public static bool UnZip(string path)
    {
        string directoryName = Path.GetDirectoryName(path);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        string filePathWithoutExtension = directoryName + Path.DirectorySeparatorChar + fileNameWithoutExtension;

        FastZipEvents events = new FastZipEvents();
        events.Progress = onProgress;
        FastZip fast = new FastZip();
        fast.ExtractZip(path, filePathWithoutExtension, "");
        return true;
    }


    public static void Zip(string toZip, string savePath, ProcessFileHandler progressHandler = null, CompletedFileHandler completeFileHandler = null)
    {
        FastZipEvents events = new FastZipEvents();
        if (progressHandler != null)
        {
            events.ProcessFile = progressHandler;
            events.ProgressInterval = TimeSpan.FromSeconds(1);
        }
        if (completeFileHandler != null)
        {
            events.CompletedFile = completeFileHandler;
        }

        FastZip fastZip = new FastZip(events);
        fastZip.CreateEmptyDirectories = true;
        fastZip.CreateZip(toZip, savePath, true, "");
    }


    private static void onProgress(object sender, ICSharpCode.SharpZipLib.Core.ProgressEventArgs e)
    {
        UnityEngine.Debug.Log(sender);
    }
}

