using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class AI_target_change : MonoBehaviour
{

	private GameObject NPC_list;

	private void Awake()
	{
		NPC_list = GameObject.Find("NPCs");
	}

	private void Start()
	{
		foreach(Transform child in NPC_list.transform)
		{
			TargetThis(child);
		}
	}

	void TargetThis(Transform targeter)
	{
		targeter.GetComponent<AIDestinationSetter>().target = gameObject.transform;
	}
}
