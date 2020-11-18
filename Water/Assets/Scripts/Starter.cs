using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Starter : MonoBehaviour
{
    void Start()
    {
        foreach (GameObject i in Resources.LoadAll("Floaters", typeof(GameObject)))
        {
            FindObjectOfType<NetworkManager>().spawnPrefabs.Add(i);
            Debug.Log("Prefab Cargado: " + i);
        }

    }
}
