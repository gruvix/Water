using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class NetWorkManagerWater : NetworkManager
{
	public ServerManager serverMan;

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
		if(SceneManager.GetActiveScene().name == "Lobby")
		{
			serverMan = FindObjectOfType<ServerManager>();
		}
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		if (SceneManager.GetActiveScene().name == "Lobby" && conn != NetworkServer.localConnection)
		{
			string Msg = null;
			foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (player.GetComponent<NetworkIdentity>().connectionToClient == conn)
				{
					Msg = $"<#D7CF93>{player.GetComponent<PlayerData>().userName} has disconnected</color>\n";
				}
			}
			serverMan.lobby.chat.CmdSendMessage(Msg);
		}
		base.OnServerDisconnect(conn);
	}


	[Server]
	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		base.OnServerAddPlayer(conn);
		if (SceneManager.GetActiveScene().name != "Lobby" || conn == NetworkServer.localConnection) { return; }
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (player.GetComponent<NetworkIdentity>().connectionToClient == conn)
			{
				serverMan.AddKickBn(player, conn);
			}
		}
	}

}
