﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SngServer
{
    public class WorldObject_S
    {
        public int m_id;
        public UnityEngine.Vector3 m_position;

        public WorldObject_S()
        {
            m_id = 0;

            m_position = UnityEngine.Vector3.zero;
        }
    }
}
