﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_graph_update : NetworkBehaviour
{

    private int count_time = 0;

    // Update is called once per frame
    [Server]
    void Update()
    {
        if (count_time >= 100)
        {
            count_time = 0;
            AstarPath.active.Scan();
        }
        else
        {
            count_time++;
        }
        
    }
}
