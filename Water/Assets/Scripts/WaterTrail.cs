using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrail : MonoBehaviour
{
	public ParticleSystem Trail;
	public GameObject killer;
	private Rigidbody2D body;
	public Transform transform;
	[Range(2f, 30f)]
	public float Distance = 10;


    private void OnTriggerEnter2D(Collider2D col)
    {
    	body = col.GetComponent<Rigidbody2D>();
        if(col.transform.IsChildOf(transform))// chekea q sea del bote
        {
        	var Particle = Instantiate(Trail, col.transform.position, transform.rotation);
            Particle.transform.parent = col.transform;
        	var collider = Particle.collision;
        	collider.SetPlane(0, killer.transform);
        	Particle.enableEmission = true;
        	Particle.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        body = col.GetComponent<Rigidbody2D>();
        if(col.transform.IsChildOf(transform) && (transform.childCount > 0))
        {
            Destroy(col.transform.GetChild(0).gameObject);
        }
    }
}
