using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
	private Transform owner;
	public AudioSource audio;

    void SetItem(Transform ownerTrans)
    {
    	owner = ownerTrans;
    	audio = GetComponent<AudioSource>();
    	gameObject.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (owner != null)
        {
        	var vec1 = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        	var vec2 = new Vector2(owner.transform.position.x, owner.transform.position.y);
        	var rot = gameObject.transform.rotation.z;
        	rot = Vector2.Angle(vec1, vec2);
        }
            if (Input.GetMouseButtonUp(2))
        {
				audio.Play();
        }
    }
}
