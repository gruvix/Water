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

    void Start()
    {
        bnStart.onClick.AddListener(Startgame);
    }

    void Startgame()
	{
        ChangePlayerPrefab();
        NetworkManager.singleton.ServerChangeScene(gameScene);
    }

    [ClientRpc]
    public void ChangePlayerPrefab()
    {
        NetworkManager.singleton.playerPrefab = playergamePrefab;
    }
}
