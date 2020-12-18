using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class Collector : NetworkBehaviour
{
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    private bool ghostCheck = true;
    [SyncVar]
    public bool has_floater = false; // indica si el personaje tiene agarrado algo
    [SyncVar]
    public bool has_item = false; //indica si el personaje tiene un objeto
    // Alcance para colocar objetos
    [Range(0.01f, 5f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    private GameObject item;
    private int dir = 1;
    // Variables de objetos del mundo
    private Transform _areaefecto;

    [Client]
    public override void OnStartClient()
    {
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line = _areaefecto.gameObject.GetComponent<LineRenderer>();
        Ghost.GetComponent<Ghost>().alcance = alcance;
        Ghost.SetActive(false);
        _areaefecto.gameObject.SetActive(true);
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
                if (ghostCheck)
                {
                    CmdAdopcion(Ghost.transform.position, Ghost.transform.rotation, floater, gameObject);
                    Ghost.GetComponent<Ghost>().DestroyCollider();
                    has_floater = false;
                    floater = null;
                    Ghost.SetActive(false);
                }
            }

            // Si el jugador no tiene nada agarrado agarra lo que haya cliqueado si es que clickeo algo y no tiene dueño
            else
            {
                if(hit.collider != null && Vector2.Distance(m_puntero, gameObject.transform.position) <= alcance)
				{

                    if ((hit.collider.tag == "Floater" || hit.collider.tag == "FloaterPlatform") && hit.collider.transform.parent.tag != "Player")//Cuando el objeto es un floater
                    {
                        floater = hit.collider.gameObject;
                        
                        has_floater = true;
                        Ghost.SetActive(true);
                        Ghost.GetComponent<Ghost>().SetCollider(floater);
                        CmdTransicion(ClientScene.localPlayer.gameObject, floater);
                    }

                    else if (hit.collider.tag == "Item" && !has_item)//Cuando el objeto es un arma/herramienta
                    {
                        item = hit.collider.gameObject;
                        has_item = true;
                        CmdItemPickup(ClientScene.localPlayer.gameObject, item);
                    }
				}
            }
        }



        //Soltar Objeto (Huerfano)
        if (Input.GetMouseButtonDown(1) && has_floater)
        {
            Ghost.SetActive(false);
            if (gameObject.GetComponent<CharacterController2D>().m_FacingRight)
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
        else { return; }

        //Rotacion del Ghost, va ultimo por el return
        if (Input.GetKey(KeyCode.Q))
        {
            Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z + 2));
            floater.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z + 2));
        }
        if (Input.GetKey(KeyCode.E))
        {
            Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z - 2));
            floater.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z - 2));
        }

    }

    [Command]
    private void CmdTransicion(GameObject player, GameObject floater)
    {
        floater.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
        floater.GetComponent<Floater>().RpcTransicion(player);
    }

    [Command]
    private void CmdAdopcion(Vector3 ghostPos, Quaternion ghostRot, GameObject floater, GameObject player)
    {
        floater.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);

        NetworkServer.UnSpawn(floater);
        floater.GetComponent<Floater>().Adopcion(ghostPos, ghostRot);
    }

    [Command]
    private void CmdHuerfano(int dir, Transform playerTran, GameObject floater)
	{
        floater.GetComponent<NetworkIdentity>().AssignClientAuthority(playerTran.GetComponent<NetworkIdentity>().connectionToClient);
        floater.GetComponent<Floater>().CmdHuerfano(dir, playerTran);
    }

    [Command]
    private void CmdItemPickup(GameObject player, GameObject item)//Todo lo que continua debería hacerse en otro script que maneja todos los items... por ahora va directo a el unica arma "SpaceGun"
    {
        item.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
        RpcItemPickup(player, item);
    }

    [ClientRpc]
    private void RpcItemPickup(GameObject player, GameObject item)
	{
        item.transform.SetParent(player.transform);
        item.GetComponent<SpaceGun>().enabled = true;
        item.GetComponent<SpaceGun>().SetItem(gameObject.transform);
    }
}
