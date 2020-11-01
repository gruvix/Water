﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGun : MonoBehaviour
{
	public AudioSource audio;
	Vector3 mouse_pos;
	Vector3 object_pos;
	Transform target;
	float angle;
	float radians;
	public float playerHeight = 0.1f;//Altura a la q va el arma
	public float offset = 0.05f; //Que tan lejos está el arma del personaje
	public float compensation = -38f;//compensa que la punta del arma no está en la base del sprite


    public void SetItem(Transform owner)
    {
    	target = owner;
    	audio = GetComponent<AudioSource>();
    	gameObject.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
     		mouse_pos = Input.mousePosition;
     		mouse_pos.z = 10; //The distance between the camera and object
     		object_pos = Camera.main.WorldToScreenPoint(target.position) + new Vector3(0, playerHeight, 0);
     		mouse_pos.x = mouse_pos.x - object_pos.x;
     		mouse_pos.y = mouse_pos.y - object_pos.y + compensation;
     		radians = Mathf.Atan2(mouse_pos.y, mouse_pos.x);
    		angle = radians * Mathf.Rad2Deg;
    		if (angle > 90 || angle < -90)
    		{
    			transform.rotation = Quaternion.Euler(new Vector3(180, 180, angle));
    			gameObject.GetComponent<SpriteRenderer>().flipX = true;
    		}
    		else
    		{
    			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    			gameObject.GetComponent<SpriteRenderer>().flipX = false;
    		}

        	gameObject.transform.position = target.position + new Vector3(Mathf.Cos(radians) * offset, Mathf.Sin(radians) * offset + playerHeight, 0) ;

        }
            if (Input.GetMouseButtonDown(2))
        {
				audio.Play();
        }
    }
}