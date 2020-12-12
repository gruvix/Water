using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
	public float speed = 20f;
	private Rigidbody2D rb;
    public bool doDamage = false;
    

    void Start()
    {
    	rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.GetComponent<Floater>() != null && doDamage)
    	{
    		hitInfo.gameObject.GetComponent<Floater>().Damage(20f);
    	}
    	Destroy(gameObject);
    }

}
