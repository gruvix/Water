﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class TrashCollector : NetworkBehaviour
{
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    public bool has_floater = false; // indica si el personaje tiene agarrado algo
    public bool has_item = false; //indica si el personaje tiene un objeto
    // Alcance para colocar objetos
    [Range(0.01f, 5f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    private GameObject item;
    private int dir = 1;
    // Variables de objetos del mundo
    private GameObject Bote;
    private Transform _areaefecto;
    private SpriteRenderer ghostRender;
    private bool ghostCheck = true;

    
    [Client]
    public override void OnStartClient()
    {
        Bote = GameObject.Find("Bote");
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line=_areaefecto.gameObject.GetComponent<LineRenderer>();
        ghostRender = Ghost.GetComponent<SpriteRenderer>();
        Ghost.SetActive(false);
    }
    [Client]
    private void Update()
    {
        if (!hasAuthority) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(m_puntero, Vector2.zero);
            if (has_floater){
                // Si tiene algo agarrado y haces click lo deja donde hiciste click y se lo da al bote
                //Debug.Log(Vector2.Distance(m_puntero, gameObject.transform.position));
                ghostCheck = Ghost.GetComponent<Ghost>().CanPlace;
                if(Vector2.Distance(m_puntero, gameObject.transform.position)<=alcance && ghostCheck)
                {
                    Adopcion(floater);
                    has_floater = false;
                }
                else{
                    Debug.Log("muy lejos...");
                }
            }

            // Si el jugador no tiene nada agarrado agarra lo que haya clicleado si es que clickeo algo
            else{
                
                if (hit.collider != null && (hit.collider.tag == "Floater"|| hit.collider.tag == "FloaterPlatform"))
                {
                    floater = hit.collider.gameObject;
                    Transicion(floater);                    
                    has_floater = true;
                }

                if (hit.collider != null && hit.collider.tag == "Item" && !has_item)
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
        	Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z + 3));
        }
        if (Input.GetKey(KeyCode.E))
        {
            Ghost.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.transform.rotation.eulerAngles.z -3));
        }



        if (Input.GetMouseButtonDown(1) && has_floater)//Click derecho suelta el objeto
        {
        	Ghost.SetActive(false);
            floater.GetComponent<Waver>().Huerfano();
            if(gameObject.transform.localScale.x > 0)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }
            Ghost.GetComponent<Ghost>().DestroyCollider();
            floater.GetComponent<Rigidbody2D>().velocity = gameObject.transform.right *2f * dir + transform.up *1.5f;
            has_floater = false;
            _line.enabled = false;
        }

        if (has_floater)//Crea el fantasma verde del objeto
        {
            _areaefecto.transform.Rotate(0,0,1f, Space.Self);
        }
    }

    [Client]
    public void Transicion(GameObject pickedupfloater)
    {
        if (!hasAuthority) { return; }
        ChangeGohstItem(pickedupfloater);
        NetworkDestroy(pickedupfloater);
        //Hay que darle autoridad al jugador local
    }

    [Client]
    public void Adopcion(GameObject pickedupfloater)
    {
        if (!hasAuthority) { return; }
        ChangeGohstItem(null);

        //CmdCreatFloater(pickedupfloater);
    }
    /*
    [Command]
    public void CmdPickUpFloater(GameObject pickedupfloater)
    {
        ZeroPos = new Vector3(transform.position.x, transform.position.y+2,0)
        ZeroRot = Quaternion.identity;
        Instantiate(pickedupfloater, ZeroPos, ZeroRot);

        NetworkDestroy(pickedupfloater);
    }
    */
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
            Ghost.transform.position = new Vector3(0,0,0);
            Ghost.transform.localScale = transform.localScale;
            Ghost.transform.rotation = transform.rotation;
            ghostRender.sprite = null;
            Ghost.SetActive(false);
            Ghost.GetComponent<Ghost>().DestroyCollider();
        }
    }

    [Command]
    private void CmdCreatFloater()
    {
        GameObject newBoatObject = Instantiate(sceneObjectPrefab, pos, rot);
    }

    [Client]
    void NetworkDestroy(GameObject Object)
    {
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
