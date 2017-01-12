// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LabelProxy.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class LabelProxy :MonoBehaviour
{
    public enum FontType
    {
        UnityFont,
        UIFont,
    }

    public string fontName;
    public FontType fontType;

    public bool isAsyncLoad = true;


    UILabel label;

    UILabel Label
    {
        get
        {
            if(label == null)
            {
                label = GetComponent<UILabel>();
            }
            return label;
        }
    }

    private Asset asset;

    void Awake()
    {
        SetFont();
    }


    void OnDestroy()
    {
        if(asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
    }


    public void SetFontName(string font, FontType fontType)
    {
        this.fontType = fontType;
        SetFontName(font);
    }


    public void SetFontName(string font)
    {
        if (string.IsNullOrEmpty(font))
            return;

        if (font == this.fontName)
            return;

        if(asset != null)
        {
            asset.ReleaseRef();
            asset = null;
        }
        this.fontName = font;
        SetFont();
    }


    private void SetFont()
    {
        if (string.IsNullOrEmpty(fontName))
            return;

        if (fontType == FontType.UnityFont)
        {
            this.Label.trueTypeFont = (Font)ResourceLoader.LoadFromResources(fontName);
            return;
        }

        if (!isAsyncLoad)
        {
            LoadDone(AssetLoader.Instance.SyncLoad(fontName));
        }
        else
        {
            AssetLoader.Instance.AsyncLoad(fontName, LoadDone);
        }

    }


    private void LoadDone(Asset asset)
    {
        if (asset == null)
            return;

        if (asset.name != this.fontName)
            return;

        if(this.fontType == FontType.UnityFont)
        {
            this.Label.trueTypeFont = (Font)asset.mainObject;
        }
        else
        {
            this.Label.bitmapFont = asset.GetFromObject<UIFont>();
        }
        asset.AddRef();
        this.asset = asset;
    }
}

