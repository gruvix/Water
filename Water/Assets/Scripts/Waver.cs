﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waver : MonoBehaviour
{

//ESTE SCRIPT VA EN UN OBJETO
	//1) Cuando es clickeado EL PERSONAJE llama a esta funcion; el objeto se va a la cabeza, si es de la balza se cambia el material a natural
	//2) Desde la cabeza click IZ se une a la balza, cambia el parent y material
	//2.5) Click derecho lo devuelve al agua
	//3) cuando se rompe el fixedjoint se cae al agua, cambia el parent y material

    private GameObject Floaters;
    private GameObject Bote;
    public bool hasJoint = false;
    public float HP = 100;

    //Variables que definen el joint del cuerpo (ajustables en el PREFAB)
    public float Break_Force = 1;

    void Start()
    {
        Floaters = GameObject.Find("Floaters");
        Bote = GameObject.Find("Bote");
    }

    void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano();
        hasJoint = false;
        gameObject.transform.SetParent(Floaters.transform);
    	Floaters.GetComponentInParent<SetMaterial>().Resolve();
    	gameObject.layer = 9;
    }

    public void Huerfano()//Se va pa'l agua
    {
    	Destroy(GetComponent<FixedJoint2D>());
    	hasJoint = false;
    	gameObject.transform.SetParent(Floaters.transform);
    	Floaters.GetComponentInParent<SetMaterial>().Resolve();
    	gameObject.layer = 9;
    }

    public void Transicion(GameObject owner)//Lo tiene el paisano
    {
    	JointCheck();
        gameObject.transform.SetParent(owner.transform);
        gameObject.transform.position = owner.transform.position + new Vector3(0, 0.32f, 0);
        gameObject.GetComponent<FixedJoint2D>().connectedBody = owner.GetComponent<Rigidbody2D>();
        gameObject.layer = 8;
        owner.GetComponentInParent<SetMaterial>().Resolve();
    }

    public void Adopcion(Vector2 puntero)//Ahora es del bote
    {
    	JointCheck();
    	gameObject.transform.position = new Vector3( puntero[0], puntero[1], 0);
    	gameObject.transform.SetParent(Bote.transform);
    	gameObject.GetComponent<FixedJoint2D>().connectedBody = Bote.GetComponent<Rigidbody2D>();
    	gameObject.GetComponent<FixedJoint2D>().enabled = true;
		Bote.GetComponentInParent<SetMaterial>().Resolve();
        gameObject.layer = 10;
    }

    public void JointCheck()//Se fija q haya un joint, si no lo hay lo crea
    {
    if (hasJoint == false)
    	{	
    	var joint = gameObject.AddComponent<FixedJoint2D>();
    	joint.breakForce = Break_Force;
    	hasJoint = true;
    	}
    }
}
