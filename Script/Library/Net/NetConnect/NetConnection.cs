// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetConnection.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using System;
using System.Net;
using System.Net.Sockets;


public class NetConnection : NetConnectionBase
{
    public static float GameTime;

    private string addresss;
    private byte[] recvBuff;
    private NetFrameSplitor frameSplitor;

    private Socket tcpSocket;

    private bool connect_fail = false;
    private int connect_count = 0;

    private string connectTimeHead = string.Empty;
    private string tryConnectTime = string.Empty;

    private float lastReceiveTime = 0; //秒为单位


    public override void CloseNetConnection(string whoClose)
    {
        NetLog.Info(connectTimeHead, "CloseNetConnection, when: ", whoClose, " " + this.addresss);
        if (this.tcpSocket != null)
        {
            this.tcpSocket.Close();
        }
    }


    public override void ConnectServer(string host, int port)
    {
        if (connect_count > 0)
            throw new Exception("[NetConnection]this could use only once, please re-create one");
        connect_count++;

        tryConnectTime = System.DateTime.Now.ToString("hh:mm:ss-fff");
        connectTimeHead = "[NetConnection " + tryConnectTime + "]";

        IPAddress address;
        if (!IPAddress.TryParse(host, out address))
        {
            address = Dns.GetHostEntry(host).AddressList[0];
        }

        this.tcpSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.tcpSocket.NoDelay = true;
        this.frameSplitor = new NetFrameSplitor();
        this.recvBuff = new byte[4096];

        this.addresss = "[" + host + "," + port + "]";

        try
        {
            this.tcpSocket.BeginConnect(new IPEndPoint(address, port), new AsyncCallback(this.On_Connect), null);
        }
        catch (Exception exception)
        {
            NetLog.Info(connectTimeHead, "ConnectServer, Exception: " + exception.Message + this.addresss);
            this.CloseNetConnection("连接失败，关闭socket");
        }
    }


    private void On_Connect(IAsyncResult asr)
    {
        try
        {
            this.tcpSocket.EndConnect(asr);
        }
        catch (Exception exception)
        {
            NetLog.Info(connectTimeHead, "On_Connect, Exception: " + exception.Message + this.addresss);
            this.CloseNetConnection("连接失败，关闭socket");
            this.connect_fail = true;
            return;
        }

        if (!IsConnecting())
        {
            NetLog.Info(connectTimeHead, "ConnectFailed " + this.addresss);
            this.CloseNetConnection("连接失败，关闭socket");
            this.connect_fail = true;
            return;
        }
        lastReceiveTime = GameTime;
        this.ToRead();
        NetLog.Info(connectTimeHead, "On_Connect, Connect SUCCESS: ", this.tcpSocket.LocalEndPoint, " ==> ", this.tcpSocket.RemoteEndPoint);
    }


    private void OnReceiveDone(IAsyncResult asr)
    {
        int length = 0;
        try
        {
            length = this.tcpSocket.EndReceive(asr);
            if (length < 1)
            {
                NetLog.Error(connectTimeHead, "OnRead, Server Close Socket, bytesRead == " + length + this.addresss);
                this.CloseNetConnection("接受数据长度为0，关闭socket");
            }
            else
            {
                this.frameSplitor.Write(recvBuff, 0, length);
                byte[] frameData = this.frameSplitor.Split();
                while (frameData != null)
                {
                    this.onMessageEvent(frameData);
                    frameData = this.frameSplitor.Split();
                }
                this.ToRead();
                lastReceiveTime = GameTime;
            }
        }
        catch (Exception exception3)
        {
            NetLog.Error(connectTimeHead, "OnRead, 拆包失败或者socket接受异常: " + exception3.Message + this.addresss);
            this.CloseNetConnection("拆包失败或者socket接受异常，关闭socket");
        }
    }


    private void OnSendDone(IAsyncResult r)
    {
        try
        {
            this.tcpSocket.EndSend(r);
        }
        catch (Exception exception3)
        {
            NetLog.Error(connectTimeHead, "OnWrite, Exception: " + exception3.Message + this.addresss);
            this.CloseNetConnection("发送数据失败，关闭socket");
        }
    }


    public override void SendData(byte[] data)
    {
        if (IsConnecting())
        {
            try
            {
                this.tcpSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.OnSendDone), null);
            }
            catch (Exception exception2)
            {
                NetLog.Error(connectTimeHead, "BeginWrite, Exception: " + exception2.Message + this.addresss);
                this.CloseNetConnection("发送数据失败，关闭socket");
            }
        }
    }


    private void ToRead()
    {
        this.tcpSocket.BeginReceive(recvBuff, 0, recvBuff.Length, SocketFlags.None, new AsyncCallback(this.OnReceiveDone), null);
    }


    public override bool IsConnecting()
    {
        if (this.tcpSocket == null)
        {
            return false;
        }
        return this.tcpSocket.Connected;
    }


    public override bool IsConnectFail()
    {
        return connect_fail;
    }


    public override string GetTryConnectTime()
    {
        return tryConnectTime;
    }


    public float LastReceiveTime()
    {
        return lastReceiveTime;
    }

}


