// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: SupportNetConnectBehaviour.cs
//  Creator 	:  
//  Date		: 2016-12-2
//  Comment		: 
// ***************************************************************


using SLua;


[CustomLuaClass]
public class SupportNetConnectBehaviour : NetConnectBehaviour
{
    protected LuaFunction initializeFunc;
    protected LuaFunction disposeFunc;
    protected LuaFunction firstConnectSuccessFunc;
    protected LuaFunction firstConnectFailFunc;
    protected LuaFunction reconnectSuccessFunc;
    protected LuaFunction reconnectFailedFunc;
    protected LuaFunction communicationDisconnectFunc;
    protected LuaFunction netStatusFunc;
    protected LuaFunction netConnectModeFunc;
    protected LuaFunction commandHandleFunc;


    protected override void CacheLuaFunction()
    {
        base.CacheLuaFunction();
        initializeFunc = ScriptHelper.GetFunction(LuaTable, "initialize");
        disposeFunc = ScriptHelper.GetFunction(LuaTable, "dispose");
        firstConnectSuccessFunc = ScriptHelper.GetFunction(LuaTable, "firstConnectSuccess");
        firstConnectFailFunc = ScriptHelper.GetFunction(LuaTable, "firstConnectFail");
        reconnectSuccessFunc = ScriptHelper.GetFunction(LuaTable, "reconnectSuccess");
        reconnectFailedFunc = ScriptHelper.GetFunction(LuaTable, "reconnectFailed");
        communicationDisconnectFunc = ScriptHelper.GetFunction(LuaTable, "communicationDisconnect");
        netStatusFunc = ScriptHelper.GetFunction(LuaTable, "netStatus");
        netConnectModeFunc = ScriptHelper.GetFunction(LuaTable, "netConnectMode");
        commandHandleFunc = ScriptHelper.GetFunction(LuaTable, "commandHandle");
    }


    public override void Initialize()
    {
        base.Initialize();
        if (initializeFunc != null)
        {
            initializeFunc.call(LuaTable);
        }
    }


    public override void Dispose()
    {
        base.Dispose();
        if (disposeFunc != null)
        {
            disposeFunc.call(LuaTable);
        }
    }


    public override void FirstConnectSuccess(NetSession netSession)
    {
        base.FirstConnectSuccess(netSession);
        if (firstConnectSuccessFunc != null)
        {
            firstConnectSuccessFunc.call(LuaTable);
        }
    }


    public override void FirstConnectFail()
    {
        base.FirstConnectFail();
        if (firstConnectFailFunc != null)
        {
            firstConnectFailFunc.call(LuaTable);
        }
    }


    //重连成功
    public override void ReconnectSuccess(NetSession netSession)
    {
        base.ReconnectSuccess(netSession);
        if (reconnectSuccessFunc != null)
        {
            reconnectSuccessFunc.call(LuaTable);
        }
    }


    public override void ReconnectFailed()
    {
        base.ReconnectFailed();
        if (reconnectFailedFunc != null)
        {
            reconnectFailedFunc.call(LuaTable);
        }
    }


    public override void CommunicationDisconnect()
    {
        base.CommunicationDisconnect();
        if (communicationDisconnectFunc != null)
        {
            communicationDisconnectFunc.call(LuaTable);
        }
    }


    public override void NetStatus(NetClient.NetBusyStatus status)
    {
        base.NetStatus(status);
        if (netStatusFunc != null)
        {
            netStatusFunc.call(LuaTable);
        }
    }


    public override void NetConnectMode(NetReconnMgr.State reconnMode)
    {
        base.NetConnectMode(reconnMode);
        if (netConnectModeFunc != null)
        {
            netConnectModeFunc.call(LuaTable);
        }
    }


    public override void CommandHandle(int status)
    {
        base.CommandHandle(status);
        if (commandHandleFunc != null)
        {
            commandHandleFunc.call(LuaTable);
        }
    }
}

