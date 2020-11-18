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
    public Animator ErrorText;
    public TMPro.TextMeshProUGUI RetryStatus;
    public GameObject playermenuPrefab;
    private bool connecting = false;
    private int retryMax = 4;
    private int retry;

    private void Start()
    {
        retry = retryMax;
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
        NetworkManager.singleton.playerPrefab = playermenuPrefab;
    }

    private void HostClick()
	{
        NetworkManager.singleton.StartHost();
    }

    private void ConnectClick()
    {
        NetworkManager.singleton.StartClient();
        connecting = true;
        ErrorText.SetBool("Show", false);
    }

    private void CancelClick()
    {
        connecting = false;
        NetworkManager.singleton.StopClient();
        RetryStatus.text = ("Connecting");
        retry = retryMax;
    }

	private void Update()//Failed Connection Check, will try again for _retryMax_ times
	{
		if (connecting && !NetworkClient.active)
		{
            if(retry == 0)
            { 
			    MenuMultiplayer.SetActive(true);
			    MenuConnecting.SetActive(false);
                connecting = false;
                RetryStatus.text = ("Connecting");
                ErrorText.SetBool("Show", true);
                retry = retryMax;
            }
			else
			{
                NetworkManager.singleton.StartClient();
                RetryStatus.text = ($"Retrying ({retryMax + 1 - retry})");
                retry--;
			}
        }

	}

	public void UpdateText()
    {
        NetworkManager.singleton.networkAddress = ipField.text;
    }
}
