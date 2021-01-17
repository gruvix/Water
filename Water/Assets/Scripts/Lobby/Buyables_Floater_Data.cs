using UnityEngine;
using Mirror;
public class Buyables_Floater_Data : NetworkBehaviour
{

	public string nameString;
	[SyncVar]
	public GameObject prefab;
	[SyncVar]
	public bool isFloater = false;
	public float cost = 2;

	private void Start()
	{
		if(transform.parent.name == "Buyables") { return; }
        transform.SetParent(GameObject.Find("Bote").transform);
    }
}
