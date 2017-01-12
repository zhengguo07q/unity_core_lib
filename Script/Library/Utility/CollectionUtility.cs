// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: CollectionUtility.cs
//  Creator 	: zg
//  Date		: 2016-10-21
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;


public class CollectionUtility
{
    public static T CloneDictionary<T, K, V>(T srcCollect, ref T destCollect) where T : Dictionary<K, V>
    {
        Dictionary<K, V>.Enumerator srcCollectEnumerator = srcCollect.GetEnumerator();
        while (srcCollectEnumerator.MoveNext())
        {
            destCollect.Add(srcCollectEnumerator.Current.Key, srcCollectEnumerator.Current.Value);
        }
        return destCollect;
    }


    public static T CloneList<T, V>(T srcCollect, ref T destCollect) where T : List<V>
    {
        for (int i = 0; i < srcCollect.Count; i++)
        {
            destCollect.Add(srcCollect[i]);
        }
        return destCollect;
    }


    public static T CloneQueue<T, V>(T srcCollect, ref T destCollect) where T : Queue<V>
    {
        V [] elementList = srcCollect.ToArray();
        for (int i = 0; i < elementList.Length; i++)
        {
            destCollect.Enqueue(elementList[i]);
        }
        return destCollect;
    }
}

