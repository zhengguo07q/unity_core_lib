// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SearchPathUtility.cs
//  Creator 	: zg
//  Date		: 
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;
using System.IO;


public class SearchPathUtility
{
    private static List<string> searchPathList = new List<string>();


    public static List<string> GetSearchPaths()
    {
        return searchPathList;
    }


    public static void SetSearchPaths(List<string> searchPaths)
    {
        searchPathList = searchPaths;
    }


    public static void AddSearchPath(string path)
    {
        AddSearchPath(path, false);
    }


    public static void AddSearchPath(string path, bool front)
    {
        FixedPath(ref path);
        if (front)
        {
            var index = searchPathList.IndexOf(path);
            if (index == -1)
                searchPathList.Insert(0, path);
            else if (index > 0)
            {
                searchPathList.Remove(path);
                searchPathList.Insert(0, path);
            }
        }
        else
        {
            var index = searchPathList.IndexOf(path);
            if (index == -1)
                searchPathList.Add(path);
            else if (index > searchPathList.Count - 1)
            {
                searchPathList.Remove(path);
                searchPathList.Add(path);
            }
        }
    }


    private static void FixedPath(ref string path)
    {
        if (!path.EndsWith("/"))
        {
            path = path + "/";
        }
    }


    public static string GetFullPath(string fileName)
    {
        for (int i = 0; i < searchPathList.Count; i++)
        {
            string path = searchPathList[i];
            if (IsRoot(path, fileName))
                continue;
            FixedPath(ref path);
            if (FileUtility.IsFileExist(path + fileName))
            {
                return path + fileName;
            }
        }
        return "";
    }


    private static bool IsRoot(string path, string fileName)
    {
        bool ret = false;
        if (Path.GetDirectoryName(fileName).IndexOf(path) > -1)
        {
            ret = true;
        }
        return ret;
    }
}

