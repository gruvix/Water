using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;

public class ServerManager : NetworkBehaviour
{
    public Button bnStart;
    public GameObject playergamePrefab;
    [Scene]
    public string gameScene;
    public LobbyHandler lobby;
    public Button kickPrefab;

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

    [Server]
    public void AddKickBn(GameObject player, NetworkConnection conn)
    {
        var go = Instantiate(kickPrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(player.transform);
        go.transform.position = new Vector3(0.2f, 1.02f, 0f);
        go.onClick.AddListener(() => lobby.Kick(conn));
    }
}
