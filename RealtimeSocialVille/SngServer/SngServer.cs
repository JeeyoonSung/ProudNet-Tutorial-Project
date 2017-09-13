using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nettention.Proud;
using System.Threading;

namespace SngServer
{
    class SngServer
    {
        public bool m_runLoop;

        private NetServer m_netServer = new NetServer();

        private Nettention.Proud.ThreadPool netWorkerThreadPool = new Nettention.Proud.ThreadPool(8);
        private Nettention.Proud.ThreadPool userWorkerThreadPool = new Nettention.Proud.ThreadPool(8);

        // RMI proxy for server-to-client messaging
        internal SocialGameS2C.Proxy m_S2CProxy = new SocialGameS2C.Proxy();
        internal SocialGameC2S.Stub m_C2SStub = new SocialGameC2S.Stub();

        private ConcurrentDictionary<String, Ville_S> m_villes = new ConcurrentDictionary<string, Ville_S>();
        // guides which client is in which ville.
        private ConcurrentDictionary<HostID, Ville_S> m_remoteClients = new ConcurrentDictionary<HostID, Ville_S>();
        
        public SngServer()
        {
            m_runLoop = true;

            m_netServer.AttachProxy(m_S2CProxy);
            m_netServer.AttachStub(m_C2SStub);

            m_netServer.ConnectionRequestHandler = (AddrPort clientAddr, ByteArray userDataFromClient, ByteArray reply) =>
                {
                    reply = new ByteArray();
                    reply.Clear();
                    return true;
                };

            m_netServer.ClientHackSuspectedHandler = (HostID clientID, HackType hackType) =>
                {

                };

            m_netServer.ClientJoinHandler = (NetClientInfo clientInfo) =>
                {
                    Console.WriteLine("OnClientJoin: {0} ", clientInfo.hostID);
                };

            m_netServer.ClientLeaveHandler = (NetClientInfo clientInfo, ErrorInfo errorInfo, ByteArray comment) =>
                {
                    Console.WriteLine("OnClientLeave: {0}", clientInfo.hostID);

                    Monitor.Enter(this);
                    Ville_S ville;

                    //remove the client and play info, and then remove the ville if it is empty.
                    if (m_remoteClients.TryGetValue(clientInfo.hostID, out ville))
                    {
                        RemoteClient_S clientValue;
                        Ville_S villeValue;
                        bool changeMaster;
                        int changedMasterValue;

                        ville.m_players.TryRemove(clientInfo.hostID, out clientValue);
                        m_remoteClients.TryRemove(clientInfo.hostID, out villeValue);
                        
                        //방장 바꾸기, 칸바꾸기
                        ville.UpdateNextLoc(clientValue.waitingIdx);

                        changeMaster = false;
                        changedMasterValue = -1;
                        
                        if (clientInfo.hostID == ville.masterID && ville.m_players != null)    // master leaved
                        {
                            changeMaster = true;
                            if(ville.m_players.Count != 0)
                                ville.masterID = ville.m_players.Keys.First();
                            changedMasterValue = (int)ville.masterID;
                            Console.WriteLine("masterID changed: {0}->{1}", clientInfo.hostID, changedMasterValue);
                        }
                        
                        if (ville.m_players.Count == 0)
                        {
                            UnloadVille(ville);
                        }
                        else
                        {
                            // notify to all player
                            foreach (KeyValuePair<HostID, RemoteClient_S> iPlayer in ville.m_players)
                            {
                                m_S2CProxy.NotifyPlayerLeave(iPlayer.Key, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, clientValue.waitingIdx, changeMaster, changedMasterValue);
                            }
                        }
                    }
                    Monitor.Exit(this);
                };

            m_netServer.ErrorHandler = (ErrorInfo errorInfo) =>
                {
                    Console.WriteLine("OnError: {0}", errorInfo.ToString());
                };

            m_netServer.WarningHandler = (ErrorInfo errorInfo) =>
                {
                    Console.WriteLine("OnWarning! {0}", errorInfo.ToString());
                };

            m_netServer.ExceptionHandler = (Exception e) =>
                {
                    Console.WriteLine("OnWarning! {0}", e.Message.ToString());
                };

            m_netServer.InformationHandler = (ErrorInfo errorInfo) =>
                {
                    Console.WriteLine("OnInformation! {0}", errorInfo.ToString());
                };

            m_netServer.NoRmiProcessedHandler = (RmiID rmiID) =>
                {
                    Console.WriteLine("OnNoRmiProcessed! {0}", rmiID);
                };

            m_netServer.P2PGroupJoinMemberAckCompleteHandler = (HostID groupHostID, HostID memberHostID, ErrorType result) =>
                {

                };

            m_netServer.TickHandler = (Object context) =>
                {

                };

            m_netServer.UserWorkerThreadBeginHandler = () =>
                {

                };

            m_netServer.UserWorkerThreadEndHandler = () =>
                {

                };
            m_C2SStub.RequestLogon = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, String villeName, String nickName, bool isNewVille) =>
                {
                    Monitor.Enter(this);
                    Ville_S ville;
                    HostID[] list = m_netServer.GetClientHostIDs();

                    if (!m_villes.TryGetValue(villeName, out ville))
                    {
                        // create new one
                        ville = new Ville_S();
                        ville.m_p2pGroupID = m_netServer.CreateP2PGroup(list, new ByteArray());
                        ville.masterID = remote;

                        Console.WriteLine("m_p2pGroupID : {0}", ville.m_p2pGroupID);

                        NetClientInfo info = m_netServer.GetClientInfo(list.Last());
                        Console.WriteLine("Client HostID : {0}, IP:Port : {1}:{2}", info.hostID, info.tcpAddrFromServer.IPToString(), info.tcpAddrFromServer.port);
                        
                        // load ville info
                        m_villes.TryAdd(villeName, ville);
                        ville.m_name = villeName;
                    }
                    
                    m_villes.TryGetValue(villeName, out ville);
                    if (ville.canJoin == false)
                    {
                        // can't join
                        m_S2CProxy.ReplyLogon(remote, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, 1, "빈자리가 없습니다.",false);
                    }
                    else
                    {
                        m_S2CProxy.ReplyLogon(remote, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, 0, "", (remote == ville.masterID));
                        MoveRemoteClientToLoadedVille(remote, ville, nickName);
                    }
                    
                    Monitor.Exit(this);
                    return true;
                };

            m_C2SStub.RequestAddTree = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 position) =>
                {
                    Monitor.Enter(this);

                    Ville_S ville;
                    HostID[] list = m_netServer.GetClientHostIDs();
                    WorldObject_S tree = new WorldObject_S();

                    if (m_remoteClients.TryGetValue(remote, out ville))
                    {
                        // add the tree
                        tree.m_position = position;
                        tree.m_id = ville.m_nextNewID;
                        ville.m_worldOjbects.TryAdd(tree.m_id, tree);
                        ville.m_nextNewID++;
                    }
                    else
                    {
                        ville = new Ville_S();
                    }
                    

                    foreach (HostID id in list)
                    {
                        // notify the tree's creation to users
                        m_S2CProxy.NotifyAddTree(id, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, tree.m_id, tree.m_position);
                    }
                    Monitor.Exit(this);
                    return true;
                };

            m_C2SStub.RequestRemoveTree = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int treeID) =>
                {
                    Monitor.Enter(this);
                    
                    // find the ville
                    Ville_S ville;
                    if (m_remoteClients.TryGetValue(remote, out ville))
                    {
                        // find the tree
                        WorldObject_S tree;
                        if (ville.m_worldOjbects.TryGetValue(treeID, out tree))
                        {
                            WorldObject_S obj;
                            ville.m_worldOjbects.TryRemove(treeID, out obj);

                            HostID[] list = m_netServer.GetClientHostIDs();
                            foreach (HostID id in list)
                            {
                                m_S2CProxy.NotifyRemoveTree(id, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, tree.m_id);
                            }
                        }
                    }
                    Monitor.Exit(this);
                    return true;
                };
        }

        public void Start()
        {
            //fill server startup parameters
            StartServerParameter sp = new StartServerParameter();
            sp.protocolVersion = new Nettention.Proud.Guid(SngCommon.Vars.g_sngProtocolVersion);
            sp.tcpPorts = new IntArray();
            sp.tcpPorts.Add(SngCommon.Vars.g_serverPort);   // must be same to the port number at client
            sp.serverAddrAtClient = "192.168.219.103";
            sp.localNicAddr = "192.168.219.103";
            sp.SetExternalNetWorkerThreadPool(netWorkerThreadPool);
            sp.SetExternalUserWorkerThreadPool(userWorkerThreadPool);

            // let's start!
            m_netServer.Start(sp);
        }

        public void MoveRemoteClientToLoadedVille(HostID remote, Ville_S ville, String nickName)
        {
            RemoteClient_S remoteClientValue;
            Ville_S villeValue;

            if(!ville.m_players.TryGetValue(remote, out remoteClientValue) && !m_remoteClients.TryGetValue(remote, out villeValue)) 
            {
                ville.m_players.TryAdd(remote, new RemoteClient_S(nickName, ville.nextLocation));
                ville.UpdateNextLoc(ville.nextLocation);
                m_remoteClients.TryAdd(remote, ville);
            }

            // now, the player can do P2P communication with other player in the smae ville.
            m_netServer.JoinP2PGroup(remote, ville.m_p2pGroupID);

            // check nickname
            ville.m_players.TryGetValue(remote, out remoteClientValue);
            Console.WriteLine("Client NickName : {0}", remoteClientValue.nickName);

            //notify current world state to new user
            foreach (KeyValuePair<int, WorldObject_S> iWO in ville.m_worldOjbects)
            {
                m_S2CProxy.NotifyAddTree(remote, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, iWO.Value.m_id, iWO.Value.m_position);
            }
            Console.WriteLine("masterID: {0}", ville.masterID);
            Console.WriteLine("<Update Client List>");
            // notify current players list to new user

            foreach (KeyValuePair<HostID, RemoteClient_S> iPlayer in ville.m_players)
            {
                Console.WriteLine("{0}", iPlayer.Value.nickName);
                m_S2CProxy.NotifyPlayerJoin(remote, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, iPlayer.Value.nickName, iPlayer.Value.waitingIdx);
            }

            // except remote, notfy a new Player enters
            foreach (KeyValuePair<HostID, RemoteClient_S> iPlayer in ville.m_players)
            {
                if (iPlayer.Key == remote) continue;
                m_S2CProxy.NotifyPlayerJoin(iPlayer.Key, RmiContext.ReliableSend, (int)ville.m_p2pGroupID, remoteClientValue.nickName, remoteClientValue.waitingIdx);
            }

        }

        private void UnloadVille(Ville_S ville)
        {
            // ban the players in the ville
            foreach (KeyValuePair<HostID, RemoteClient_S> iPlayer in ville.m_players)
            {
                m_netServer.CloseConnection(iPlayer.Key);
            }

            Ville_S villeValue;

            //shutdown the loaded ville
            m_villes.TryRemove(ville.m_name, out villeValue);

            //reales the cache data tree
            m_netServer.DestroyP2PGroup(ville.m_p2pGroupID);

        }

        public void Dispose()
        {
            //NetServer의 경우 프로그램 종료 또는 NetServer 객체 파괴시 명시적으로 NetServer.Dispose()를 호출해주어야함
            m_netServer.Dispose();
        }
    }
}
