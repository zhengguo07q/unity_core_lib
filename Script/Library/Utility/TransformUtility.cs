// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: TransformUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************

using System;
using UnityEngine;


public class TransformUtility
{
    public static void SetLocalPositionX(Transform trans, float x)
    {
        trans.localPosition = new Vector3(trans.localPosition.x + x, trans.localPosition.y, trans.localPosition.z);
    }


    public static void SetLocalPositionY(Transform trans, float y)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y + y, trans.localPosition.z);
    }


    public static void SetLocalPositionZ(Transform trans, float z)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + z);
    }


    public static void SetLocalProperty(Transform trans, Vector3 postion, Quaternion rotation, Vector3 scale)
    {
        trans.localPosition = postion;
        trans.localRotation = rotation;
        trans.localScale = scale;
    }


    public static void RestLocalProperty(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
}

