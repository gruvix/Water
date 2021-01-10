using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Nucleo : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Bote").transform);
		if (isServer) { GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; }
    }

}
