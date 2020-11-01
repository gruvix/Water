using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplashDrops : MonoBehaviour
{

	public ParticleSystem Drops;
	public GameObject killer;
	private Rigidbody2D body;
	[Range(2f, 30f)]
	public float Distance = 10;


    private void OnTriggerEnter2D(Collider2D col)
    {
    	body = col.GetComponent<Rigidbody2D>();
        if(col.tag != "WaterBox" && Mathf.Abs(body.velocity.y) > 0.5f)
        {
        	var Particle = Instantiate(Drops, col.transform.position, transform.rotation);
            Particle.transform.SetParent(GameObject.Find("EffectHolder").transform);
        	var main =  Particle.main;
        	var collider = Particle.collision;
        	collider.SetPlane(0, killer.transform);
        	main.startSpeed = Mathf.Abs(body.velocity.y)*Distance;
        	Particle.Play();
        	Destroy(Particle.gameObject, 3f);
        }
    }
}
