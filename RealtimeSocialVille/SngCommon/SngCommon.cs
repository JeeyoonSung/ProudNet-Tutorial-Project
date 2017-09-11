using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SngCommon
{
    public class Vars
    {
        // SngClient/Assets/Scripts 안의 값과 동일해야함
        public static System.Guid g_sngProtocolVersion = new System.Guid("{0x612df376,0x37a9,0x49d5,{0x8e,0xc0,0x8f,0x41,0xb8,0x4f,0x11,0x56}}");
        public static int g_serverPort = 15001;

        static Vars()
        {

        }
    }
}
