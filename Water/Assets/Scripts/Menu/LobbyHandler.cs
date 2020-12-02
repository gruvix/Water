using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.RemoteCalls;
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
	[SyncVar]
	public string hostName;
	[Scene]
	public string gameScene;
	public TMPro.TMP_InputField nameField;
	public string htmlColor = null;
	public Chat chat;
	private bool started = false;
	private int delay = 0;

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
		nameField.text = "No_Name";//ACA IRIA EL NOMBRE DESDE ANTES
	}

	private void PlayerStart()
	{
		if (!isClientOnly)
		{
			ClientScene.localPlayer.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			ClientScene.localPlayer.gameObject.transform.GetChild(1).gameObject.SetActive(true);
		}
		else
		{
			bnReady.gameObject.SetActive(true);
			bnStart.gameObject.SetActive(false);
		}
		var rgb = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		htmlColor = ColorUtility.ToHtmlStringRGB(rgb);
		NameField();
		chat.ChatWelcome();
	}

	public void NameField()
	{
		NetworkManager.singleton.userName = nameField.text;
		CmdUpdateName(ClientScene.localPlayer.gameObject, $"<#{htmlColor}>{nameField.text}</color>");
		if (!isClientOnly) { hostName = nameField.text; }
	}

	[ClientRpc]
	public void NameFieldRpc()
	{
		string name = $"<#{htmlColor}>{nameField.text}</color>";
		CmdUpdateName(ClientScene.localPlayer.gameObject, name);
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

	private void Update()
	{
		if(!started)
		{
			if(delay < 5)
			{
				
				delay++;
			}
			else
			{
				PlayerStart();
				started = true;
			}
		}
	}
}
