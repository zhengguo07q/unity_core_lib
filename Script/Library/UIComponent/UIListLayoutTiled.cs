// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIListLayoutTiled.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using UnityEngine;
using System.Collections;

public class UIListLayoutTiled : UIListLayout
{
    public int cellWidth;
    public int cellHeight;
    public int columnLimit;

	protected bool isHorizontal = false;

	protected override void Start()
	{
		base.Start();
		isHorizontal = scrollView.movement == UIScrollView.Movement.Horizontal;
	}


	public override void Validate()
	{
		base.Validate();
	}


	public void InvalidAnimateNow(int type=1)
	{
		if (isStart) Validate();
	}


    protected override void UpdateContent()
    {
        float extents = cellHeight * listChildren.size * 0.5f / (float)columnLimit;
        Vector3[] corners = listPanel.worldCorners;
        
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = transform.InverseTransformPoint(v);
            corners[i] = v;
        }
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        for (int i = 0; i < listChildren.size; ++i)
        {
            Transform t = listChildren[i];
            float distance = t.localPosition.y - center.y;
            float newPosition = 0f;
            if (distance < -extents)
            {
                newPosition = t.localPosition.y + extents * 2f;
                if (newPosition <= 0)
                {
                    t.localPosition += new Vector3(0f, extents * 2f, 0f);
                    distance = t.localPosition.y - center.y;
                    int col = (int)t.localPosition.x / cellWidth;
                    int row = Mathf.Abs((int)newPosition / cellHeight);
                    UpdateItem(t, row*columnLimit + col, true);
                }
            }
            else if (distance > extents)
            {
                newPosition = t.localPosition.y - extents * 2f;
                int col = (int)t.localPosition.x / cellWidth;
                int row = Mathf.Abs((int)newPosition / cellHeight);
                int index = row * columnLimit + col;
                if (index < dataProvider.Count)
                {
                    t.localPosition -= new Vector3(0f, extents * 2f, 0f);
                    distance = t.localPosition.y - center.y;
                    UpdateItem(t, row*columnLimit + col, true);
                }
            }
        }
    }
    

    protected override void ResetPosition()
    {
        renderMap.Clear();
        for (int i = 0; i < listChildren.size; ++i)
        {
            Transform t = listChildren[i];
            int col = i % columnLimit;
            int row = i / columnLimit;
            t.localPosition = new Vector3(col*cellWidth, -row*cellHeight, 0f);
            UpdateItem(t, i, false);
        }
        scrollView.ResetPosition();
    }
    

    protected override void CalculateBounds()
    {
        if (dataProvider == null || !(scrollView is UIListScrollView))
            return ;
        
        Vector3 size = new Vector3();
        Vector3 center = new Vector3();
        float rowCount = Mathf.Ceil((float)dataProvider.Count/(float)columnLimit);
        size.x = listPanel.baseClipRegion.z;
        size.y = cellHeight * rowCount;
        center.x = size.x / 2f;
        center.y = -size.y / 2f;
        (scrollView as UIListScrollView).CustomBounds = new Bounds(center, size);
    }
}

