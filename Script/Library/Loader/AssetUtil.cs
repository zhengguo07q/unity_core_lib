// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetUtil.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.IO;


public class AssetUtil
{
    public static Logger logger = LoggerFactory.GetInstance().GetLogger(typeof(AssetUtil));
    private static byte[] encryptionKey = { 12, 239, 49, 78, 20, 61, 0, 1, 223 };


    public static string GetAssetWWWPath(string path)
    {
        string readPath = AssetUtil.GetAssetAbsolutePath(path);
        if (readPath.StartsWith("jar:file:///")) //判断是否为android下的streamingAssets
        {
            return readPath;
        }

#if UNITY_EDITOR
        readPath = "file:///" + readPath;
#else
        readPath = "file://" + readPath;
#endif
        return readPath;
    }


    public static string GetAssetAbsolutePath(string path)
    {
        string addFirstChar = AddFirstChar(path);
        string abpath = null;
#if UNITY_EDITOR && UNITY_IOS           //在编辑器模式下，不在启动下载和载入下载配置
        abpath = "AssetBundle/IOS/";
        abpath = Application.dataPath + "/../" + abpath + addFirstChar + ".ab";
        return abpath;
#elif UNITY_EDITOR && UNITY_ANDROID
        abpath = "AssetBundle/Android/";
        abpath = Application.dataPath + "/../" + abpath + addFirstChar + ".ab";
        return abpath;
#else
        abpath = AssetUtility.GetResourcePath(addFirstChar);
        return abpath + ".ab";
#endif
    }


    public static string AddFirstChar(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(dir))
        {
            return path;
        }

        int lastSplit = dir.LastIndexOf("/");
        string firstChar = string.Empty;
        if (lastSplit > 0)
        {
            firstChar = dir.Substring(lastSplit + 1, 1);
        }
        else
        {
            firstChar = dir.Substring(0, 1);
        }
        return dir + "/" + firstChar + Path.GetFileNameWithoutExtension(path);
    }


    public static void EncryptData(byte[] data)
    {
        return;
        if (data == null) return;
        byte code = 0;
        for(int i = 0;i < data.Length;i++)
        {
            if(i < encryptionKey.Length)
            {
                code = encryptionKey[i];
                data[i] += code;
            }
            else
            {
                break;
            }
        }
    }


    public static void DecryptData(byte[] data)
    {
        return;
        if (data == null) return;
        byte code = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if(i < encryptionKey.Length)
            {
                code = encryptionKey[i];
                data[i] -= code;
            }
            else
            {
                break;
            }
        }
    }
}