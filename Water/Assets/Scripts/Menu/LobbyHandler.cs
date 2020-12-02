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
	public int readyUsers;
	[SyncVar]
	public int totalUsers;
	[Scene]
	public string gameScene;
	public TMPro.TMP_InputField nameField;

	[Client]
	private void Start()
	{
		if (isClientOnly)//IS CLIENT
		{
			bnReady.gameObject.SetActive(true);
		}
		else//IS HOST
		{
			totalUsers = 1;
			readyUsers = 1;
			bnStart.gameObject.SetActive(true);
			
		}
		bnDisconnect.onClick.AddListener(DisconnectClick);
		nameField.text = "Error_No_Name";
		NameField();
	}

	public void NameField()
	{
		NetworkManager.singleton.userName = nameField.text;
		CmdUpdateName(ClientScene.localPlayer.gameObject, nameField.text);
	}

	[ClientRpc]
	public void NameFieldRpc()
	{
		CmdUpdateName(ClientScene.localPlayer.gameObject, nameField.text);
	}


	[Command(ignoreAuthority = true)]
	public void CmdUpdateName(GameObject player, string name)
	{
		UpdateForAll(player, name);
	}


	[ClientRpc]
	private void UpdateForAll(GameObject player, string name)
	{
		player.transform.GetComponent<TMPro.TextMeshProUGUI>().text = name;
	}



	[TargetRpc]
	public void MoveLobbyPlayer(NetworkConnection target, float yOffset)
	{
		ClientScene.localPlayer.transform.position = new Vector3(ClientScene.localPlayer.transform.position.x, yOffset, ClientScene.localPlayer.transform.position.z);
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!isClientOnly)
		{
			ClientScene.localPlayer.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			ClientScene.localPlayer.gameObject.transform.GetChild(1).gameObject.SetActive(true);
		}
		else
		{
			bnReady.gameObject.SetActive(true);
			bnStart.gameObject.SetActive(false);
			AddKickBn(ClientScene.localPlayer.gameObject);
		}
	}

	[Command(ignoreAuthority = true)]//ACA SE AGREGA EL BOTON DE KICK
	private void AddKickBn(GameObject newPlayer)
	{
		//Instantiate
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


	public void ReadyClick()
	{
		bnReady.transform.GetChild(0).gameObject.SetActive(!IsReady);
		bnReady.transform.GetChild(1).gameObject.SetActive(IsReady);
		IsReady = !IsReady;
		CmdReadyChange(ClientScene.localPlayer.gameObject, IsReady);
	}

	[Command(ignoreAuthority = true)]
	public void CmdReadyChange(GameObject player, bool readyState)
    {
        if(readyState)
		{
			readyUsers += 1;
		}
		else
		{
			readyUsers -= 1;
		}
		ReadyUpdate(player, readyState);
    }

	[ClientRpc]
	public void ReadyUpdate(GameObject player, bool readyState)
	{
		player.transform.GetChild(0).gameObject.SetActive(!readyState);
		player.transform.GetChild(1).gameObject.SetActive(readyState);
	}
}
