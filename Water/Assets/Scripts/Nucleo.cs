using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Nucleo : NetworkBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
		if (isServer) { GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; }
    }

}
