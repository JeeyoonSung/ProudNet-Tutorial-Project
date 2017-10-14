using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettention.Proud;
using UnityEngine.SceneManagement;

partial class GameClient : MonoBehaviour
{
    public void TryStartGame()
    {
        Debug.Log("try");
        if(isMaster)
        {
            m_C2SProxy.RequestEnterGame(HostID.HostID_Server, RmiContext.ReliableSend);
        } else
        {
            Debug.Log("권한이 없습니다.");
        }
    }
    
}
public class ScenesManager : MonoBehaviour {
    
    public static void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
