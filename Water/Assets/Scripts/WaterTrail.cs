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
    private Vector3 position;


    private void OnTriggerEnter2D(Collider2D col)
    {
    	body = col.GetComponent<Rigidbody2D>();
        if(col.transform.IsChildOf(transform))// chekea q sea del bote
        {
            //position = col.transform.position;
            position = new Vector3(col.transform.position.x, -4.8f, 0);
        	var Particle = Instantiate(Trail, position, Quaternion.identity, col.transform);
            //Particle.transform.parent = col.transform;
        	var collider = Particle.collision;
        	collider.SetPlane(0, killer.transform);
        	Particle.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        body = col.GetComponent<Rigidbody2D>();
        if(col.transform.IsChildOf(transform) && (transform.childCount > 0))
        {
            //foreach (Transform child in col)
            Destroy(col.transform.GetChild(0).gameObject);
            //Destroy(col.transform.GetChild(1).gameObject);
        }
    }
}
