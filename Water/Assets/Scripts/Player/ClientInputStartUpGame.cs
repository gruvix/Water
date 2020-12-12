using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientInputStartUpGame : NetworkBehaviour
{

    private Camera cam;
    void Start()
    {
        if(!hasAuthority) { return; }
    }
}
