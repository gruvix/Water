using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
	#pragma warning disable 0649// <- evita el warning de "null" en unity
	private string[] ItemIDs;
    #pragma warning restore 0649
	private int i = 1;

    void Start()
    {
        string[] ItemIDs = new string[i];
        ItemIDs[0] = "SpaceGun";
    }

	public void MakeItem()
	{
		string ID = ItemIDs[Random.Range(0, ItemIDs.Length)];
		Resources.Load<GameObject>("Items/"+ID);
	}
}
