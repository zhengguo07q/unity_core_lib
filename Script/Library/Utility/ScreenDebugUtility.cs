// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScreenDebugUtility.cs
//  Creator 	: zg
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class ScreenDebugUtility : SingletonMono<ScreenDebugUtility>
{

    public void OnGUI()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)

#endif
     //   FastLuaUtility.ScreenUtility();
    }


    public static new ScreenDebugUtility GetInstance()
    {
        return Instance;
    }
}

