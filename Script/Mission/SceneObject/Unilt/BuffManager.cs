// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: BuffManager.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using System.Collections.Generic;
using UnityEngine;


public class BuffData
{
    public int targetId;
    public int buffId;
    public string buffName;
    public float cd;
}


public class BuffIcon 
{
    public int buffId;
    public GameObject gameObject;
    private float duration;
    private float createTime;
    private float amout = 1.0f;
    private UISprite cdSprit;
    public bool Running { get; private set; }


    public BuffIcon(GameObject obj, UISprite cdSprit, float duration)
    {
        gameObject = obj;
        this.cdSprit = cdSprit;
        createTime = Time.time;
        this.duration = duration;
        Running = (duration != 0);
        cdSprit.fillAmount = 0.0f;
    }


    public void Update()
    {
        if (Running)
        {
            if (duration > 0)
            {
                amout = (Time.time - createTime) / duration;
                if (amout >= 1.0f)
                {
                    amout = 1.0f;
                    Running = false;
                }
            }
            else if (amout != 0)
            {
                amout = 0;
                cdSprit.fillAmount = amout;
            }
        }
    }


    public void Dispose(bool gc = false)
    {
        if (gameObject)
            GameObject.Destroy(gameObject, 0.03f);
        gameObject = null;
        cdSprit = null;
        Running = false;
    }
}


public class BuffManager : MonoBehaviour
{
    private List<BuffIcon> buffList = new List<BuffIcon>();
    private List<BuffIcon> removeList = new List<BuffIcon>();
    private GameObject sprite;
    private const int show_max_num = 4;

    private float updateLastTime = 0;


    public void AddBuff(BuffData data)
    {
        BuffIcon buffIcon = FindBuffIcon(data.buffId);
        if (buffIcon != null) 
            return; 
        CreateBuffIcon(data);
        UpdateBuffIconLayer();
    }


    public void RemoveBuff(int buffId)
    {
        BuffIcon bi = FindBuffIcon(buffId);
        if (bi == null) { return; }
        buffList.Remove(bi);
        bi.Dispose();
        UpdateBuffIconLayer();
    }


    public void Dispose()
    {
        ClearList(buffList);
        ClearList(removeList);
    }


    private void Awake()
    {
        sprite = transform.Find("Buff_Layer/Buff_Icon").gameObject;
        sprite.SetActive(false);
    }


    private void FixedUpdate()
    {
        if (Time.time >= updateLastTime + 0.1f)
        {
            CheckBuff();
            updateLastTime = Time.time;
        }
    }


    private void OnDestroy()
    {
        Dispose();
    }


    private void CheckBuff()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffIcon bi = buffList[i];
            bi.Update();
            if (!bi.Running)
            {
                removeList.Add(bi);
            }
        }
        if (removeList.Count > 0)
        {
            for (int i = removeList.Count - 1; i >= 0; i--)
            {
                BuffIcon bi = removeList[i];
                buffList.Remove(bi);
                bi.Dispose();
            }
            removeList.Clear();
            UpdateBuffIconLayer();
        }
    }


    private void CreateBuffIcon(BuffData data)
    {
        if (sprite == null) return;
        GameObject obj = GameObject.Instantiate(sprite) as GameObject;
        obj.transform.parent = sprite.transform.parent;
        obj.transform.localRotation = sprite.transform.localRotation;
        obj.transform.localScale = sprite.transform.localScale;
        obj.transform.localPosition = sprite.transform.localPosition;
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        UISprite sp = obj.GetComponent<UISprite>();
        sp.spriteName = data.buffName;
        UISprite cdsp = obj.transform.FindChild("Cd_Time").GetComponent<UISprite>();
        BuffIcon bi = new BuffIcon(obj, cdsp, data.cd);
        bi.buffId = data.buffId;
        buffList.Add(bi);
    }


    private void UpdateBuffIconLayer()
    {
        int count = buffList.Count;
        for (int i = 0; i < count; i++)
        {
            BuffIcon bi = buffList[i];
            if (i < show_max_num)
            {
                bi.gameObject.SetActive(true);
                UISprite us = bi.gameObject.GetComponent<UISprite>();
                bi.gameObject.transform.localPosition = new Vector3(0.0f, -us.height * i, 0);
            }
            else { bi.gameObject.SetActive(false); }
        }
    }


    private BuffIcon FindBuffIcon(int buffId)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffIcon buffIcon = buffList[i];
            if (buffIcon.buffId == buffId)
            {
                return buffIcon;
            }
        }
        return null;
    }


    private void ClearList(List<BuffIcon> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            BuffIcon bi = list[i];
            bi.Dispose();
        }
        list.Clear();
    }
}
