using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

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
	public string htmlColor = null;
	public Chat chat;
	private bool started = false;
	private int delay = 0;
	[SyncVar]
	public float boatPoints = 20;
	public TextMeshProUGUI boatValueText;
	private bool timeout = false;

	[Client]
	private void Start()
	{
		if (isClientOnly)//IS CLIENT
		{
			bnReady.gameObject.SetActive(true);
			boatValueText.text = $"Boat Points:{boatPoints}/100";
		}
		else//IS HOST
		{
			bnStart.gameObject.SetActive(true);
		}
		bnDisconnect.onClick.AddListener(DisconnectClick);
	}

	private void PlayerStart()
	{
		StartCoroutine(HammerTime());
		while (!ClientScene.localPlayer)
		{
			if (timeout)
			{
				string msg = ClientScene.localPlayer.GetComponent<PlayerData>().userName + "<#D7CF93> failed to connect</color>\n";
				chat.CmdSendMessage(msg);
				NetworkManager.singleton.StopClient();
				return;
			}
		}

		var rgb = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		htmlColor = ColorUtility.ToHtmlStringRGB(rgb);
		NetworkManager.singleton.htmlColor = htmlColor;

		if (isServer)
		{
			ClientScene.localPlayer.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			ClientScene.localPlayer.gameObject.transform.GetChild(1).gameObject.SetActive(true);
			readyUsers = 1;
			IsReady = true;
			if (isServer) { hostName = NetworkManager.singleton.userName; }
		}
		else
		{
			bnReady.gameObject.SetActive(true);
			bnStart.gameObject.SetActive(false);
			chat.ChatWelcome();
		}
		CmdUpdateName(ClientScene.localPlayer.gameObject, $"<#{htmlColor}>{NetworkManager.singleton.userName}</color>");
	}

	IEnumerator HammerTime()
	{
		yield return new WaitForSeconds(1);
		timeout = true;
		Debug.Log("player start timeout");
	}

	[ClientRpc]
	public void RpcUpdateBoatPoints()
	{
		boatValueText.text = $"Boat Points:{boatPoints}";
	}

	[ClientRpc]
	public void NameFieldRpc()
	{
		string name = $"<#{htmlColor}>{NetworkManager.singleton.userName}</color>";
		CmdUpdateName(ClientScene.localPlayer.gameObject, name);
	}


	[Command(ignoreAuthority = true)]
	public void CmdUpdateName(GameObject player, string name)
	{
		RpcUpdateForAll(player, name);
	}


	[ClientRpc]
	private void RpcUpdateForAll(GameObject player, string name)
	{
		player.transform.GetComponent<TMPro.TextMeshProUGUI>().text = name;
		player.transform.SetParent(transform);
		player.GetComponent<PlayerData>().userName = name;
	}


	[TargetRpc]
	public void RpcMoveLobbyPlayer(NetworkConnection target, float yOffset)
	{
		ClientScene.localPlayer.transform.position = new Vector3(ClientScene.localPlayer.transform.position.x, yOffset, ClientScene.localPlayer.transform.position.z);
	}


	[TargetRpc]
	public void RpcKick(NetworkConnection conn)
	{
		string msg = ClientScene.localPlayer.GetComponent<PlayerData>().userName + "<#D7CF93> was kicked from the lobby</color>\n";
		chat.CmdSendMessage(msg);
		NetworkManager.singleton.StopClient();
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
		RpcReadyUpdate(player, readyState);
    }

	[ClientRpc]
	public void RpcReadyUpdate(GameObject player, bool readyState)
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
