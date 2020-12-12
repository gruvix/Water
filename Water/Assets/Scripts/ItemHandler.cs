using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{

	public System.Type wepType;
	public Transform currentOwner;

	public void SetOwner(Transform newOwner)
	{
		currentOwner = newOwner;
		var weapon = GetComponent(wepType);
		//weapon.SetItem();
	}

	//Copy Sprite
	//Get Type -> add component "type"
}
