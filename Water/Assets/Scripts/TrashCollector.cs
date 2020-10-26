using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public bool has_floater=false;
    [Range(0.01f, 1f)]
    public float alcance = 0.3f;
    private GameObject floater;
    private GameObject Bote;
    private Transform _areaefecto;
    LineRenderer _line;
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
                Debug.Log(Vector2.Distance(m_puntero, gameObject.transform.position));
                if(Vector2.Distance(m_puntero, gameObject.transform.position)<=alcance){
                    floater.transform.position = m_puntero;
                    floater.transform.SetParent(Bote.transform);
                    floater.GetComponent<FixedJoint2D>().connectedBody = Bote.GetComponent<Rigidbody2D>();
                    has_floater = false;
                    _line.enabled = false;
                }
                else{
                    Debug.Log("muy lejos...");
                }
                
            }
            else{
                if (hit.collider != null)
                {
                    floater = hit.collider.gameObject;
                    floater.transform.SetParent(gameObject.transform);
                    floater.transform.position = gameObject.transform.position + new Vector3(0, 0.3f, 0);
                    floater.GetComponent<FixedJoint2D>().connectedBody=gameObject.GetComponent<Rigidbody2D>();
                    floater.GetComponent<FixedJoint2D>().enabled = true;
                    has_floater = true;
                    _line.enabled = true;
                }
            }
        }
        if (has_floater)
        {
            _areaefecto.transform.Rotate(0,0,1f, Space.Self);
        }
    }
}