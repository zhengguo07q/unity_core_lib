// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: DomamolDialogBase.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: 与一般的模态对话框不同， 手游要求所有的对话框都有模态， 只有层的区分问题
//                关于遮罩问题，这里只做一个遮罩，每次关闭和打开UI的时候会去检测当前被注册的模态对话框， 并把遮罩加载最上面
// ***************************************************************


using SLua;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class DomamolDialogBase : WindowBase
{
    public delegate void OnClickDelegate(GameObject maskGameObject);

    protected static string MASK_RESOURCE_NAME = "UI/Background/WindowMask";

    protected static GameObject maskGameObject;
    protected static UISprite maskSprite;
    public static List<DomamolDialogBase> DomamolDialogList = new List<DomamolDialogBase>();
    public static DomamolDialogBase LastDomamoDialog { get; set; }

    protected bool allowMaskClick = false;
    protected OnClickDelegate OnClick;

    protected override void Show()
    {
        DomamolDialogList.Add(this);
        AddMask();
    }


    protected override void Hide()
    {
        DomamolDialogList.Remove(this);
        AddMask();
    }


    protected void Close(GameObject closeGo)
    {
        Close();
    }


    private static void OnClickMask(GameObject go)
    {
        Transform parentTrans = go.transform.parent;
        if (parentTrans == null)
            return;

        DomamolDialogBase windowBase = GameObjectUtility.FindAndGet<DomamolDialogBase>("", parentTrans.gameObject);
        if (windowBase.allowMaskClick == true)
        {
            if (windowBase.OnClick != null)
            {
                windowBase.OnClick(go);
            }
            else
            {
                windowBase.Close();
            }
        }
    }


    public static void AddMask()
    {
        DomamolDialogBase dialogBase = GetTopDomamolDialog();
        if (dialogBase == null || dialogBase == LastDomamoDialog)
        {
            LastDomamoDialog = null;
            return;
        }

        DestoryMaskGameObject();
        GetMaskGameObject();

        maskSprite = GameObjectUtility.GetIfNotAdd<UISprite>(maskGameObject);
        maskSprite.depth = -100;

        GameObjectUtility.AddGameObject(dialogBase.gameObject, maskGameObject);
        LastDomamoDialog = dialogBase;
    }


    private static DomamolDialogBase GetTopDomamolDialog()
    {
        int maxId = int.MinValue;
        DomamolDialogBase topDialog = null;

        for (int i = 0; i < DomamolDialogList.Count; i++)
        {
            DomamolDialogBase dialogBase = DomamolDialogList[i];
            if (dialogBase == null)
                continue;

            if (maxId < dialogBase.sortId)
            {
                maxId = dialogBase.sortId;
                topDialog = dialogBase;
            }
        }

        return topDialog;
    }


    private static GameObject GetMaskGameObject()
    {
        if (maskGameObject == null)
        {
			maskGameObject = ResourceLoader.Instantiate(MASK_RESOURCE_NAME);
            UIEventListener.Get(maskGameObject).onClick = OnClickMask;
        }
        return maskGameObject;
    }


    private static void DestoryMaskGameObject()
    {
        if (maskGameObject != null)
        {
            GameObjectUtility.DestoryGameObject(maskGameObject);
            maskGameObject = null;
        }
    }
}

