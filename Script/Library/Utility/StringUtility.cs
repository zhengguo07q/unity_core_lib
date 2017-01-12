// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: StringUtility.cs
//  Creator 	: zg
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using SLua;
using System;


[CustomLuaClass]
public class StringUtility
{
    public static string EncodeUnicode(string srcStr)
    {
        char[] srcChar = srcStr.ToCharArray();
        string outStr = "";
        for (int i = 0; i < srcChar.Length;)
        {
            if (srcChar[i] == '\\' && srcChar[i + 1] == 'u')
            {
                char[] word = new char[4];
                Array.Copy(srcChar, i + 2, word, 0, 4);
                string val = new string(word);
                outStr += (char)int.Parse(val, System.Globalization.NumberStyles.HexNumber);
                i = i + 6;
            }
            else if (srcChar[i] == '\\' && srcChar[i + 1] == '/')
            {
                outStr += srcChar[i + 1];
                i += 2;
            }
            else
            {
                outStr += srcChar[i];
                i++;
            }
        }
        return outStr;
    }
}
