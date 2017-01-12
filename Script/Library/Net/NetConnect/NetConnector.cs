// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetConnector.cs
//  Creator 	:  
//  Date		: 2016-12-1
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class NetOnceConnect
{
    NetSession netSession;

    float timeTick = 0;

    public const float MaxSpan = 10; //最大连接超时8s
    public const float minSpan = 1;


    public NetOnceConnect(string host, int port)
    {
        netSession = new NetSession();
        netSession.Connect(host, port);
    }


    public void Update()
    {
        timeTick += Time.deltaTime;
    }


    public bool ConnectSucess()
    {
        if (netSession.IsConnecting())
        {
            return true;
        }
        return false;
    }


    public bool ConnectFail()
    {
        if (netSession.IsConnectFail())
        {
            if (timeTick > minSpan)
            {
                return true;
            }
        }

        if (timeTick > MaxSpan)
        {
            return true;
        }
        return false;
    }


    public NetSession NetSession
    {
        get
        {
            return netSession;
        }
    }
}


public class NetMultiConnect
{

    NetOnceConnect onceConnect;

    int maxCount = 0;
    int curCount = 0;

    string host;
    int port;


    public NetMultiConnect(string _host, int _port, int count = 1)
    {
        this.maxCount = count;
        this.host = _host;
        this.port = _port;
        onceConnect = new NetOnceConnect(host, port);
        curCount++;
    }


    public void Update()
    {
        onceConnect.Update();

        if (onceConnect.ConnectFail())
        {
            NetLog.Error("[NetMultiConnect] 自动重连 第" + curCount +"次失败");
            if (curCount < maxCount)
            {
                onceConnect = new NetOnceConnect(host, port);
                curCount++;
            }
        }
    }


    public bool ConnectSuccess()
    {
        if (onceConnect.ConnectSucess())
        {
            return true;
        }
        return false;
    }


    public bool ConnectFail()
    {
        if (onceConnect.ConnectFail())
        {
            if (curCount >= maxCount)
            {
                return true;
            }
        }
        return false;
    }


    public NetSession NetSession
    {
        get
        {
            return onceConnect.NetSession;
        }
    }
}
