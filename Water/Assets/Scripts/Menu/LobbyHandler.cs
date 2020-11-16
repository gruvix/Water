using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyHandler : NetworkBehaviour
{
    public bool IsReady = false;
	public Button bnReady;
	public Button bnStart;
    public Button bnDisconnect;
	public int totalUsers = 0;
	public int readyUsers = 0;

	private void Start()
	{
		if (isClientOnly)//IS CLIENT
		{
			bnReady.gameObject.SetActive(true);
			bnReady.onClick.AddListener(ReadyClick);
		}
		else//IS HOST
		{
			NetworkManager.singleton.playerPrefab.transform.GetChild(0).gameObject.SetActive(false);
			NetworkManager.singleton.playerPrefab.transform.GetChild(1).gameObject.SetActive(true);
			bnStart.gameObject.SetActive(true);
			bnStart.onClick.AddListener(StartClick);
		}
		bnDisconnect.onClick.AddListener(DisconnectClick);

		totalUsers += 1;
	}

	private void StartClick()
	{
		Debug.Log("START GAME");
	}


	private void DisconnectClick()
	{
        if(isClientOnly)
		{
            NetworkManager.singleton.StopClient();
        }
        else
		{
            NetworkManager.singleton.StopHost();
		}
	}


	private void ReadyClick()
	{
		if(!IsReady)
		{
			bnReady.transform.GetChild(0).gameObject.SetActive(true);
			bnReady.transform.GetChild(1).gameObject.SetActive(false);
			readyUsers += 1;
		}
		else
		{
			bnReady.transform.GetChild(0).gameObject.SetActive(false);
			bnReady.transform.GetChild(1).gameObject.SetActive(true);
			readyUsers -= 1;
		}
		ReadyChange();
	}

	public void ReadyChange()
    {
        IsReady = !IsReady;
    }
}
