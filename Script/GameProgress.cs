// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: GameProgress.cs
//  Creator 	: zg
//  Date		: 
//  Comment		: 启动的时候还没有启动好LUA， 所以这里不允许使用LUA
// ***************************************************************


using SLua;
using UnityEngine;


[CustomLuaClass]
public class GameProgress : WindowLayerBase
{
    public static GameProgress Instance;

    public UIProgressBar progressBar;
    private UILabel logLbl;
    private UILabel titleLbl;
    private UILabel versionLbl;

    public override void Initialize()
    {
        progressBar = Find("progressBar") as UIProgressBar;
        logLbl = Find("logLbl") as UILabel;
        titleLbl = Find("titleLbl") as UILabel;
        versionLbl = Find("versionLbl") as UILabel;

        (Find("waringLbl") as UILabel).text = LS.StrFromXml("jiankangzhonggao");
        Instance = this;
    }

	
	protected override void Update ()
    {
        if (AssetStatusManager.Instance.remoteConfProject != null)
        {
            versionLbl.text = ("v" + AssetStatusManager.Instance.remoteConfProject.version);
        }
    }


    public void SetProgressTxt(string message, string progressTxt, float progressValue)
    {
        logLbl.text = message;
        titleLbl.text = progressTxt;
        progressBar.value = progressValue;
    }


    public override void Dispose()
    {

    }
}
