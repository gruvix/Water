using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ServerManager : NetworkBehaviour
{
    public Button bnStart;
    public GameObject playergamePrefab;
    [Scene]
    public string gameScene;
    public LobbyHandler lobby;
    NetworkManager netManager;

    void Start()
    {
        bnStart.onClick.AddListener(ChangePlayerPrefab);
        bnStart.onClick.AddListener(Startgame);
    }

    private void Startgame()
    {
        NetworkManager.singleton.ServerChangeScene(gameScene);
        netManager = NetworkManager.singleton;
    }

	private void OnPlayerConnected(NetworkConnection player)
	{
        Debug.Log("Player Connected????");
        UpdateCount();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
        Debug.Log("Player has connected wiiiii");
        int count = NetworkManager.singleton.numPlayers;
		/*foreach (NetworkConnection con in NetworkServer.connections)
		{
            if(con!=null)
			{
                count++;
			}
		}*/
        UpdateCount();
        lobby.totalUsers = count;
    }

	[ClientRpc]
    private void UpdateCount()
	{
        Debug.Log("A player has connected UGAGAGA");
	}

	[ClientRpc]
    public void ChangePlayerPrefab()
    {
        NetworkManager.singleton.playerPrefab = playergamePrefab;
    }
}
