// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIListScrollView.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using System;
using UnityEngine;

public class UIListScrollView : UIScrollView
{
    private Bounds customBounds;
    private Vector3 boundsMax;
    private Vector3 boundsMin;
    public UIListScrollView():base()
    {
    }

    public Bounds CustomBounds
    {
        set { customBounds = value; }
    }
    
    public Vector3 BoundsMax
    {
        set{ boundsMax = value; }
    }
    
    public Vector3 BoundsMin
    {
        set{ boundsMin = value; }
    }


    public override Bounds bounds
    {
        get
        {
            if (!customBounds.size.Equals(Vector3.zero)) return customBounds;

            if (!mCalculatedBounds)
            {
                mCalculatedBounds = true;
                mTrans = transform;
                mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
                if (boundsMax != Vector3.zero)
                    mBounds.max = boundsMax;
                if (boundsMin != Vector3.zero)
                    mBounds.min = boundsMin;
            }
            return mBounds;
        }
    }
}

