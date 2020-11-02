using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplashDrops : MonoBehaviour
{

	public ParticleSystem Drops;
	public GameObject killer;
	private Rigidbody2D body;
	[Range(100f, 5000f)]
	public float Force = 200;


    private void OnTriggerEnter2D(Collider2D col)
    {
    	body = col.GetComponent<Rigidbody2D>();
        if(col.tag != "WaterBox" && (Mathf.Abs(body.velocity.y) > 0.35f || Mathf.Abs(body.velocity.x) > 1f))
        {
        	var Particle = Instantiate(Drops, col.transform.position, transform.rotation);
            Particle.transform.SetParent(GameObject.Find("EffectHolder").transform);
        	var main =  Particle.main;
        	var collider = Particle.collision;
            var renderer = Particle.GetComponent<Renderer>();
            renderer.sortingOrder = 2;
        	collider.SetPlane(0, killer.transform);
        	main.startSpeed = Mathf.Clamp(Mathf.Abs(body.velocity.y), 0.5f, 8f)* Force * body.mass;
        	Particle.Play();
        	Destroy(Particle.gameObject, 3f);
        }
    }
}
