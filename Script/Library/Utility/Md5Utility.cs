// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: Md5Utility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************

using System;
using System.Security.Cryptography;
using System.Text;


public class Md5Utility
{

    private static string GetMd5Hash(MD5 md5Hash, byte[] data)
    {
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }


    private static string GetMd5Hash(MD5 md5Hash, string input)
    {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        return GetMd5Hash(md5Hash, data);
    }


    public static string GetMd5(string file)
    {
        string data = FileUtility.GetString(file);
        using (MD5 md5Hash = MD5.Create())
        {
            return GetMd5Hash(md5Hash, data);
        }
    }


    public static bool EqualsMd5(string oldPath, string newPath)
    {
        string _old = FileUtility.GetString(oldPath);
        string _new = FileUtility.GetString(newPath);
        if (_old == null || _new == null)
        {
            return false;
        }
        using (MD5 md5Hash = MD5.Create())
        {
            string hash = GetMd5Hash(md5Hash, _old);
            string hashOfInput = GetMd5Hash(md5Hash, _new);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

