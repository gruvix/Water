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

    private void OnServerConnect()
	{
        lobby.totalUsers++;
        Debug.LogError("A PLAYER CONNECTED MESSAGE FOR SERVERONLY");
        test();
	}

    [ClientRpc]
    private void test()
	{
        Debug.Log("A player has connected");
	}

	[ClientRpc]
    public void ChangePlayerPrefab()
    {
        NetworkManager.singleton.playerPrefab = playergamePrefab;
    }
}
