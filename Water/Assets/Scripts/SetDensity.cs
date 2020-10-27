using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDensity : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float Densidad=1;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider2D>().density = Densidad;
    }

}
