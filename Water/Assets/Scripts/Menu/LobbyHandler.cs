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
	[SyncVar]
	public int readyUsers = 0;
	public GameObject playergamePrefab;

	[Scene]
	public string gameScene;

	[Client]
	private void Start()
	{
		if (isClientOnly)//IS CLIENT
		{
			bnReady.gameObject.SetActive(true);
			bnReady.onClick.AddListener(ReadyClick);
		}
		else//IS HOST
		{
			Debug.Log("IS HOST");
			NetworkManager.singleton.playerPrefab.transform.GetChild(0).gameObject.SetActive(true);
			NetworkManager.singleton.playerPrefab.transform.GetChild(1).gameObject.SetActive(false);
			bnStart.gameObject.SetActive(true);
		}
		bnDisconnect.onClick.AddListener(DisconnectClick);
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
