using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Telepathy;


public class NetWorkMenu : NetworkBehaviour
{
    public TMPro.TMP_InputField ipField;
    public Button bnHost;
    public Button bnConnect;
    public Button bnCancel;
    public GameObject MenuMultiplayer;
    public GameObject MenuConnecting;
    public GameObject MenuMain;
    private bool connecting = false;

    private void Start()
    {
        if (NetworkManager.singleton.isStarted)
        {
            MenuMain.SetActive(false);
            MenuMultiplayer.SetActive(true);
        }
        else
        {
            NetworkManager.singleton.isStarted = true;
        }
        bnHost.onClick.AddListener(HostClick);
        bnConnect.onClick.AddListener(ConnectClick);
        bnCancel.onClick.AddListener(CancelClick);
    }

    private void HostClick()
	{
        NetworkManager.singleton.StartHost();
    }

    private void ConnectClick()
    {
        NetworkManager.singleton.StartClient();
        connecting = true;
    }

    private void CancelClick()
    {
        NetworkManager.singleton.StopClient();
        connecting = false;
    }

	private void Update()
	{
		if (connecting && !NetworkClient.active)
		{
			MenuMultiplayer.SetActive(true);
			MenuConnecting.SetActive(false);
            connecting = false;
        }

	}

	public void UpdateText()
    {
        NetworkManager.singleton.networkAddress = ipField.text;
    }
}
