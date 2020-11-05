using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class TrashCollector : MonoBehaviour
{
    
    public GameObject Ghost;//Ghost es el fantasma verde guia de construccion
    public bool has_floater = false; // indica si el personaje tiene agarrado algo
    public bool has_item = false; //indica si el personaje tiene un objeto
    // Alcance para colocar objetos
    [Range(0.01f, 1f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    private GameObject item;
    private int dir = 1;
    // Variables de objetos del mundo
    private GameObject Bote;
    private Transform _areaefecto;
    private SpriteRenderer ghostRender;

    
   
    private void Start()
    {
        Ghost.transform.SetParent(GameObject.Find("EffectHolder").transform);
        Bote = GameObject.Find("Bote");
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line=_areaefecto.gameObject.GetComponent<LineRenderer>();
        ghostRender = Ghost.GetComponent<SpriteRenderer>();
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

                    floater.GetComponent<Waver>().Adopcion(Ghost.transform);
                    Ghost.SetActive(false);
                    has_floater = false;
                    _line.enabled = false;
                    
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

                    //Activa el efecto fantasma y copia el sprite
                    var floaterRender = floater.GetComponent<SpriteRenderer>();
                    ghostRender.sprite = floaterRender.sprite;
                    Ghost.transform.position = floater.transform.position; 
                    Ghost.transform.localScale = floater.transform.lossyScale;
                    Ghost.transform.rotation = floater.transform.localRotation;
                    Ghost.SetActive(true);
                }

                if (hit.collider != null && hit.collider.tag == "Item" && !has_item)
                {
                    item = hit.collider.gameObject;
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
            floater.GetComponent<Rigidbody2D>().velocity = gameObject.transform.right *2f * dir + transform.up *1.5f;
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
            Ghost.GetComponent<TargetJoint2D>().target = new Vector2(puntero[0], puntero[1]);
        }

    }

}