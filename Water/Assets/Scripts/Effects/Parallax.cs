using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	private float length, startpos;
	public GameObject bg;
	public float parallaxEffect;
	private float bgLenght;
	private float pointCero;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        bgLenght = bg.GetComponent<SpriteRenderer>().bounds.size.x;
        pointCero = bg.transform.position.x - (bgLenght / 2) - length;
    }

    // Update is called once per frame
    void Update()
    {
    	
    	float temp = (transform.position.x * (1 - parallaxEffect));
        float dist = (transform.position.x - pointCero);

        transform.position = new Vector3(transform.position.x + 0.05f * parallaxEffect, transform.position.y, transform.position.z);

        if (dist > (bgLenght + length * 2)) 
        {
        	transform.position = new Vector3(pointCero, transform.position.y, transform.position.z);
        }

    }
}
