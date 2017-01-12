// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: WindowUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;

public class WindowUtility
{
    public static float GetAutoAdpateSize()
    {
        float ret = 1;
        if ((float)Screen.width / Screen.height < (float)1280 / 720)
        {
            float logicWidth = (float)Screen.width * 720 / Screen.height;
            float factor = logicWidth / 1280;
            ret = factor;
        }
        return ret;
    }

    public static void ChangeSize(Transform transform)
    {
        if (transform == null)
            return;

        float factor = GetAutoAdpateSize();
        factor = 1 / factor;
        transform.localScale = Vector3.one * factor;
    }
}

