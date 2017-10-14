using System.Collections;
using System.Collections.Generic;
using UnityEngine;


partial class GameClient : MonoBehaviour
{
}

public class CameraManager : MonoBehaviour {
    public Camera playerCam;
    public Camera loadingUICam;

	// Use this for initialization
	void Start () {
        LoadingUICamOn();
    }

    void PlayerCamOn()
    {
        playerCam.enabled = true;
        loadingUICam.enabled = false;
    }

    void LoadingUICamOn()
    {
        playerCam.enabled = false;
        loadingUICam.enabled = true;
    }
}
