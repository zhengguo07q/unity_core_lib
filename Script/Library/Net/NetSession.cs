// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetSession.cs
//  Creator 	:  
//  Date		: 2016-11-29
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;


[CustomLuaClass]
public class NetSession
{

    NetConnection netConnect;

    List<byte[]> frameList;
    public Action<int, ProtocolCommand> OnReceiveMessage;

    private string logHead = string.Empty;
    private string connectLogHead
    {
        get
        {
            if (logHead == string.Empty)
            {
                logHead = "[NetSession" + ConnectTime() + "]";
            }
            return logHead;
        }
    }


    public NetSession()
    {
        netConnect = new NetConnection();
        frameList = new List<byte[]>();
        netConnect.OnMessage = NetConnect_OnMessage;
    }


    public void Connect(string host, int port)
    {
        netConnect.ConnectServer(host, port);
    }


    public void SendData(int id, LuaTable obj)
    {
        var data = NetProtocalParser.Instance.Encode(obj,  id);
        byte[] frame = new byte[data.Length + 4];
        byte[] head = BitConverter.GetBytes(data.Length);
        Array.Copy(head, 0, frame, 0, 4);
        Array.Copy(data, 0, frame, 4, data.Length);
        netConnect.SendData(frame);
        NetLog.Info(connectLogHead, "protocal send id:", id);
    }


    public string ConnectTime()
    {
        if (netConnect != null)
            return netConnect.GetTryConnectTime();
        return string.Empty;
    }


    public bool IsConnecting()
    {
        return netConnect.IsConnecting();
    }


    public bool IsConnectFail()
    {
        return netConnect.IsConnectFail();
    }


    public void CloseConnect()
    {
        netConnect.OnMessage = null;
        netConnect.CloseNetConnection("netsession主动关闭socket");
        lock (frameList)
        {
            frameList.Clear();
        }
    }


    public void NetMsg_Deal()
    {
        byte[] frame = NetMsg_GetNext();
        while (frame != null)
        {
            NetMsg_Parse(frame);
            frame = NetMsg_GetNext();
        }
    }


    public float GetLastReceiveTime()
    {
        return netConnect.LastReceiveTime();
    }


    private byte[] NetMsg_GetNext()
    {
        byte[] frame = null;
        lock (frameList)
        {
            if (frameList.Count > 0)
            {
                frame = frameList[0];
                frameList.RemoveAt(0);
            }
        }
        return frame;
    }


    private void NetMsg_Parse(byte[] data)
    {
        int id;
        ProtocolCommand obj = NetProtocalParser.Instance.Decode(data, out id);
        NetLog.Info(connectLogHead, "protocal receive id", id);
        onReceiveMessageEvent(id, obj);
    }


    private void onReceiveMessageEvent(int key, ProtocolCommand msg)
    {
        if (OnReceiveMessage != null)
        {
            OnReceiveMessage(key, msg);
        }
    }


    private void NetConnect_OnMessage(byte[] data)
    {
        lock (frameList)
        {
            frameList.Add(data);
        }
    }
}

