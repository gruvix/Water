using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class Collector : NetworkBehaviour
{
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    private SpriteRenderer ghostRender;
    private bool ghostCheck = true;
    public GameObject CarriedObject; //El sprite que le muestra a los demas que tenes agarrado
    private SpriteRenderer pickedRender;
    [SyncVar]
    public bool has_floater = false; // indica si el personaje tiene agarrado algo
    [SyncVar]
    public bool has_item = false; //indica si el personaje tiene un objeto
    // Alcance para colocar objetos
    [Range(0.01f, 5f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    private GameObject floateronclient;
    [SyncVar(hook = nameof(OnChangeFloater))]
    private int nFloater;
    private GameObject item;
    private int dir = 1;
    // Variables de objetos del mundo
    private GameObject Bote;
    private Transform _areaefecto;
    private GameObject Gema;

    [Client]
    public override void OnStartClient()
    {
        Bote = GameObject.Find("Bote");
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line = _areaefecto.gameObject.GetComponent<LineRenderer>();
        ghostRender = Ghost.GetComponent<SpriteRenderer>();
        Ghost.SetActive(false);
        Gema = GameObject.Find("SoulFragment");

        CarriedObject = gameObject.transform.Find("CarriedObject").gameObject;
        CarriedObject.SetActive(false);
        pickedRender = CarriedObject.GetComponent<SpriteRenderer>();
    }
    [Client]
    private void Update()
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
                if (Vector2.Distance(m_puntero, gameObject.transform.position) <= alcance && ghostCheck)
                {
                    CmdAdopcion(Ghost.transform.position, Ghost.transform.rotation, floater);
                    Ghost.GetComponent<Ghost>().DestroyCollider();
                    has_floater = false;
                    floater = null;
                    Ghost.SetActive(false);
                }
                else
                {
                    Debug.Log("muy lejos...");
                }
            }

            // Si el jugador no tiene nada agarrado agarra lo que haya cliqueado si es que clickeo algo y no tiene dueño
            else
            {

                if ((hit.collider != null && (hit.collider.tag == "Floater" || hit.collider.tag == "FloaterPlatform")) && hit.collider.transform.parent.tag != "Player")//Cuando el objeto es un floater
                {
                    floater = hit.collider.gameObject;
                    CmdTransicion(ClientScene.localPlayer.gameObject, floater);
                    has_floater = true;
                    Ghost.SetActive(true);
                    Ghost.GetComponent<Ghost>().SetCollider(floater);
                }

                if (hit.collider != null && hit.collider.tag == "Item" && !has_item)//Cuando el objeto es un arma/herramienta
                {
                    item = hit.collider.gameObject;

                    item.GetComponent<SpaceGun>().enabled = true;
                    item.GetComponent<SpaceGun>().SetItem(gameObject.transform);

                    has_item = true;
                }

            }

        }

        if (Input.GetKey(KeyCode.Q))
        {
            Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z + 2));
        }
        if (Input.GetKey(KeyCode.E))
        {
            Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z - 2));
        }

        if (Input.GetMouseButtonDown(1) && has_floater)//Click derecho suelta el objeto
        {
            Ghost.SetActive(false);
            if (gameObject.transform.localScale.x > 0)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }
            Ghost.GetComponent<Ghost>().DestroyCollider();
            CmdHuerfano(dir, transform, floater);
            has_floater = false;
            _line.enabled = false;
        }

        if (has_floater)
        {
            _areaefecto.transform.Rotate(0, 0, 1f, Space.Self);
        }
    }

    [Command]
    private void CmdTransicion(GameObject player, GameObject floater)
    {
        floater.GetComponent<Floater>().Transicion(player);
    }

    [Command]
    private void CmdAdopcion(Vector3 ghostPos, Quaternion ghostRot, GameObject floater)
    {
        floater.GetComponent<Floater>().Adopcion(ghostPos, ghostRot);
        nFloater = -1;
    }

    [Command]
    private void CmdHuerfano(int dir, Transform playerTran, GameObject floater)
	{
        floater.GetComponent<Floater>().Huerfano(dir, playerTran);
    }






    [Client]
    public void GetPrefab(GameObject pickedupfloater)
    {
        int nf = 0;
        foreach (GameObject i in NetworkManager.singleton.spawnPrefabs)
        {
            Debug.Log("i es: " + i.GetComponent<NetworkIdentity>().assetId + " y estoy buscando: " + pickedupfloater.GetComponent<NetworkIdentity>().assetId);
            Debug.Log("i es: " + i.name + " y estoy buscando: " + pickedupfloater.name);
            if (i.name == pickedupfloater.name || i.GetComponent<NetworkIdentity>().assetId == pickedupfloater.GetComponent<NetworkIdentity>().assetId)
            {
                floater = i;
                nFloater = nf;
                return;
            }
            nf++;
        }
        nFloater = -1;
        floater = null;
        return;
    }

    [Client]
    public void ChangeGohstItem(GameObject floater_)
    {
        if (!hasAuthority) { return; }
        if (floater_)
        {
            _line.enabled = true;
            Ghost.transform.position = floater_.transform.position;
            Ghost.transform.localScale = floater_.transform.localScale / transform.localScale[0];
            Ghost.transform.rotation = floater_.transform.rotation;
            ghostRender.sprite = floater_.GetComponent<SpriteRenderer>().sprite;
            Ghost.SetActive(true);
            Ghost.GetComponent<Ghost>().SetCollider(floater_);
        }
        else
        {
            _line.enabled = false;
            Ghost.transform.position = new Vector3(0, 0, 0);
            Ghost.transform.localScale = transform.localScale;
            Ghost.transform.rotation = transform.rotation;
            ghostRender.sprite = null;
            Ghost.SetActive(false);
            ghostRender.enabled = false;
            Ghost.GetComponent<Ghost>().DestroyCollider();
        }
    }

    [Client]
    private void OnChangeFloater(int oldFloater, int newFloater)
    {
        if (newFloater >= 0)
        {
            floateronclient = NetworkManager.singleton.spawnPrefabs[newFloater];
            Debug.Log(floateronclient);
            CarriedObject.SetActive(true);
            pickedRender.sprite = floateronclient.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            CarriedObject.SetActive(false);
            pickedRender.sprite = null;
        }
    }

    [Client]
    void NetworkDestroy(GameObject Object)
    {
        Debug.Log("Destruyendo objeto" + Object);
        //Get the NetworkIdentity assigned to the object
        NetworkIdentity id = Object.GetComponent<NetworkIdentity>();
        // Check if we successfully got the NetworkIdentity Component from our object, if not we return(essentially do nothing).
        if (id == null) return;
        // First check if the objects NetworkIdentity can be transferred, or if it is server only.
        if (hasAuthority)
        {
            // Do we already own this NetworkIdentity? If so, don't do anything.
            if (id.hasAuthority == false)
            {
                // If we do not already have authority over the NetworkIdentity, assign authority.
                // Keep in mind, using connectionToClient to get this NetworkIdentity is only valid for Network Player Objects.
                if (id.AssignClientAuthority(connectionToClient) == true)
                {
                    // If takeover was successful, we can now destroy our GameObject.
                    NetworkServer.Destroy(Object);
                }
            }
            else
            {
                // Do nothing because we already have ownership of this NetworkIdentity.
            }
        }
        else
        {
            //Server only, so we can't do anything.
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdNetworkDestroy(GameObject Object)
    {
        NetworkServer.Destroy(Object);
    }


    //este comando da autoridsad sobre el objeto que cliqueas
    [Command]
    public void CmdSetAuthority(NetworkIdentity iobject, NetworkIdentity player)
    {
        //Checks if anyone else has authority and removes it and lastly gives the authority to the player who interacts with object
        //iobject.RemoveClientAuthority();
        bool aut = iobject.AssignClientAuthority(player.connectionToClient);
        Debug.Log("dando autoridad de " + iobject + " a " + player.connectionToClient + " y salio " + aut);
    }
}
