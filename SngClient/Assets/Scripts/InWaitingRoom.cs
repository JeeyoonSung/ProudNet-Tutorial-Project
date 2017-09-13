using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;
using UnityEngine.UI;

partial class GameClient : MonoBehaviour
{

    public GameObject waitingPlayerMarks, waitingPlayerPrefab;
    public GameObject nickNameField, nickNameTextPrefab;
    public GameObject waitingRoom;

    public Dictionary<int, GameObject> playerMarks = new Dictionary<int, GameObject>();
    public Dictionary<GameObject, bool> isVacant = new Dictionary<GameObject, bool>();
    public Dictionary<int, GameObject> playerNicks = new Dictionary<int, GameObject>();

    public bool isMaster;

    private void SetWaitingRoom()
    {
        int playerNum = 8;
        int offset_x = 20;
        int offset_y = 20;
        float pos_x, pos_y;
        float pos_z = 50.0f;
        for (int i = 0; i < playerNum; i++)
        {
            pos_x = -(playerNum / 2 - 1) * offset_x / 2 + offset_x * (i % (playerNum / 2));
            if (i / (playerNum / 2) == 0) pos_y = offset_y / 2;
            else pos_y = -offset_y / 2;
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(pos_x, pos_y, pos_z);

            GameObject waitingMark = Instantiate(waitingPlayerPrefab, pos, Quaternion.Euler(0, 180f, 0));
            waitingMark.transform.SetParent(waitingPlayerMarks.transform);
            playerMarks.Add(i, waitingMark);
            isVacant.Add(waitingMark, true);

            GameObject nickText = Instantiate(nickNameTextPrefab, UnityEngine.Vector3.zero, Quaternion.identity);
            nickText.transform.SetParent(nickNameField.transform);
            nickText.transform.position = pos - UnityEngine.Vector3.up * 2;
            playerNicks.Add(i, nickText);

            nickText.SetActive(false);
            waitingRoom.SetActive(false);
        }
    }


    void Start_InWaitingRmiStub()
    {
        m_S2CStub.NotifyPlayerJoin = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, string nick, int idx) =>
        {
            Debug.Log("Call NotifyPlayerJoin " + idx);

            GameObject nickText, markObj;
            if ((int)m_myP2PGroupID == groupID)
            {
                playerNicks.TryGetValue(idx, out nickText);
                nickText.GetComponent<Text>().text = nick;
                nickText.SetActive(true);

                playerMarks.TryGetValue(idx, out markObj);
                markObj.transform.rotation = Quaternion.Euler(UnityEngine.Vector3.zero);

                isVacant[markObj] = false;
            }
            return true;
        };

        m_S2CStub.NotifyPlayerLeave = (Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, int groupID, int idx, bool changeMaster, int newMasterID) =>
        {
            Debug.Log("Call NotifyPlayerLeave " + idx);

            GameObject nickText, markObj;
            if ((int)m_myP2PGroupID == groupID)
            {
                playerNicks.TryGetValue(idx, out nickText);
                nickText.SetActive(false);

                playerMarks.TryGetValue(idx, out markObj);
                markObj.transform.rotation = Quaternion.Euler(UnityEngine.Vector3.up*180);

                isVacant[markObj] = true;

                // appoint new master
                if(changeMaster && (int)remote == newMasterID)
                {
                    isMaster = true;
                }
            }
            return true;
        };
    }

}