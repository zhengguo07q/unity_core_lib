// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIListLayout.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;

public abstract class UIListLayout : UIScriptBehaviour
{
    public int childrenCount = 7;

    protected ListCollection dataProvider;
    protected GameObject itemRenderer;
    protected TypeEventHandler eventHandler;

    protected UIPanel listPanel;
    protected UIScrollView scrollView;

    protected bool isInvalidRenderer = false;
    protected BetterList<Transform> listChildren = new BetterList<Transform>();
    protected Dictionary<object, UIItemRenderer> renderMap = new Dictionary<object, UIItemRenderer>();
    

    protected override void Start ()
    {
        listPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        scrollView = listPanel.GetComponent<UIScrollView>();
        scrollView.GetComponent<UIPanel>().onClipMove = OnMove;

        if (isInvalidRenderer) CommitRender();
        base.Start();
    }


    protected override void OnDestroy()
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
    

    protected void OnMove (UIPanel panel) 
    {
        UpdateContent(); 
    }
    

    protected override void CommitData()
    {
        CalculateBounds();
        CheckCount();
        ResetPosition();
    }


    protected void CommitRender()
    {
        ClearChildren();
        CreateChildren();
    }
    

    protected void InvalidRender()
    {
        if (isStart) CommitRender();
        else isInvalidRenderer = true;
    }
    

    protected virtual void UpdateContent()
    {
    }
    

    protected virtual void ResetPosition()
    {
    }
    

    protected virtual void CalculateBounds()
    {
    }
    

    protected virtual void UpdateItem (Transform item, int index, bool checkOriginal) 
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
            renderMap.Add(newData, uiItemRenderer);
        }
    }
    

    protected void CreateChildren()
    {
        if (itemRenderer != null)
        {
            for (int i=0; i<childrenCount; i++)
            {
                GameObject go = Instantiate(itemRenderer) as GameObject;
                ScriptManager.Instance.LoadScriptBehaviour<SupportUIItemRenderer>(go);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
				go.name = itemRenderer.name + i;
                listChildren.Add(go.transform);
            }
            AddEventListener();
        }
    }


    protected void AddEventListener()
    {
        if (eventHandler != null && listChildren.size > 0)
        {
            for (int i = 0; i < listChildren.size; i++)
            {
                UIItemRenderer uiItemRenderer = listChildren[i].gameObject.GetComponent<UIItemRenderer>();
                if (uiItemRenderer != null) uiItemRenderer.eventHandler = eventHandler;
            }
        }
    }


    protected void ClearChildren()
    {
        for (int i=0; i<listChildren.size; i++)
            GameObject.Destroy(listChildren[i].gameObject);

        listChildren.Clear();
        renderMap.Clear();
    }


    protected void OnAddItem(object data)
    {
        InvalidNow();
    }


    protected void OnRemoveItem(object data)
    {
        InvalidNow();
    }


    protected void OnUpdateItem(object data)
    {
        Dictionary<object, UIItemRenderer>.Enumerator enumer = renderMap.GetEnumerator();
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


    protected void CheckCount()
    {
        int count = dataProvider != null ? dataProvider.Count : 0;
        for (int i = 0; i < listChildren.size; ++i)
        {
            listChildren[i].gameObject.SetActive(i<count);
        }
    }

    
    public ListCollection DataProvider
    {
        set
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
            InvalidNow();
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
                InvalidRender();
            }
        }
    }


    public TypeEventHandler EventHandler
    {
        set
        {
            if (eventHandler != value)
            {
                eventHandler = value;
                AddEventListener();
            }
        }
    }


    public Dictionary<object, UIItemRenderer> RenderMap{
		get{ return renderMap; }
	}
}
