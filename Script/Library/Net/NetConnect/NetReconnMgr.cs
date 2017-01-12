// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetReconnMgr.cs
//  Creator 	:  
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System;


public class NetReconnMgr : MonoBehaviour
{

    public Action<NetSession> connectSuccess;
    public Action connectFail;

    const int autoCount = 3;
    const int maxManualCount = 3;

    string host;
    int port;

    int manualCount = 0;

    public enum State
    {
        AutoConnect, //自动重连阶段
        ManualConnnect, //手动重连阶段
    }

    State state;

    NetMultiConnect netMultiConnect;
    NetOnceConnect netOnceConnect;

    NetConnectBehaviour connectBehaviour;


    public static void Reconn(string _host, int _port, Action<NetSession> connSuccess, Action connFail)
    {
        GameObject obj = new GameObject("NetReconnMgr");
        NetReconnMgr netReconn = obj.AddComponent<NetReconnMgr>();
        netReconn.StartConnect(_host, _port, connSuccess, connFail);
    }


    protected void StartConnect(string _host, int _port, Action<NetSession> connSuccess, Action connFail)
    {
        this.connectSuccess = connSuccess;
        this.connectFail = connFail;
        this.host = _host;
        this.port = _port;
        state = State.AutoConnect;
        netMultiConnect = new NetMultiConnect(this.host, this.port, autoCount);
    }


    protected void Update()
    {
        if(state == State.AutoConnect)
        {
            netMultiConnect.Update();
            if(netMultiConnect.ConnectSuccess())
            {
                GameObject.Destroy(gameObject);
                this.connectSuccess(netMultiConnect.NetSession);
                return;
            }
            if(netMultiConnect.ConnectFail())
            {
                state = State.ManualConnnect;
                connectBehaviour.NetConnectMode(State.ManualConnnect);
    //            ApplicationGlobal.reconnectDialog.ShowMannualConnect(ConnectOnce); //显示手动重连界面
            }
        }

        if(state == State.ManualConnnect)
        {
            if (netOnceConnect == null)
                return;

            netOnceConnect.Update();
            if(netOnceConnect.ConnectSucess())
            {
                GameObject.Destroy(gameObject);
                this.connectSuccess(netOnceConnect.NetSession);
                return;
            }
            if (netOnceConnect.ConnectFail())
            {
                NetLog.Error("[NetReconnMgr] 手动重连 第" + manualCount + "次失败");
                if(manualCount >= maxManualCount)
                {
                    NetLog.Error("[NetReconnMgr] 手动重连三次失败，放弃重连");
                    GameObject.Destroy(gameObject);
                    connectFail();
                }
                else
                {
                    netOnceConnect = null;
      //              ApplicationGlobal.reconnectDialog.ShowMannualConnect(ConnectOnce); //显示手动重连界面
                }
            }
        }
    }


    protected void ConnectOnce()
    {
        manualCount++;
        netOnceConnect = new NetOnceConnect(this.host, this.port);
 //       ApplicationGlobal.reconnectDialog.ShowAutoConnection();
    }
}
