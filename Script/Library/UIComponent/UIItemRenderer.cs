// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: UIItemRenderer.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using SLua;


[CustomLuaClass]
public class UIItemRenderer : UIScriptBehaviour
{
    public TypeEventHandler eventHandler;

    public void EventHandler(string type, object data)
    {
        if (eventHandler != null)
            eventHandler(type, data);
    }
}
