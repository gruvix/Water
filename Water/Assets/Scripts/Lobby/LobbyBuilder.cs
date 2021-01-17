using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyBuilder : NetworkBehaviour
{
    public Transform bote;
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    private bool ghostCheck = true;
    private bool has_floater = false;
    [SerializeField][SyncVar]
    private GameObject floater;
    [SerializeField]
    private string floaterName;
    private float rotationCounter = 0;
    private float rotationAmount = 30f;
    private float alcance = 2f;
    private GameObject lobbyHandler;
    private float cost = 0;

    void Start()
    {
        bote = GameObject.Find("Bote").transform;
        lobbyHandler = GameObject.Find("LobbyCanvas");
    }



    void Update()
    {
        if (!hasAuthority) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(m_puntero, Vector2.zero);
            if (has_floater)
            {
                // Si tiene algo agarrado y haces click lo deja donde hiciste click y se lo da al bote
                ghostCheck = Ghost.GetComponent<LobbyGhost>().CanPlace;
                if (ghostCheck)
                {
                    CmdMoveFloater(Ghost.transform.position, Ghost.transform.rotation, floater);
                    Ghost.GetComponent<LobbyGhost>().DestroyCollider();
                    has_floater = false;
                    floater = null;
                    Ghost.SetActive(false);
                }
            }

            // Si el jugador no tiene nada agarrado agarra lo que haya cliqueado si es que clickeo algo y no tiene dueño
            else
            {
                if (hit.collider != null && Vector2.Distance(m_puntero, gameObject.transform.position) <= alcance)
                {

                    if ((hit.collider.tag == "Floater" || hit.collider.tag == "FloaterPlatform") && hit.collider.transform.parent.tag != "Player")//Cuando el objeto es un floater
                    {
                        floaterName = hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().nameString;
                        cost = hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().cost;

                        if (hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().isFloater)
						{
                            //CmdMoveFloater(ClientScene.localPlayer.gameObject, floater);
                            floater = hit.collider.gameObject;
                            Debug.Log("hiciste click en el floater: "+ floater);
                            has_floater = true;
                            Ghost.SetActive(true);
                            Ghost.GetComponent<LobbyGhost>().SetCollider(floater);
                            Ghost.transform.rotation = Quaternion.identity;
                            Ghost.transform.position = new Vector3(m_puntero[0], m_puntero[1], Ghost.transform.position.z);
                        }

                        else 
                        { 
                            if(lobbyHandler.GetComponent<LobbyHandler>().boatPoints < cost)
							{
								Debug.Log("Insuficiente Dinero");
							}
                            else
							{
                                //floater = hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().prefab;
                                CmdBuyFloater(floaterName, cost, gameObject);
                                has_floater = true;
                                Ghost.transform.rotation = Quaternion.identity;
                                Ghost.SetActive(true);
                                Ghost.GetComponent<LobbyGhost>().SetCollider(floater);
                                Ghost.transform.position = new Vector3(m_puntero[0], m_puntero[1], Ghost.transform.position.z);
                            }
                        }
                    }
                }
            }

        }
        if (has_floater)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z + rotationAmount));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z - rotationAmount));
            }

            //Rotacion del Ghost
            if (Input.GetKey(KeyCode.Q))
            {
                rotationCounter -= 0.015f;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotationCounter += 0.015f;
            }
            else
            {
                rotationCounter = 0;
            }

            rotationCounter = Mathf.Clamp(rotationCounter, -1, 1);
            if (Mathf.Abs(rotationCounter) == 1)
            {
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z - rotationCounter * rotationAmount));
                rotationCounter = 0;
            }
        }


        //Soltar Objeto (Huerfano)
        if (Input.GetMouseButtonDown(1) && has_floater)
        {
            Ghost.GetComponent<LobbyGhost>().DestroyCollider();
            Ghost.SetActive(false);
            has_floater = false;
			if (floater) { CmdSellFloater(floater); }
        }
    }

    //Crea un objeto nuevo cuando clicleas en la imagen
    [Command]
    private void CmdBuyFloater(string floaterString, float cost, GameObject player)
    {
        lobbyHandler.GetComponent<LobbyHandler>().boatPoints -= cost;
        lobbyHandler.GetComponent<LobbyHandler>().RpcUpdateBoatPoints();
        //Resources.Load($"Floaters/{floaterString}") as GameObject
        GameObject newFlot = Instantiate(NetworkManager.singleton.spawnPrefabs.Find((X) => X.name == floaterString), new Vector3(-100, -100, 0), Quaternion.identity);
        newFlot.transform.parent = bote;
        //newFlot.GetComponent<Floater>().enabled = false;
        newFlot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        newFlot.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        newFlot.layer = 10;
        NetworkServer.Spawn(newFlot);
        newFlot.AddComponent<Buyables_Floater_Data>();
        newFlot.GetComponent<Buyables_Floater_Data>().nameString = floaterString;
        newFlot.GetComponent<Buyables_Floater_Data>().prefab = newFlot;
        newFlot.GetComponent<Buyables_Floater_Data>().isFloater = true;
        newFlot.GetComponent<Buyables_Floater_Data>().cost = cost;
        newFlot.transform.parent = bote;
        player.GetComponent<LobbyBuilder>().floater = newFlot;
    }

    //Mueve un objeto que ya es parte de la balsa
    [Command]
    private void CmdMoveFloater(Vector3 ghostPos, Quaternion ghostRot, GameObject adoptedFloater)
    {
        //ver si no se puede mover directamente sin desespawnear
        NetworkServer.UnSpawn(adoptedFloater);
        adoptedFloater.transform.SetPositionAndRotation(ghostPos, ghostRot);
        NetworkServer.Spawn(adoptedFloater);

    }

    //Despawnea el objeto
    [Command]
    private void CmdSellFloater(GameObject soldFloater)
    {
        cost = soldFloater.GetComponent<Buyables_Floater_Data>().cost;
        lobbyHandler.GetComponent<LobbyHandler>().boatPoints += cost;
        lobbyHandler.GetComponent<LobbyHandler>().RpcUpdateBoatPoints();
        NetworkServer.Destroy(floater);
    }
}
