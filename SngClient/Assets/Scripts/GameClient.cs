using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;

public partial class GameClient : MonoBehaviour {

    string m_serverAddr = "localhost";
    string m_villeName = "Ville";
    string m_nickName = "Player";
    //text to be shown on the button
    string m_loginButtonText = "Connect";

    // uses while the scene is 'error mode'
    string m_failMessage = "";

    NetClient m_netClient = new NetClient();

    // for sending client-to-server messages
    SocialGameC2S.Proxy m_C2SProxy = new SocialGameC2S.Proxy();
    SocialGameS2C.Stub m_S2CStub = new SocialGameS2C.Stub();

    // for P2P communication
    SocialGameC2C.Proxy m_C2CProxy = new SocialGameC2C.Proxy();
    SocialGameC2C.Stub m_C2CStub = new SocialGameC2C.Stub();
    
    enum State
    {
        InLobby,
        Connecting,
        LoggingOn,
        StandBy,
        InGame,
        Failed,
    }

    private State m_state = State.InLobby;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        SetWaitingRoom();

        m_S2CStub.ReplyLogon = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, int result, string comment, bool isMaster) =>
        {
            m_myP2PGroupID = (HostID)groupID;

            if(result == 0 && m_state == State.LoggingOn) // ok
            {
                m_state = State.StandBy;
                waitingRoom.SetActive(true);
                this.isMaster = isMaster;
            }
            else
            {
                m_state = State.Failed;
                m_failMessage = "Logon failed Error=" + comment;
            }
            return true;
        };

        Start_InVilleRmiStub();
        Start_InWaitingRmiStub();
    }
    

	void Update () {
        m_netClient.FrameMove();    //FrameMove를 통해 이벤트 통지

        switch(m_state)
        {
            case State.StandBy:
                Update_InVille();
                break;
        }
	}

    public void OnGUI()
    {
        switch(m_state)
        {
            case State.InLobby:
            case State.Connecting:
            case State.LoggingOn:
                OnGUI_Logon();
                break;
            case State.StandBy:
                //OnGUI_InVille();
                break;
            case State.Failed:
                GUI.Label(new Rect(10, 30, 200, 80), m_failMessage);
                if (GUI.Button(new Rect(10, 100, 180, 30), "Quit"))
                {
                    Application.Quit();
                }
                break;
        }
    }

    void OnGUI_Logon()
    {
        GUI.Label(new Rect(10, 10, 300, 70), "ProudNet Project : \nA Basic Realtime Social Game");
        GUI.Label(new Rect(10, 60, 180, 30), "Server Address");
        m_serverAddr = GUI.TextField(new Rect(10, 80, 180, 30), m_serverAddr);
        GUI.Label(new Rect(10, 110, 180, 30), "World Name");
        m_villeName = GUI.TextField(new Rect(10, 130, 180, 30), m_villeName);
        GUI.Label(new Rect(10, 160, 180, 30), "Nick Name");
        m_nickName = GUI.TextField(new Rect(10, 180, 180, 30), m_nickName);

        if (GUI.Button(new Rect(10, 220, 100, 30), m_loginButtonText))
        {
            if (m_state == State.InLobby)
            {
                m_state = State.Connecting;
                m_loginButtonText = "Connecting...";
                IssueConnect(); // attemp to connect and logon
            }
        }
    }
    
    

    private void IssueConnect()
    {
        m_netClient.AttachProxy(m_C2SProxy);
        m_netClient.AttachStub(m_S2CStub);

        m_netClient.AttachProxy(m_C2CProxy);
        m_netClient.AttachStub(m_C2CStub);

        m_netClient.JoinServerCompleteHandler = (ErrorInfo info, ByteArray replyFromServer) =>
        {
            if (info.errorType == ErrorType.ErrorType_Ok)
            {
                m_state = State.LoggingOn;
                m_loginButtonText = "Logging on...";

                // try to join the specified ville by name given by the user.
                m_C2SProxy.RequestLogon(HostID.HostID_Server, RmiContext.ReliableSend, m_villeName, m_nickName, false);
                
            }
            else
            {
                m_state = State.Failed;
                m_loginButtonText = "FAIL!";
                m_failMessage = info.ToString();
            }
        };

        //if the server connection is down, we should prepare for exit.
        m_netClient.LeaveServerHandler = (ErrorInfo info) =>
        {
            m_state = State.Failed;
            m_failMessage = "Disconnected from server: " + info.ToString();
        };

        //fill parameters and go
        NetConnectionParam cp = new NetConnectionParam();
        cp.serverIP = m_serverAddr;
        cp.clientAddrAtServer = "192.168.219.103";
        cp.serverPort = 15001;
        cp.protocolVersion = new Nettention.Proud.Guid("{0x612df376,0x37a9,0x49d5,{0x8e,0xc0,0x8f,0x41,0xb8,0x4f,0x11,0x56}}");

        m_netClient.Connect(cp);   
    }

    private void OnDestroy()
    {
        m_netClient.Dispose();
    }


}
