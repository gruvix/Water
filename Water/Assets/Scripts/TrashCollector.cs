using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    
    public bool has_floater=false; // indica si el personaje tiene agarrado algo
    // Alcance para colocar objetos
    [Range(0.01f, 1f)]
    public float alcance = 0.4f;
    LineRenderer _line; // Linea que indica el rango
    private GameObject floater;
    // Variables de objetos del mundo
    private GameObject Bote;
    private Transform _areaefecto;
    
    

    private void Start()
    {
        Bote = GameObject.Find("Bote");
        _areaefecto = gameObject.transform.Find("Areadeefecto");
        _line=_areaefecto.gameObject.GetComponent<LineRenderer>();
        //Debug.Log(AreaEfecto);
    }
    [Client]
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
                    floater.transform.position = m_puntero;
                    floater.transform.SetParent(Bote.transform);
                    floater.GetComponent<FixedJoint2D>().connectedBody = Bote.GetComponent<Rigidbody2D>();
                    has_floater = false;
                    _line.enabled = false;
                    Bote.GetComponentInParent<SetMaterial>().Resolve();
                }
                else{
                    Debug.Log("muy lejos...");
                }
            }
            // Si el jugador no tiene nada agarrado agarra lo que haya clicleado si es que clickeo algo
            else{
                Debug.Log("clickeando sobre una " + hit.collider.name);
                if (hit.collider != null && hit.collider.tag == "Floater")
                {
                    floater = hit.collider.gameObject;
                    floater.transform.SetParent(gameObject.transform);
                    floater.transform.position = gameObject.transform.position + new Vector3(0, 0.3f, 0);
                    floater.GetComponent<FixedJoint2D>().connectedBody=gameObject.GetComponent<Rigidbody2D>();
                    floater.GetComponent<FixedJoint2D>().enabled = true;
                    has_floater = true;
                    _line.enabled = true;
                    Debug.Log(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {

        }
        if (has_floater)
        {
            _areaefecto.transform.Rotate(0,0,1f, Space.Self);
        }
    }
}