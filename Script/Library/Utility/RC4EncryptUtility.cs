// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: RC4EncryptUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


public class RC4EncryptUtility 
{
    public static int EncryptLen = 256;
    public static string EncryptKey = "this2is3source encryption key for 帝国远征 game,please 来破解啊";


    public static byte[] RC4(byte[] input, string key)
    {
        byte[] result = new byte[input.Length];
        int x, y, j = 0;
        int[] box = new int[256];

        for (int i = 0; i < 256; i++)
        {
            box[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            j = (key[i % key.Length] + box[i] + j) % 256;
            x = box[i];
            box[i] = box[j];
            box[j] = x;
        }

        for (int i = 0; i < input.Length; i++)
        {
            y = i % 256;
            j = (box[y] + j) % 256;
            x = box[y];
            box[y] = box[j];
            box[j] = x;

            result[i] = (byte)(input[i] ^ box[(box[y] + box[j]) % 256]);
        }
        return result;
    }


    public static void Encrypt(ref byte[] input)
    {
        byte[] tmp = new byte[input.LongLength];
        System.Array.Copy(input, 0, tmp, 0, input.Length);
        byte[] de = RC4(tmp, EncryptKey);
        System.Array.Copy(de, 0, input, 0, de.Length);
    }
}
