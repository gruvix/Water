using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyBuilder : NetworkBehaviour
{
    public Transform bote;
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    private bool ghostCheck = true;
    //[SerializeField]
    private bool has_floater = false;
    public GameObject floater;
    //[SerializeField]
    private string floaterName;
    private float rotationCounter = 0;
    private float rotationAmount = 30f;
    private float alcance = 20f;
    private GameObject lobbyHandler;

    private Hashtable precios = new Hashtable();

    void Start()
    {
        bote = GameObject.Find("Bote").transform;
        lobbyHandler = GameObject.Find("LobbyCanvas");

        //Lista de precios
        precios.Add("Crate1", 2f);
        precios.Add("Crate1(Clone)", 2f);
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
                //si puede colocarlo
                if (ghostCheck)
                {
                    if (floater.transform.parent.name == "Bote")
                    {
                        CmdSpawnandPlaceFloater(floaterName, Ghost.transform.position, Ghost.transform.rotation);
                        CmdDestroyFloater(floater);
                    }
                    else
                    {
                        CmdSpawnandPlaceFloater(floaterName, Ghost.transform.position, Ghost.transform.rotation);
                        CmdDiscountValue(floaterName);
                    }
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
                        floater = hit.collider.gameObject;
                        has_floater = true;
                        floaterName = hit.collider.gameObject.name;
                        
                        Debug.Log("hiciste click en el floater: " + floater);
                        
                        //Si es un objeto que ya compre
                        if (hit.collider.transform.parent == bote)
						{
                            Ghost.SetActive(true);
                            Ghost.GetComponent<LobbyGhost>().SetCollider(floater);
                            Ghost.transform.rotation = Quaternion.identity;
                            Ghost.transform.position = new Vector3(m_puntero[0], m_puntero[1], Ghost.transform.position.z);
                            CmdMoveFloater(new Vector3(5,5,0), floater.transform.rotation, floater);
                        }

                        //Si es de la lista de compra
                        else 
                        { 
                            //Si no podes pagarlo
                            if(lobbyHandler.GetComponent<LobbyHandler>().boatPoints < (float)precios[floaterName])
							{
								Debug.Log("Insuficiente Dinero");
							}
                            //Si podes pagarlo
                            else
							{
                                //CmdBuyFloater(floaterName, cost, gameObject); //no se le puede dar a un jugador un objeto del server
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
			if (floater) {
                if (floater.transform.parent.name == "Bote")
                {
                    CmdSellFloater(floater);
                }
                else
                {
                    //CmdDismisFloater(floater);
                }
            }
            Ghost.GetComponent<LobbyGhost>().DestroyCollider();
            Ghost.SetActive(false);
            has_floater = false;
        }
    }


    //Mueve un objeto que ya es parte de la balsa
    [Command]
    private void CmdMoveFloater(Vector3 ghostPos, Quaternion ghostRot, GameObject adoptedFloater)
    {
        //ver si no se puede mover directamente sin desespawnear
        adoptedFloater.transform.SetPositionAndRotation(ghostPos, ghostRot);
    }

    [Command]
    private void CmdSpawnandPlaceFloater(string floaterString, Vector3 ghostPos, Quaternion ghostRot)
    {
        //Quitar el (Clone) del final del string. 
        //Si existe el caracter ')' entonces elimina los ultimos 7 caracteres de string que coresponden a (Clone)
        if (floaterString.LastIndexOf(')')!=-1)
        {
            floaterString = floaterString.Remove(floaterString.Length - 7);
            Debug.Log("String limpio: " + floaterString);
        }
        GameObject newFlot = Instantiate(NetworkManager.singleton.spawnPrefabs.Find((X) => X.name == floaterString), new Vector3(-100, -100, 0), Quaternion.identity);
        newFlot.transform.parent = GameObject.Find("Bote").transform;
        NetworkServer.Spawn(newFlot);
        newFlot.GetComponent<Floater>().Adopcion(ghostPos, ghostRot);
    }

    //Despawnea el objeto
    [Command]
    private void CmdSellFloater(GameObject soldFloater)
    {
        CmdRefundValue(soldFloater.name);
        soldFloater.GetComponent<Floater>().CmdDestroy();
    }
    [Command]
    private void CmdDestroyFloater(GameObject floatertodestroy)
    {
        floatertodestroy.GetComponent<Floater>().CmdDestroy();
    }
    [Command]
    private void CmdDiscountValue(string floaterString)
    {
        lobbyHandler.GetComponent<LobbyHandler>().boatPoints -= (float)precios[floaterString];
    }
    private void CmdRefundValue(string floaterString)
    {
        lobbyHandler.GetComponent<LobbyHandler>().boatPoints += (float)precios[floaterString];
    }
}
