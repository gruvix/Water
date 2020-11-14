using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandler : MonoBehaviour
{
    string Name;
    public bool IsReady;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void ReadyChange()
    {
        IsReady = !IsReady;
    }
}
