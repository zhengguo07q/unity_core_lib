// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetProxy.cs
//  Creator 	:  
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using UnityEngine;
using System.Collections.Generic;
using SLua;


public delegate void ProtocolAction(object obj);


[CustomLuaClass]
public class NetClient : SingletonMono<NetClient>
{
    public enum NetBusyStatus
    {
        nbsNormal,
        nbsBusy,
    };

    public enum State //游戏阶段的状态
    {
        UnConnected,
        ReConnecting, //连接中
        Connected //已连接， 数据读写中
    }

    State netState = State.UnConnected;

    NetSession netSession;

    string host;
    int port;

    private const float NetBusyMaskTime = 5; //5s
    private const float NetDisconMaskTime = 20; //10s

    private readonly Dictionary<int, List<ProtocolAction>> cmdMap = new Dictionary<int, List<ProtocolAction>>();

    public  NetConnectBehaviour netConnectBehaviour;

    private string LogHead
    {
        get
        {
            string connectTime = string.Empty;
            if (netSession != null)
            {
                connectTime = netSession.ConnectTime();
            }
            return "[NetClient " + connectTime + "]";
        }
    }


    public State NetState
    {
        get
        {
            return netState;
        }
    }


    public NetScreenLog screenLog = new NetScreenLog();


    public override void Initialize()
    {
        NetLog.logExtra = ScreenLog;
    }


    public override void Destroy()
    {
        NetLog.Info("[NetClient]应用程序关闭");
        CloseNetSession();
    }


    public void BindScript(string scriptResource)
    {
        netConnectBehaviour = ScriptManager.Instance.WrapperScriptBehaviour<SupportNetConnectBehaviour>(this.gameObject, "", scriptResource);
        netConnectBehaviour.NetClient = this;
    }


    //登陆连接
    public void LoginConnect(string host, int port)
    {
        CloseNetSession();
        netState = State.UnConnected;

        this.host = host;
        this.port = port;
        NetConnectProxy.Connect(host, port, netConnectBehaviour.FirstConnectSuccess, netConnectBehaviour.FirstConnectFail);
    }


    //发送消息
    public void SendData(int id, LuaTable obj)
    {
        if (netState == State.Connected)
        {
            netSession.SendData(id, obj);
        }
        else
        {
            NetLog.Error(LogHead, "unconnected netclient, send failed");
        }
    }

    //消息处理
    private void Notify(int key, ProtocolCommand msg)
    {
        if (cmdMap.ContainsKey(key))
        {
            List<ProtocolAction> msgList = cmdMap[key];
            for (int i = 0; i < msgList.Count; i++)
            {
                ProtocolAction action = msgList[i];
                action(msg.GetLuaProtocol());
            }
        }
    }


    void Update()
    {

        NetConnection.GameTime = Time.time;
        if (netState == State.Connected)
        {
            ConnectedUpdate();
        }
    }


    void OnGUI()
    {
        screenLog.GUIUpdate();
    }


    void ScreenLog(string str)
    {
        screenLog.Add(str);
    }


    public void NetSessionIn(NetSession netSession)
    {
        if (netSession == null)
        {
            NetLog.Error("[NetClient] empty netsession");
            return;
        }

        this.netSession = netSession;
        this.netSession.OnReceiveMessage = Notify;
        this.netState = State.Connected;
    }


    void ConnectedUpdate()
    {
        netSession.NetMsg_Deal(); //处理网络消息，消息处理函数为notify

        if (netState != State.Connected) //有可能在 netSession.NetMsg_Deal 处理中把关闭了连接
            return;

        if (!netSession.IsConnecting())
        {
            NetLog.Info(LogHead, "network disconnect, Reconnect");
            RealReconnect();
            return;
        }

        float timeSpan = Time.time - netSession.GetLastReceiveTime();
        if (timeSpan > NetDisconMaskTime) //超过7s没有收到任何包，就重连
        {
            NetLog.Info(LogHead, "long time no message, Reconnect");
            CloseNetSession();
            RealReconnect();
        }
        else if (timeSpan > NetBusyMaskTime) //超过5s没有收到任何包，就提延迟窗口,（心跳包是服务器三秒推一次)
        {
            netConnectBehaviour.NetStatus(NetBusyStatus.nbsBusy);
        }
        else
        {
            netConnectBehaviour.NetStatus(NetBusyStatus.nbsNormal);
        }
    }


    private void RealReconnect()
    {
        netConnectBehaviour.CommunicationDisconnect();
        netState = State.ReConnecting;
        NetReconnMgr.Reconn(this.host, this.port, netConnectBehaviour.ReconnectSuccess, netConnectBehaviour.ReconnectFailed);
    }


    private void CloseNetSession()
    {
        if (netSession != null)
        {
            netSession.CloseConnect();
        }
    }

    public void ShutDownSocketTest()
    {
        CloseNetSession();
    }


    private void addListener(int cmdKey, ProtocolAction handler)
    {
        List<ProtocolAction> listeners;
        if (cmdMap.ContainsKey(cmdKey))
            listeners = cmdMap[cmdKey];
        else
        {
            listeners = new List<ProtocolAction>();
            cmdMap[cmdKey] = listeners;
        }

        if (!listeners.Contains(handler))
            listeners.Add(handler);
    }


    private void removeListener(int cmdKey, ProtocolAction handler)
    {
        if (cmdMap.ContainsKey(cmdKey) && cmdMap[cmdKey].Contains(handler))
            cmdMap[cmdKey].Remove(handler);
    }


    public static void Add(int cmdKey, ProtocolAction handler)
    {
        Instance.addListener(cmdKey, handler);
    }


    public static void Remove(int cmdKey, ProtocolAction handler)
    {
        Instance.removeListener(cmdKey, handler);
    }


    public static void Send(int id, LuaTable data)
    {
        Instance.SendData(id, data);
    }


    public static void Connect(string host, int port)
    {
        Instance.LoginConnect(host, port);
    }


    public new static NetClient GetInstance()
    {
        return Instance;
    }
}
