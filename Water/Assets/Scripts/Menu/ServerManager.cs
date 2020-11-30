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

    void Start()
    {
        bnStart.onClick.AddListener(ChangePlayerPrefab);
        bnStart.onClick.AddListener(Startgame);
    }

	private void Startgame()
	{
        NetworkManager.singleton.ServerChangeScene(gameScene);
    }


	private void OnPlayerConnected()
	{
        int count = NetworkManager.singleton.numPlayers;
		/*foreach (NetworkConnection con in NetworkServer.connections)
		{
            if(con!=null)
			{
                count++;
			}
		}*/
        UpdateCount(count);
    }

    private void UpdateCount(int TU)
	{
        lobby.totalUsers = TU;
        Debug.Log("A player has connected");
	}

	[ClientRpc]
    public void ChangePlayerPrefab()
    {
        NetworkManager.singleton.playerPrefab = playergamePrefab;
    }
}
