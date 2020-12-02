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

	private void Update()
	{
        if (lobby.totalUsers != NetworkManager.singleton.numPlayers) 
        {
            lobby.readyUsers = 0;
            if (lobby.totalUsers < NetworkManager.singleton.numPlayers)
            {
                lobby.NameFieldRpc();
            }
            foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
            {
                bool ready = false;
                if (Player.transform.GetChild(1).gameObject.activeSelf)
                {
                    lobby.readyUsers++;
                    ready = true;
                }
                lobby.ReadyUpdate(Player, ready);
            }
            UpdateLobby();
        }

        if (lobby.readyUsers != lobby.totalUsers)
        {
            bnStart.gameObject.SetActive(false);
        }
        else
		{
            bnStart.gameObject.SetActive(true);
		}
    }


    private void UpdateLobby()
    {
        float yOffset = 0.6f;
        foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            lobby.MoveLobbyPlayer(Player.GetComponent<NetworkIdentity>().connectionToClient, yOffset);
            yOffset -= 0.3f;
        }
        lobby.totalUsers = NetworkManager.singleton.numPlayers;
        Debug.Log($"Users Online: {lobby.totalUsers}");
	}

	[ClientRpc]
    public void ChangePlayerPrefab()
    {
        NetworkManager.singleton.playerPrefab = playergamePrefab;
    }
}
