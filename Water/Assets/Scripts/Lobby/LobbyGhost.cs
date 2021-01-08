using UnityEngine;

public class LobbyGhost : MonoBehaviour
{
    private float speedLimit = 30f;
    public bool CanPlace = true;
    private float distanciaGhost = 3f;
    private int counter = 0;
    private Component copyCollider = null;
    private Component copyTrigger = null;
    private System.Type type = null;
    private Vector2 m_puntero;
    private float alcance = 10f;
    private bool colliding;


    void OnCollisionEnter2D(Collision2D collider)
    {
        if (Mathf.Abs(collider.relativeVelocity.magnitude) <= speedLimit && CanPlace)
        {
            gameObject.GetComponent<TargetJoint2D>().maxForce = 0.1f;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 0);
            CanPlace = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        colliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (counter == 0) { colliding = false; }
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
        var dist = Vector2.Distance(m_puntero, transform.position);
        if (dist >= distanciaGhost && colliding)
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 0);
            CanPlace = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }

        if (counter == 0)
        {
            var dist2 = Vector2.Distance(transform.position, transform.parent.position);
            if (dist2 <= alcance)
            {
                gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 1);
                CanPlace = true;
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
            gameObject.GetComponent<TargetJoint2D>().maxForce = 0.4f;
        }
    }

    public void SetCollider(GameObject original)
    {
        GetComponent<SpriteRenderer>().sprite = original.GetComponent<SpriteRenderer>().sprite;
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

        transform.position = original.transform.position;
        float x = original.transform.localScale.x / gameObject.transform.parent.lossyScale.x;
        float y = original.transform.localScale.y / gameObject.transform.parent.lossyScale.y;
        float z = original.transform.localScale.z / gameObject.transform.parent.lossyScale.z;
        transform.localScale = new Vector3(x * 0.186f, y * 0.186f, z);
        transform.rotation = original.transform.rotation;
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
