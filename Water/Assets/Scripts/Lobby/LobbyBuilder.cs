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
    [SerializeField]
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
        Ghost.GetComponent<Ghost>().isImage = true;
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
                ghostCheck = Ghost.GetComponent<Ghost>().CanPlace;
                if (ghostCheck)
                {
                    CmdAdopcion(Ghost.transform.position, Ghost.transform.rotation, floater);
                    Ghost.GetComponent<Ghost>().DestroyCollider();
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
                        floater = hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().prefab;
                        cost = hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().cost;

                        if (hit.collider.gameObject.GetComponent<Buyables_Floater_Data>().isFloater)
						{
                            //CmdTransicion(ClientScene.localPlayer.gameObject, floater);
                        }

                        else 
                        { 
                            if(lobbyHandler.GetComponent<LobbyHandler>().boatPoints < cost)
							{
								Debug.Log("Insuficiente Dinero");
							}
                            else
							{
                                CmdBuyFloater(floaterName, cost, gameObject);
                                has_floater = true;
                                Ghost.SetActive(true);
                                Ghost.GetComponent<Ghost>().SetCollider(floater);
                                Ghost.transform.rotation = Quaternion.identity;
                            }
                        }

                        

                    }
                }
            }

        }
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


        //Soltar Objeto (Huerfano)
        if (Input.GetMouseButtonDown(1) && has_floater)
        {
            Ghost.GetComponent<Ghost>().DestroyCollider();
            CmdHuerfano(0, transform, floater);
            has_floater = false;
        }
    }

    [Command]
    private void CmdBuyFloater(string floaterString, float cost, GameObject player)
    {

        lobbyHandler.GetComponent<LobbyHandler>().boatPoints -= cost;
        lobbyHandler.GetComponent<LobbyHandler>().RpcUpdateBoatPoints();

        GameObject newFlot = Instantiate(Resources.Load($"Floaters/{floaterString}") as GameObject, new Vector3(-100, -100, 0), Quaternion.identity, bote);
        //newFlot.GetComponent<Floater>().enabled = false;
        newFlot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        newFlot.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        newFlot.layer = 10;


        newFlot.AddComponent<Buyables_Floater_Data>();
        newFlot.GetComponent<Buyables_Floater_Data>().nameString = floaterString;
        newFlot.GetComponent<Buyables_Floater_Data>().prefab = floater;
        newFlot.GetComponent<Buyables_Floater_Data>().isFloater = true;
        newFlot.GetComponent<Buyables_Floater_Data>().cost = cost;
        NetworkServer.Spawn(newFlot);

        NetworkConnection conn = player.GetComponent<NetworkIdentity>().connectionToClient;
        RpcBuyFloater(conn, newFlot);
    }

    [TargetRpc]
    private void RpcBuyFloater(NetworkConnection conn, GameObject newFlot)
	{
        floater = newFlot;
	}


    [ClientRpc]
    private void RpcTransicion()
	{
        Debug.Log("Comprable clickeado");
	}

    [Command]
    private void CmdAdopcion(Vector3 ghostPos, Quaternion ghostRot, GameObject adoptedFloater)
    {
        NetworkServer.UnSpawn(adoptedFloater);
        adoptedFloater.transform.SetPositionAndRotation(ghostPos, ghostRot);
        NetworkServer.Spawn(adoptedFloater);

    }

    [ClientRpc]
    private void RpcAdopcion(Vector3 ghostPos, Quaternion ghostRot, string floaterString)
    {
        GameObject newFlot = Instantiate(Resources.Load($"Floaters/{floaterString}") as GameObject, ghostPos, ghostRot, bote);
        newFlot.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        newFlot.layer = 10;

        if (newFlot.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            newFlot.GetComponent<PlatformEffector2D>().enabled = true;
        }

        newFlot.GetComponent<Rigidbody2D>().simulated = true;
        newFlot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

    }

    [Command]
    private void CmdHuerfano(int dir, Transform playerTran, GameObject floater)
    {
        RpcHuerfano();
    }

    [ClientRpc]
    private void RpcHuerfano()
    {
        Debug.Log("Comprable cancelado");
    }

}
