using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{

	public Transform currentOwner;
	public void SetOwner(Transform newOwner)
	{
		currentOwner = newOwner;

	}
}
