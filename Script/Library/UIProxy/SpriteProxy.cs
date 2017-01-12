// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SpriteProxy.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class SpriteProxy : MonoBehaviour
{
    public string atlasName;
    public bool isAsyncLoad = true;
    private UISprite sprite;
    private Asset asset;


    void Awake()
    {
        SetAtlas();
    }


    UISprite Sprite
    {
        get
        {
            if(sprite == null)
            {
                sprite = GetComponent<UISprite>();
            }
            return sprite;
        }
    }


    void OnDestroy()
    {
        if (asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
    }

    public void SetAtlasSprite(string spriteName, string atlasName)
    {
        Sprite.spriteName = spriteName;
        SetAtlasName(atlasName);
    }

    public void SetAtlasName(string atlasName)
    {
        if (string.IsNullOrEmpty(atlasName))
            return;

        if (atlasName == this.atlasName)
            return;

        if (asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
        this.atlasName = atlasName;
        SetAtlas();

        //Debug.LogError("set atlas ," + atlasName);
    }

    void SetAtlas()
    {
        if (string.IsNullOrEmpty(atlasName))
            return;

        //time = Time.realtimeSinceStartup;
        if (!isAsyncLoad)
            LoadDone(AssetLoader.Instance.SyncLoad(atlasName));
        else
            AssetLoader.Instance.AsyncLoad(atlasName, LoadDone);
    }

    void LoadDone(Asset asset)
    {
        if (asset == null)
            return;

        if (asset.name != this.atlasName)
            return;

        
        this.Sprite.atlas = asset.GetFromObject<UIAtlas>();
        asset.AddRef();
        this.asset = asset;

        AfterCheckComponent();
    }


    private void AfterCheckComponent()
    {
        UISpriteAnimation spriteAnimation = this.gameObject.GetComponent<UISpriteAnimation>();
        if (spriteAnimation != null)
        {
            spriteAnimation.RebuildSpriteList();
        }
    }


    public static SpriteProxy GetProxy(UISprite uiSprite)
    {
        if (uiSprite == null) return null;
        SpriteProxy spriteProxy = uiSprite.GetComponent<SpriteProxy>();
        if(spriteProxy == null)
        {
            spriteProxy = uiSprite.gameObject.AddComponent<SpriteProxy>();
        }
        return spriteProxy;
    }


    public static SpriteProxy GetProxy(GameObject spriteObj)
    {
        UISprite sprite = spriteObj.GetComponent<UISprite>();
        if (sprite == null) return null;
        return GetProxy(sprite);
    }

    public static void SetSpriteProxy(UISprite uiSprite, string atlasName)
    {
        SpriteProxy proxy = GetProxy(uiSprite);
        proxy.SetAtlasName(atlasName);
    }

    public static void SetSpriteProxy(GameObject spriteObj, string atlasName)
    {
        UISprite sprite = spriteObj.GetComponent<UISprite>();
        if (sprite == null) return;
        SpriteProxy proxy = GetProxy(sprite);
        proxy.SetAtlasName(atlasName);
    }

    public static void SetSpriteProxy(UISprite uiSprite, int atlasType)
    {
        string atlasName = GetAtalsPath(atlasType);
        SetSpriteProxy(uiSprite, atlasName);
    }

    public static void SetSpriteProxy(UISprite uiSprite, AtlasType type)
    {
        string atlasName = GetAtalsPath(type);
        SetSpriteProxy(uiSprite, atlasName);
    }


    private static readonly string generalAtlasPath = "Atlas/RoleIcon/RoleIcon";
    private static readonly string generalSkillAtalsPath = "Atlas/Modules/Skill";
    private static readonly string cardAtalsPath = "Atlas/Card/CardprefabTroopIconAtlas";
    private static readonly string troopSkillAtalsPath = "Atlas/SkillNew/Skill";
    private static readonly string cardMagicAtlasPath = "Atlas/Card/CardMagicSkillAtlas";
    private static readonly string farmLandAtlasPath = "Atlas/FarmLand/FarmLandAtlas";
    private static readonly string consumeAtlasPath = "Atlas/ItemIcon/ConsumeAtlas";
    private static readonly string itemAtlasPath = "Atlas/ItemIcon/ItemIcon";
    private static readonly string FuwenAtlasPath = "Atlas/Fuwen/FuwenAtlas";
    private static readonly string CardGeneralEquipAtlasPath = "Atlas/ItemIcon/CardGeneralEquipAtlas";

    public static string GetAtalsPath(AtlasType type)
    {
        switch (type)
        {
            case AtlasType.General:
                return generalAtlasPath;
            case AtlasType.GeneralSkill:
                return generalSkillAtalsPath;
            case AtlasType.Card:
                return cardAtalsPath;
            case AtlasType.TroopSkill:
                return troopSkillAtalsPath;
            case AtlasType.MagicCard:
                return cardMagicAtlasPath;
            case AtlasType.FarmLand:
                return farmLandAtlasPath;
            default:
                return null;
        }
    }

    public static string GetAtalsPath(int altas)
    {
        switch (altas)
        {
            case 0:
                return consumeAtlasPath;
            case 1: //物品(材料类（（将军icon，其id段为200047-200086） +（悬赏装备icon，其id段为250001-250008）+
                return itemAtlasPath;
            case 2:
                return CardGeneralEquipAtlasPath;
            case 3:
                return farmLandAtlasPath;
            case 4:
                return FuwenAtlasPath;
            default:
                return null;
        }
    }
}


public enum AtlasType
{
    General,
    GeneralSkill,
    Card,
    TroopSkill,
    MagicCard,
    FarmLand,
}


