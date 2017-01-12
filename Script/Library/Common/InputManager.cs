// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: InputManager.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using UnityEngine;


public enum TouchStatus
{
    tsNormal,
    tsUp,
    tsDown,
    tsPress,
}

public class InputManager : SingletonMono<InputManager>
{
    public Vector3 postion = Vector3.zero;
    public TouchStatus status;
    public float touchTimeDelta;


    private LoggerView view;


    public override void Initialize()
    {
        view = LoggerView.Instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            status = TouchStatus.tsDown;
            touchTimeDelta = 0;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            status = TouchStatus.tsUp;
            touchTimeDelta = 0f;
        }
        else if (Input.GetButton("Fire1"))
        {
            status = TouchStatus.tsPress;
            touchTimeDelta += Time.deltaTime;
        }
        else
        {
            status = TouchStatus.tsNormal;
        }

        CheckMouse();
    }

    private void CheckMouse()
    {
        switch (status)
        {
            case TouchStatus.tsPress:
//#if UNITY_EDITOR
                view.SetVisible(touchTimeDelta);
//#endif
                break;

        }
    }


}

