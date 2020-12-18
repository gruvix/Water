using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet1 : NetworkBehaviour
{
	public float speed = 20f;
	private Rigidbody2D rb;

    void Start()
    {
    	rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.GetComponent<Floater>() != null)
    	{
    		hitInfo.gameObject.GetComponent<Floater>().Damage(20f);
    	}
    	NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.name == "KillBox") { NetworkServer.Destroy(gameObject); }
    }
}
