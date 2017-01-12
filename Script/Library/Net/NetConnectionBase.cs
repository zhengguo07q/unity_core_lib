// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetConnectionBase.cs
//  Creator 	:  
//  Date		: 2016-11-30
//  Comment		: 
// ***************************************************************


using System;


public abstract class NetConnectionBase
{

    public Action<byte[]> OnMessage;

    public abstract void CloseNetConnection(string whoClose);
    public abstract void ConnectServer(string host, int port);
    public abstract void SendData(byte[] byteBuf);

    public abstract bool IsConnecting();

    public abstract bool IsConnectFail();

    public abstract string GetTryConnectTime();

    protected void onMessageEvent(byte[] data)
    {
        if (OnMessage != null)
        {
            OnMessage(data);
        }
    }

}