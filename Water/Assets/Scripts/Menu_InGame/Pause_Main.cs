using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Pause_Main : MonoBehaviour
{

    public static Pause_Main singleton = null;
    public bool paused = false;

    public Button bnDisconnect;

    void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        bnDisconnect.onClick.AddListener(Disconnect);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Pause();
        }
    }

    void Pause()
	{
        paused = !paused;
        transform.GetChild(0).gameObject.SetActive(paused);
    }

    private void Disconnect()
	{
        if (NetworkManager.singleton.mode == NetworkManagerMode.Host) { NetworkManager.singleton.StopHost(); }
        else { NetworkManager.singleton.StopClient(); }
	}
}
