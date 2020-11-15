using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private float speedLimit = 7f;
    public bool CanPlace = true;
    private float distanciaGhost = 0.8f;
    private int counter = 0;
    private Component copyCollider = null;
    private Component copyTrigger = null;
    private System.Type type = null;
    private Vector2 m_puntero;
    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (Mathf.Abs(collider.relativeVelocity.magnitude) <= speedLimit || !CanPlace)
        {
            gameObject.GetComponent<TargetJoint2D>().maxForce = 0.012f;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 0);
            CanPlace = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    void OnTriggerEnter2D()
    {
        counter++;
    }

    void OnTriggerExit2D()
    {
        counter--;
    }

    void Update()
    {
        m_puntero = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.GetComponent<TargetJoint2D>().target = m_puntero;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(m_puntero, gameObject.transform.position) >= distanciaGhost)
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 0);
            CanPlace = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }

        if (counter == 0)
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 1);
            CanPlace = true;
            gameObject.GetComponent<Collider2D>().enabled = true;
            gameObject.GetComponent<TargetJoint2D>().maxForce = 0.1f;
        }
    }

    public void SetCollider(GameObject original)
    {
        if (original.GetComponent<Collider2D>() != null)
        {
            CopyComponent(original.GetComponent<Collider2D>(), copyCollider);
            CopyComponent(original.GetComponent<Collider2D>(), copyTrigger);
            Collider2D[] colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (i == 1)
                {
                    colliders[i].isTrigger = true;
                }
            }
        }
    }

    public void DestroyCollider()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }

    }

    public Component CopyComponent(Component original, Component copy)
    {

        type = original.GetType();
        copy = gameObject.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        return copy;
    }
}
