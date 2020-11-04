using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class TrashCollector : MonoBehaviour
{
    
    public bool has_floater = false; // indica si el personaje tiene agarrado algo
    public bool has_item = false; //indica si el personaje tiene un objeto
    // Alcance para colocar objetos
    [Range(0.01f, 1f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    private GameObject item;
    // Variables de objetos del mundo
    private GameObject Bote;
    private Transform Ghost;//Ghost es el fantasma verde guia de construccion
    private Transform _areaefecto;
    
   
    private void Start()
    {
        Bote = GameObject.Find("Bote");
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line=_areaefecto.gameObject.GetComponent<LineRenderer>();
        //Debug.Log(AreaEfecto);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(m_puntero, Vector2.zero);
            if (has_floater){


                // Si tiene algo agarrado y haces click lo deja donde hiciste click y se lo da al bote
                //Debug.Log(Vector2.Distance(m_puntero, gameObject.transform.position));
                if(Vector2.Distance(m_puntero, gameObject.transform.position)<=alcance){

                    floater.GetComponent<Waver>().Adopcion(m_puntero);

                    has_floater = false;
                    _line.enabled = false;
                    Ghost.gameObject.SetActive(false);
                }
                else{
                    Debug.Log("muy lejos...");
                }
            }
            // Si el jugador no tiene nada agarrado agarra lo que haya clicleado si es que clickeo algo
            else{
                
                if (hit.collider != null && hit.collider.tag == "Floater")
                {
                    floater = hit.collider.gameObject;
                    floater.GetComponent<Waver>().Transicion(gameObject);
                    has_floater = true;
                    _line.enabled = true;

                    Ghost = hit.collider.gameObject.transform.GetChild(0);
                    Ghost.gameObject.SetActive(true);;
                    

                }

                if (hit.collider != null && hit.collider.tag == "Item" && !has_item)
                {
                    item = hit.collider.gameObject;
                    item.GetComponent<SpaceGun>().SetItem(gameObject.transform);
                    has_item = true;
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ghost.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.rotation.eulerAngles.z + 1f));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ghost.rotation = Quaternion.Euler(new Vector3(0, 0, Ghost.rotation.eulerAngles.z -1f));
        }



        if (Input.GetMouseButtonDown(1) && has_floater)//Click derecho suelta el objeto
        {
        	Ghost.gameObject.SetActive(false);
            floater.GetComponent<Waver>().Huerfano();
            has_floater = false;
            _line.enabled = false;
        }

        if (Input.GetMouseButtonUp(0))
        {

        }


        if (has_floater)//Crea el fantasma verde del objeto
        {
            _areaefecto.transform.Rotate(0,0,1f, Space.Self);
            Vector2 puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ghost.gameObject.GetComponent<TargetJoint2D>().target = new Vector2(puntero[0], puntero[1]);
        }

    }


}