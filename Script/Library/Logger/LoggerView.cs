// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LoggerView.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class LoggerView : SingletonMono<LoggerView>
{
    public bool isShowThis = false;
    public float homeInterval = 5;
    public float homeButtonTime = 0;
    public bool isLogger = false;


    public void SetVisible(float delta)
    {
        if (isLogger == false)
            return;
        if (delta - homeButtonTime > homeInterval)
        {
            homeButtonTime = delta;
            isShowThis = !isShowThis;
        }
    }


    void OnGUI()
    {
        if (isShowThis)
        {
            GUI.Label(new Rect(10, 10, Screen.width - 20f, Screen.height - 20), Logger.GetUIMessage() + "\nTouch!");
        }
    }
}