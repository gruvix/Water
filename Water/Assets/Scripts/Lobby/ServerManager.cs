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
    public Button kickPrefab;
    public Transform bote;

    void Start()
    {
        bnStart.onClick.AddListener(ChangePlayerPrefab);
        bnStart.onClick.AddListener(Startgame);
    }

    private void Startgame()
    {
        for (int i = 0; i < bote.childCount; i++)
		{
            if (bote.GetChild(i).GetComponent<Buyables_Floater_Data>())
			{
                RpcRemoveDataComp(bote.GetChild(i).gameObject);
			}

		}
        NetworkManager.singleton.ServerChangeScene(gameScene);
    }

    [ClientRpc]
    private void RpcRemoveDataComp(GameObject floater)
	{
        Destroy(floater.GetComponent<Buyables_Floater_Data>());
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
                lobby.RpcReadyUpdate(Player, ready);
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
            lobby.RpcMoveLobbyPlayer(Player.GetComponent<NetworkIdentity>().connectionToClient, yOffset);
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
        go.onClick.AddListener(() => lobby.RpcKick(conn));
    }
}
