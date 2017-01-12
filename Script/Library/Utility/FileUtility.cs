// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: FileUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


[CustomLuaClass]
public class FileUtility
{
    public static bool IsFileExist(string filePath)
    {
#if UNITY_EDITOR || UNITY_IOS
        return DefaultFileUtility.IsFileExists(filePath);
#elif UNITY_ANDROID
        return AndroidFileUtility.IsFileExists(filePath);
#endif
    }


    public static bool RenameFile(string path, string oldFile, string newFile)
    {
        string _old = path + oldFile;
        string _new = path + newFile;
        try
        {
            if (IsFileExist(_old))
            {
                DeleteFile(_new);
                File.Move(_old, _new);
                return true;
            }
        }
        catch (IOException e)
        {
            return false;
        }
        return false;
    }


    public static bool DeleteFile(string file)
    {
        if (IsFileExist(file))
        {
            File.Delete(file);
            return true;
        }
        return false;
    }


    public static string GetString(string fileName)
    {
        if (!IsFileExist(fileName))
        {
            return "";
        }
#if UNITY_EDITOR || UNITY_IOS
        return DefaultFileUtility.GetString(fileName);
#elif UNITY_ANDROID
        return AndroidFileUtility.GetString(fileName);
#endif
    }


    public static string GetString(string path, string fileName)
    {
        if (!path.EndsWith("/")) path += "/";
        return GetString(path + fileName);
    }


    public static byte[] GetBytes(string path, string fileName)
    {
        if (!path.EndsWith("/")) path += "/";
        return GetBytes(path + fileName);
    }


    public static byte[] GetBytes(string fileName)
    {
        if (!IsFileExist(fileName))
        {
            return null;
        }
#if UNITY_EDITOR || UNITY_IOS
        return DefaultFileUtility.GetBytes(fileName);
#elif UNITY_ANDROID
        return AndroidFileUtility.GetBytes(fileName);
#endif
    }


    public static bool WriteFile(string filepath, string data)
    {
        try
        {
            string path = Path.GetDirectoryName(filepath);
            if (!IsDirectoryExist(path))
            {
                CreateDirectory(path);
            }
            File.WriteAllText(filepath, data, Encoding.UTF8);
            return true;
        }
        catch (IOException e)
        {
            throw e;
        }
    }


    public static bool WriteFile(string filePath, byte[] bytes)
    {
        try
        {
            string path = Path.GetDirectoryName(filePath);
            if (!IsDirectoryExist(path))
            {
                CreateDirectory(path);
            }
            File.WriteAllBytes(filePath, bytes);
            return true;
        }
        catch (IOException e)
        {
            throw e;
        }
    }


    private static void Write(FileStream fs, byte[] data)
    {
        fs.Write(data, 0, data.Length);
    }


    public static bool WriteFileStream(string path, List<byte[]> dataes)
    {
        using (FileStream fs = new FileStream(path, System.IO.FileMode.Append))
        {
            for (int i = 0; i < dataes.Count; ++i)
                Write(fs, dataes[i]);
        }
        return true;
    }


    public static bool WriteFileStream(string dir, string filename, List<byte[]> dataes)
    {
        CreateDirectory(dir);
        return WriteFileStream(Path.Combine(dir, filename), dataes);
    }


    public static void CreateDirectory(string path)
    {
        if (!IsDirectoryExist(path))
            Directory.CreateDirectory(path);
    }


    public static void ClearDirectory(string path)
    {
        DirectoryInfo info = new DirectoryInfo(path);
        if (!info.Exists)
        {
            return;
        }
        FileInfo[] files = info.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            files[i].Delete();
        }
        DirectoryInfo[] diries = info.GetDirectories();
        for (int j = 0; j < diries.Length; j++)
        {
            diries[j].Delete(true);
        }
    }


    public static bool IsDirectoryExist(string dir)
    {
        return Directory.Exists(dir);
    }


    public static bool DeleteDirectory(string dir)
    {
        if (IsDirectoryExist(dir))
        {
            Directory.Delete(dir, true);
            return true;
        }
        return false;
    }


    public static string[] GetAllFileInPath(string path)
    {
        return GetAllFileInPathWithSearchPattern(path, null);
    }


    public static string[] GetAllFileInPathWithSearchPattern(string path, string searchPattern)
    {
        List<string> list = new List<string>();
        ForEachDirectory(path, searchPattern, (string file) =>
        {
            list.Add(file);
        });

        
        return list.ToArray();
    }


    public static string[] ListFileName(string path)
    {
#if UNITY_EDITOR || UNITY_IOS
        return DefaultFileUtility.ListFileName(path);
#elif UNITY_ANDROID
        return AndroidFileUtility.ListFileName(path);
#endif
    }


    public static void ForEachDirectory(string path, Action<string> callBack)
    {
        ForEachDirectory(path, null, callBack);
    }


    public static void ForEachDirectory(string path, string searchPattern, Action<string> callBack)
    {
        DirectoryInfo info = new DirectoryInfo(path);
        if (!info.Exists)
        {
            return;
        }

        FileInfo[] files;
        if (searchPattern == null)
        {
            searchPattern = "*";
        }

        files = info.GetFiles(searchPattern, SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            callBack(files[i].FullName);
        }
    }
}

 
public class AndroidFileUtility
{
    private static AndroidJavaClass helper;


    private static AndroidJavaClass Helper
    {
        get
        {
            if (helper == null)
            {
                helper = new AndroidJavaClass("com.yetogame.sdk.util.Unity3dHelper");
            }
            return helper;
        }
    }


    public static byte[] GetBytes(string path)
    {
        if (path.IndexOf(Application.streamingAssetsPath) > -1)
        {
            path = path.Replace(Application.streamingAssetsPath + "/", "");
        }
        else if (path.IndexOf(Application.persistentDataPath) > -1)
        {
            return File.ReadAllBytes(path);
        }
        return Helper.CallStatic<byte[]>("getBytes", path);
    }


    public static string GetString(string path)
    {
        if (path.IndexOf(Application.streamingAssetsPath) > -1)
        {
            path = path.Replace(Application.streamingAssetsPath + "/", "");
        }
        else if (path.IndexOf(Application.persistentDataPath) > -1)
        {
            return File.ReadAllText(path);
        }
        return Helper.CallStatic<string>("getString", path);
    }


    public static bool IsFileExists(string path)
    {
        if(path.IndexOf(Application.streamingAssetsPath) > -1)
        {
            path = path.Replace(Application.streamingAssetsPath + "/", "");
        }
        else if(path.IndexOf(Application.persistentDataPath) > -1)
        {
            return File.Exists(path);
        }
        return Helper.CallStatic<bool>("isFileExists", path);
    }


    public static string[] ListFileName(string path)
    {
        if (path.IndexOf(Application.streamingAssetsPath) > -1)
        {
            path = path.Replace(Application.streamingAssetsPath + "/", "");
        }
        else if (path.IndexOf(Application.persistentDataPath) > -1)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return null;
            }
            FileInfo[] fileInfos = info.GetFiles();
            List<string> fileNameList = new List<string>();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                fileNameList.Add(fileInfos[i].Name);
            }
            return fileNameList.ToArray();
        }
        return Helper.CallStatic<string[]>("listFileName", path);
    }
}


public class DefaultFileUtility
{
    public static byte[] GetBytes(string path)
    {
        return File.ReadAllBytes(path);
    }


    public static string GetString(string path)
    {
        return File.ReadAllText(path);
    }


    public static bool IsFileExists(string path)
    {
        return File.Exists(path);
    }


    public static string[] ListFileName(string path)
    {
        DirectoryInfo info = new DirectoryInfo(path);
        if (!info.Exists)
        {
            return null;
        }
        FileInfo[] fileInfos = info.GetFiles();
        List<string> fileNameList = new List<string>();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            fileNameList.Add(fileInfos[i].Name);
        }
        return fileNameList.ToArray();
    }
}