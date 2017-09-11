using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour {

    public static Dictionary<int, WorldObject> m_worldObjects = new Dictionary<int, WorldObject>();
    public int m_id
    {
        set
        {
            // reassigns to the dictionary
            if (m_id_INTERNAL == value)
                return;
            if (m_id_INTERNAL != 0)
                m_worldObjects.Remove(m_id);
            if (value != 0)
                m_worldObjects.Add(value, this);
            
            // and this tool
            m_id_INTERNAL = value;
        }
        get
        {
            return m_id_INTERNAL;
        }
    }

    int m_id_INTERNAL = 0;

    private void OnDestroy()
    {
        m_id = 0;
    }




}
