using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;

partial class GameClient : MonoBehaviour
{
    public GameObject m_treePrefab;

    HostID m_myP2PGroupID = HostID.HostID_None; // will be filled after joining ville is finished.

    // scribble point. this is used for P2P communication for instant real-time scribble.
    public GameObject m_scribblePrefab;

    enum FingerMode { Tree, Scribble };
    FingerMode m_fingerMode = FingerMode.Tree;

    private void Update_InVille()
    {
        // determine if clicked (release button)
        bool pushing = Input.GetMouseButton(0);
        bool clicked = Input.GetMouseButtonUp(0);

        // pick object
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject pickedObject = null;
        if(Physics.Raycast(ray, out hit))
        {
            pickedObject = hit.transform.root.gameObject;
        }

        if(m_fingerMode == FingerMode.Tree)
        {

            if (clicked)
            {
                if (pickedObject != null)
                {
                    if (pickedObject.name == "Terrain")
                    {
                        // request to plant a tree
                        m_C2SProxy.RequestAddTree(HostID.HostID_Server, RmiContext.ReliableSend, hit.point);
                    }
                    else if (pickedObject.name.Contains("Tree"))
                    {
                        // request to delete the tree
                        WorldObject wo = (WorldObject)pickedObject.GetComponent(typeof(WorldObject));
                        if (wo != null)
                        {
                            int treeID = wo.m_id;
                            m_C2SProxy.RequestRemoveTree(HostID.HostID_Server, RmiContext.ReliableSend, treeID);
                        }

                    }
                }
            }
        }
        else if (m_fingerMode == FingerMode.Scribble)
        {
            if(pushing)
            {
                //P2P send
                m_C2CProxy.ScribblePoint(m_myP2PGroupID, RmiContext.UnreliableSend, (int)m_myP2PGroupID, hit.point);
                // ...and for me!
                Instantiate(m_scribblePrefab, hit.point, Quaternion.identity);
            }
        }
    }

    private void OnGUI_InVille()
    {
        GUI.Label(new Rect(10, 10, 500, 70), "In Ville. You can plant or remove trees by touching terrain. You can also ~");
        if (GUI.Button(new Rect(10, 90, 130, 30), "Tree"))
        {
            m_fingerMode = FingerMode.Tree;
        }
        if(GUI.Button(new Rect(300,90,130,30), "Scribble"))
        {
            m_fingerMode = FingerMode.Scribble;
        }
    }

    
    void Start_InVilleRmiStub()
    {

        m_S2CStub.NotifyAddTree = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, int treeID, UnityEngine.Vector3 position) =>
        {
            if((int)m_myP2PGroupID == groupID)
            {
                // plant a tree
                // 들어갔을 때 이미 심어져 있는 나무들을 심고 시작
                GameObject o = (GameObject)Instantiate(m_treePrefab, position, Quaternion.identity);
                WorldObject t = (WorldObject)o.GetComponent(typeof(WorldObject));
                t.m_id = treeID;
            }
            return true;
        };

        m_S2CStub.NotifyRemoveTree = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, int treeID) =>
        {
            if((int)m_myP2PGroupID == groupID)
            {
                //destroy picked tree
                WorldObject wo;
                if(WorldObject.m_worldObjects.TryGetValue(treeID, out wo))
                {
                    Destroy(wo.gameObject);
                }
            }
            return true;
        };

        m_C2CStub.ScribblePoint = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, UnityEngine.Vector3 point) =>
        {
            if((int)m_myP2PGroupID == groupID)
            {
                Instantiate(m_scribblePrefab, point, Quaternion.identity);
            }
            return true;
        };
    }
}


