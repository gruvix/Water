using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Starter : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<NetworkManager>().spawnPrefabs.Add(Resources.Load("Other/Nucleo") as GameObject);
        foreach (GameObject i in Resources.LoadAll("Floaters", typeof(GameObject)))//Agrega los prefabs iniciales al NWM
        {
            FindObjectOfType<NetworkManager>().spawnPrefabs.Add(i);
        }
        foreach (GameObject i in Resources.LoadAll("Projectiles", typeof(GameObject)))//Agrega los prefabs iniciales al NWM
        {
            FindObjectOfType<NetworkManager>().spawnPrefabs.Add(i);
        }
    }
}