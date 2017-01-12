// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class AssetUtility
{
    public static string GetNotVersionFileName(string path)
    {
        string fileName = System.IO.Path.GetFileName(path);
        string directoryName = System.IO.Path.GetDirectoryName(path);
        string[] fileNameSplit = fileName.Split('.');

        if (fileNameSplit.Length >= 2)
        {
            fileName = System.IO.Path.DirectorySeparatorChar + fileNameSplit[0] + '.' + fileNameSplit[1];
        }
        return directoryName + fileName;
    }


    public static void MakeSureDirectory(ref string directoryPath)
    {
        if (!string.IsNullOrEmpty(directoryPath) && directoryPath[directoryPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
        {
            directoryPath += "/";
        }
        FileUtility.CreateDirectory(directoryPath);
    }


    public static string GetResourcePath(string relativePath)
    {
        string absolutePath = PathUtility.PersistentDataPath + "/" + relativePath;
        Dictionary<string, AssetConf> packageAssets = AssetStatusManager.Instance.packageConfProject.Assets;
        Dictionary<string, AssetConf> streamAssets = AssetStatusManager.Instance.localConfProject.Assets;

        AssetConf packageAssetConf, localAssetConf;

        if (packageAssets.TryGetValue(relativePath, out packageAssetConf)) //包体中存在
        {
            localAssetConf = streamAssets[relativePath];

            if (localAssetConf != null && string.IsNullOrEmpty(packageAssetConf.md5) == false && packageAssetConf.md5.Equals(localAssetConf.md5)) //包体里md5存在且=外面的，使用包体的
            {
                absolutePath = Application.streamingAssetsPath + "/" + relativePath;
            }
        }
        return absolutePath;
    }
}

