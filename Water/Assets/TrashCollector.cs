using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrashCollector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                hit.collider.gameObject.transform.SetParent(gameObject.transform);
                hit.collider.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);
            }
        }
    }
}