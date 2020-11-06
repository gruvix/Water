using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	public float speedLimit = 6f;
	public bool CanPlace = true;
	private int counter = 0;
	private Component copyCollider = null;
	private Component copyTrigger = null;
	private System.Type type = null;
    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (Mathf.Abs(collider.relativeVelocity.magnitude) > speedLimit && CanPlace)
        {
        	gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 0);
        	CanPlace = false;
        	gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    void OnTriggerEnter2D()
    {
    	counter++;
    }

    void OnTriggerExit2D()
    {

    	counter--;
    	if(counter == 0)
    	{
    		gameObject.GetComponent<Renderer>().material.SetInt("_CanPlace", 1);
    		CanPlace = true;
    		gameObject.GetComponent<Collider2D>().enabled = true;
    	}
    }

    void Update()
    {
    	Debug.Log(counter);
    }

    public void SetCollider(GameObject original)//Aca va cada tipo de collider
    {
		if(original.GetComponent<Collider2D>() != null)
 			{
 				CopyComponent(original.GetComponent<Collider2D>(), copyCollider);
 				CopyComponent(original.GetComponent<Collider2D>(), copyTrigger);
    			Collider2D[] colliders = GetComponents<Collider2D>();
    			for (int i = 0; i<colliders.Length; i++)
    			{
    				if (i==1)
    				{
    					colliders[i].isTrigger = true;
    				}
    			} 
 			}
    }

    public void DestroyCollider()
    {
    	Collider2D[] colliders = GetComponents<Collider2D>();
    	for (int i = 0; i<colliders.Length; i++)
    	{
    		Destroy(colliders[i]);
    	} 
    }

    public Component CopyComponent(Component original, Component copy)
 	{

    	type = original.GetType();
    	copy = gameObject.AddComponent(type);
    	// Copied fields can be restricted with BindingFlags
    	System.Reflection.FieldInfo[] fields = type.GetFields(); 
    	foreach (System.Reflection.FieldInfo field in fields)
    	{
    	   field.SetValue(copy, field.GetValue(original));
    	}

    	return copy;
 	}
}
