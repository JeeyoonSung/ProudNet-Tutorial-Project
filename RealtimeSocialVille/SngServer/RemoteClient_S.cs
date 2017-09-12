using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SngServer
{
    public class RemoteClient_S
    {
        public string nickName;
        public int waitingIdx;
        public RemoteClient_S(string nick, int idx)
        {
            nickName = nick;
            waitingIdx = idx;
        }


    }
}
