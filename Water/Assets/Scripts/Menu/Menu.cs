using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Menu : MonoBehaviour
{
    public GameObject MultiplayerMenuManager;

    void Start()
    {
        MultiplayerMenuManager.SetActive(true);
    }

}
