using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public bool has_floater=false;
    private GameObject floater;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(m_puntero, Vector2.zero);
            if (has_floater){

            }
            else{
                if (hit.collider != null)
                {
                    hit.collider.gameObject.transform.SetParent(gameObject.transform);
                    hit.collider.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0.3f, 0);
                    
                    hit.collider.gameObject.GetComponent<FixedJoint2D>().connectedBody=gameObject.GetComponent<Rigidbody2D>();
                    hit.collider.gameObject.GetComponent<FixedJoint2D>().enabled = true;
                }
            }
        }
    }
}