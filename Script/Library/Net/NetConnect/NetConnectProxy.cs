// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetConnectProxy.cs
//  Creator 	:  
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System;


public class NetConnectProxy : MonoBehaviour
{

    Action<NetSession> connectSuccess;
    Action connectFail;

    NetOnceConnect netOnceConnect;


    public static void Connect(string host, int port, Action<NetSession> ConnectSuccess, Action ConnectFail)
    {
        GameObject obj = new GameObject("NetConnectBehaviour");
        NetConnectProxy conn = obj.AddComponent<NetConnectProxy>();
        conn.netOnceConnect = new NetOnceConnect(host, port);
        conn.connectSuccess = ConnectSuccess;
        conn.connectFail = ConnectFail;
    }


    protected void Update()
    {
        netOnceConnect.Update();
        if(netOnceConnect.ConnectSucess())
        {
            GameObject.Destroy(gameObject);
            connectSuccess(netOnceConnect.NetSession);
            return;
        }

        if(netOnceConnect.ConnectFail())
        {
            GameObject.Destroy(gameObject);
            //NetLog.Error("[NetAutoReconnBehaviour] 重连失败，放弃重连");
            connectFail();
        }
    }
}
