using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nettention.Proud;

namespace SngServer
{
    public class Ville_S
    {
        public Ville_S()
        {
            m_nextNewID = 1;
            m_p2pGroupID = HostID.HostID_None;
            masterID = HostID.HostID_None;
            nextLocation = 0;
            canJoin = true;
            for (int i = 0; i < waitingArr.Length; i++)
            {
                waitingArr[i] = false;
            }
        }

        // the players who are online.
        public ConcurrentDictionary<HostID, RemoteClient_S> m_players = new ConcurrentDictionary<HostID, RemoteClient_S>();
        // where new player can locate in waiting room
        public int nextLocation;

        // is available to join this room
        public bool canJoin;

        // who is room master
        public HostID masterID;

        // ville name
        public String m_name;

        // increases for every new world object is added.
        // this value is saved to database, too.
        public int m_nextNewID;

        // world objects
        public ConcurrentDictionary<int, WorldObject_S> m_worldOjbects = new ConcurrentDictionary<int, WorldObject_S>();

        // every players in this ville are P2P communicated.
        //this is useful for lesser latency for cloud-hosting servers
        public HostID m_p2pGroupID;

        public bool[] waitingArr = new bool[SngCommon.Vars.maxPlayerNum];

        public void FindNextLoc()
        {
            int oldLocation = nextLocation;
            for (int i = nextLocation + 1; i < SngCommon.Vars.maxPlayerNum; i++)
            {
                if (waitingArr[i] == false)
                {
                    nextLocation = i;
                    break;
                }
            }

            // can't find empty loc
            if (oldLocation == nextLocation)
            {
                nextLocation = SngCommon.Vars.maxPlayerNum;
                canJoin = false;
            }
        }

        public void UpdateNextLoc(int idx)
        {
            waitingArr[idx] = !waitingArr[idx];
            if (waitingArr[idx] == false && idx <= nextLocation)
            {
                if(nextLocation == SngCommon.Vars.maxPlayerNum)
                    canJoin = true;
                nextLocation = idx;
            }
            if (waitingArr[idx] == true)
            {
                FindNextLoc();
            }
        }
    }
}
