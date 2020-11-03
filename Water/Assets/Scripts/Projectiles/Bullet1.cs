using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
	public float speed = 20f;
	private Rigidbody2D rb;
    

    void Start()
    {
    	rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
    	if (hitInfo.tag == "Floater")
    	{
    		hitInfo.gameObject.GetComponent<Waver>().Damage(20f);
    	}
    	Destroy(gameObject);
    }

}
