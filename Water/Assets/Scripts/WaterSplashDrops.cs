using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplashDrops : MonoBehaviour
{

	public ParticleSystem Drops;


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != "WaterBox")
        {
            Debug.Log("Splash Detected");
        	var Particle = Instantiate(Drops, col.transform.position, transform.rotation);
        	Particle.enableEmission = true;
        	Particle.Play();
        	Destroy(Particle.gameObject, 5f);
        }
    }


/*
    public void OnTriggerEnter()
    {
        var effect = Instantiate(Drops, tra)
    }*/
}
