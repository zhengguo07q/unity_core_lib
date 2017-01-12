// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetConnectBehaviour.cs
//  Creator 	:  
//  Date		: 2016-12-2
//  Comment		: 
// ***************************************************************


public class NetConnectBehaviour : WindowControl
{
    public NetClient NetClient { set; get; }


    public override void Initialize()
    {
        base.Initialize();

    }


    //第一次连接
    public virtual void FirstConnectSuccess(NetSession netSession)
    {
        NetClient.NetSessionIn(netSession);
    }


    public virtual void FirstConnectFail()
    {

    }


    //重连成功
    public virtual void ReconnectSuccess(NetSession netSession)
    {
        NetClient.NetSessionIn(netSession);
        //ApplicationGlobal.reconnectDialog.HideAll(); //关闭自动重连或者手动重连界面

        ////向服务器请求，重新接入
        //var cmd = new C2001_CG_CONN_REAUTH_128();
        //cmd.authId = GlobalData.LoginVo.Userid;
        //cmd.sessionKey = GlobalData.LoginVo.Session;
        //NetService.Send(cmd);
    }


    public virtual void ReconnectFailed()
    {

    }


    public virtual void CommunicationDisconnect()
    {

    }


    public virtual void NetStatus(NetClient.NetBusyStatus status)
    {

    }


    public virtual void NetConnectMode(NetReconnMgr.State reconnMode)
    {

    }


    public virtual void CommandHandle(int status)
    {

    }
}



