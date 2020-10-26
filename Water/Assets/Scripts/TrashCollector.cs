using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public bool has_floater=false;
    public float alcance = 1;
    private GameObject floater;
    private GameObject Bote;
    private LineRenderer AreaEfecto;
    private void Start()
    {
        Bote = GameObject.Find("Bote");
        AreaEfecto = gameObject.GetComponent<LineRenderer>();
        Debug.Log(AreaEfecto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(m_puntero, Vector2.zero);
            if (has_floater){
                // Si tiene algo agarrado y haces click lo deja donde hiciste click y se lo da al bote
                floater.transform.position = m_puntero;
                floater.transform.SetParent(Bote.transform);
                floater.GetComponent<FixedJoint2D>().connectedBody = Bote.GetComponent<Rigidbody2D>();
                Debug.Log(floater.GetComponent<FixedJoint2D>().connectedBody);
                has_floater = false;
                AreaEfecto.enabled = false;
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
                    AreaEfecto.enabled = true;
                }
            }
        }
        if (has_floater)
        {

        }
    }
}