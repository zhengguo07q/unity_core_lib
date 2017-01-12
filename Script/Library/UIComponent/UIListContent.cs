// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIListContent.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;
using SLua;


[CustomLuaClass]
public class UIListContent : MonoBehaviour
{
    public int itemSize = 100;
    public int childrenCount = 7;

    protected ListCollection dataProvider;
    protected GameObject itemRenderer;
    
    protected TypeEventHandler handler;
 //   protected Type comType;
    protected bool isStart = false;
    protected bool isInvalidData = false;
    protected bool isInvalidRenderer = false;

    protected Transform trans;
    protected UIPanel listPanel;
    protected UIScrollView scrollView;
    protected bool isHorizontal = false;
    protected BetterList<Transform> listChildren = new BetterList<Transform>();
    protected Dictionary<object, UIItemRenderer> renderMap = new Dictionary<object, UIItemRenderer>();

    void Start()
    {
        trans = transform;
        listPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        scrollView = listPanel.GetComponent<UIScrollView>();
        scrollView.GetComponent<UIPanel>().onClipMove = OnMove;
        isHorizontal = scrollView.movement == UIScrollView.Movement.Horizontal;
        isStart = true;
        enabled = false;
        Validate();
    }


    private void Validate()
    {
        if (isInvalidRenderer)
        {
            isInvalidRenderer = false;

            ClearChildren();
            CreateChildren();
        }

        if (isInvalidData)
        {
            isInvalidData = false;

            CalculateBounds();
            CheckCount();
            ResetPosition();
        }
    }


    void OnDestroy()
    {
        if (dataProvider != null)
        {
            dataProvider.OnAddItem -= OnAddItem;
            dataProvider.OnRemoveItem -= OnRemoveItem;
            dataProvider.OnUpdateItem -= OnUpdateItem;
            dataProvider.OnClearItem -= OnClearItem;
            dataProvider = null;
        }
    }


    protected void OnMove(UIPanel panel)
    {
        UpdateContent();
    }


    public void InvalidDateNow()
    {
        isInvalidData = true;
        if (isStart) Validate();
    }


    public void InvalidRenderNow()
    {
        isInvalidRenderer = true;
        if (isStart) Validate();
    }


    public virtual void UpdateContent()
    {
        float extents = itemSize * listChildren.size * 0.5f;
        Vector3[] corners = listPanel.worldCorners;

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = trans.InverseTransformPoint(v);
            corners[i] = v;
        }
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);

        if (isHorizontal)
        {
            for (int i = 0; i < listChildren.size; ++i)
            {
                Transform t = listChildren[i];
                float distance = t.localPosition.x - center.x;
                float newPosition = 0f;
                if (distance < -extents)
                {
                    newPosition = t.localPosition.x + extents * 2f;
                    if (newPosition < dataProvider.Count * itemSize)
                    {
                        t.localPosition = new Vector3(newPosition, 0f, 0f);
                        distance = t.localPosition.x - center.x;
                        UpdateItem(t, (int)newPosition / itemSize, true);
                    }
                }
                else if (distance > extents)
                {
                    newPosition = t.localPosition.x - extents * 2f;
                    if (newPosition >= 0)
                    {
                        t.localPosition = new Vector3(newPosition, 0f, 0f);
                        distance = t.localPosition.x - center.x;
                        UpdateItem(t, (int)newPosition / itemSize, true);
                    }
                }
            }
        }
        else
        {
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
                        t.localPosition = new Vector3(0f, newPosition, 0f);
                        distance = t.localPosition.y - center.y;
                        UpdateItem(t, Mathf.Abs((int)newPosition / itemSize), true);
                    }
                }
                else if (distance > extents)
                {
                    newPosition = t.localPosition.y - extents * 2f;
                    if (newPosition > -dataProvider.Count * itemSize)
                    {
                        t.localPosition = new Vector3(0f, newPosition, 0f);
                        distance = t.localPosition.y - center.y;
                        UpdateItem(t, Mathf.Abs((int)newPosition / itemSize), true);
                    }
                }
            }
        }
    }


    public virtual void ResetPosition()
    {
        renderMap.Clear();
        for (int i = 0; i < listChildren.size; ++i)
        {
            Transform t = listChildren[i];
            t.localPosition = isHorizontal ? new Vector3(i * itemSize, 0f, 0f) : new Vector3(0f, -i * itemSize, 0f);
            UpdateItem(t, i, false);
        }
        if (scrollView != null)
            scrollView.ResetPosition();
    }


    protected virtual void UpdateItem(Transform item, int index, bool checkOriginal)
    {
        if (dataProvider == null || dataProvider.Count == 0 || index >= dataProvider.Count)
            return;

        UIItemRenderer uiItemRenderer = item.GetComponent<UIItemRenderer>();
        object originalData = uiItemRenderer.Data;
        object newData = dataProvider[index];
        if (checkOriginal && originalData != null && renderMap.ContainsKey(originalData))
            renderMap.Remove(originalData);

        if (uiItemRenderer != null)
        {
            uiItemRenderer.Data = newData;
            if (!renderMap.ContainsKey(newData))
                renderMap.Add(newData, uiItemRenderer);
        }
    }


    protected void CreateChildren()
    {
        if (itemRenderer != null)
        {
            for (int i = 0; i < childrenCount; i++)
            {
                GameObject go = Instantiate(itemRenderer) as GameObject;
                ScriptManager.Instance.LoadScriptBehaviour<SupportUIItemRenderer>(go);
//                go.AddComponent(comType);
                go.transform.parent = trans;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.name = itemRenderer.name + i;
                go.SetActive(false);
                listChildren.Add(go.transform);
            }
            AddEventListener();
        }
    }


    protected void AddEventListener()
    {
        if (handler != null && listChildren.size > 0)
        {
            for (int i = 0; i < listChildren.size; i++)
            {
                UIItemRenderer uiItemRenderer = listChildren[i].gameObject.GetComponent<UIItemRenderer>();
                if (uiItemRenderer != null) uiItemRenderer.eventHandler = handler;
            }
        }
    }


    protected virtual void CalculateBounds()
    {
        if (dataProvider == null)
            return;

        Vector3 size = new Vector3(0f, 0f, 0f);
        Vector3 center = new Vector3(0f, 0f, 0f);
        if (isHorizontal)
        {
            size.x = itemSize * dataProvider.Count;
            size.y = listPanel.baseClipRegion.w;
            center.x = size.x / 2f;
        }
        else
        {
            size.x = listPanel.baseClipRegion.z;
            size.y = itemSize * dataProvider.Count;
            center.y = -size.y / 2f;
        }

        if (scrollView is UIListScrollView)
            (scrollView as UIListScrollView).CustomBounds = new Bounds(center, size);
    }


    protected void ClearChildren()
    {
        for (int i = 0; i < listChildren.size; i++)
        {
            GameObject.Destroy(listChildren[i].gameObject);
        }
        listChildren.Clear();
        renderMap.Clear();
    }


    protected void CheckCount()
    {
        int count = dataProvider != null ? dataProvider.Count : 0;
        for (int i = 0; i < listChildren.size; ++i)
        {
            listChildren[i].gameObject.SetActive(i < count);
        }
    }


    protected void OnAddItem(object data)
    {
        InvalidDateNow();
    }


    protected void OnRemoveItem(object data)
    {
        InvalidDateNow();
    }


    protected void OnUpdateItem(object data)
    {
        Dictionary<object, UIItemRenderer> .Enumerator enumer = renderMap.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (dataProvider.OnCompareFunc(enumer.Current.Key, data) == 0)
            {
                UIItemRenderer uiItemRenderer = enumer.Current.Value;
                uiItemRenderer.InvalidNow();
            }
        }
    }


    protected void OnClearItem(object data)
    {
        CheckCount();
    }


    public ListCollection DataProvider
    {
        set
        {
            if (dataProvider != value)
            {
                if (dataProvider != null)
                {
                    dataProvider.OnAddItem -= OnAddItem;
                    dataProvider.OnRemoveItem -= OnRemoveItem;
                    dataProvider.OnUpdateItem -= OnUpdateItem;
                    dataProvider.OnClearItem -= OnClearItem;
                }

                dataProvider = value;

                if (dataProvider != null)
                {
                    dataProvider.OnAddItem += OnAddItem;
                    dataProvider.OnRemoveItem += OnRemoveItem;
                    dataProvider.OnUpdateItem += OnUpdateItem;
                    dataProvider.OnClearItem += OnClearItem;
                }
            }
            InvalidDateNow();
        }
        get
        {
            return dataProvider;
        }
    }


    public GameObject ItemRenderer
    {
        set
        {
            if (itemRenderer != value)
            {
                itemRenderer = value;
                InvalidRenderNow();
            }
        }
    }

    //public Type ComType
    //{
    //    set
    //    {
    //        if(comType != value)
    //        {
    //            comType = value;
    //        }
    //    }
    //}


    public TypeEventHandler eventHandler
    {
        set
        {
            if (handler != value)
            {
                handler = value;
                AddEventListener();
            }
        }
    }


    public BetterList<Transform> ListChildren
    {
        get
        {
            return listChildren;
        }
    }
}
