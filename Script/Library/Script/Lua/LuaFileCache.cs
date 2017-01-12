// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LuaFileCache.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;


public class LuaFileCache
{
    private Dictionary<string, byte[]> fileCache = new Dictionary<string, byte[]>();


    public byte[] LoadFile(string path)
    {
        if(fileCache.ContainsKey(path))
            return fileCache[path];
        byte[] bytes = null;
        if(FileUtility.IsFileExist(path))
        {
            bytes = FileUtility.GetBytes(path);
        }
        
        if(bytes!=null)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        if(bytes!=null)
            RC4EncryptUtility.Encrypt(ref bytes);//这里进行解密操作

            fileCache.Add(path, bytes);
#endif
        }

        return bytes;
    }


    public static readonly LuaFileCache Instance = new LuaFileCache();
}

