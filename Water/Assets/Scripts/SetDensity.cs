using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDensity : MonoBehaviour
{
    public int Densidad;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider2D>().density = Densidad;
    }

}
