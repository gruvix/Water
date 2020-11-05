using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Waver : MonoBehaviour
{

//ESTE SCRIPT VA EN UN OBJETO
	//1) Cuando es clickeado EL PERSONAJE llama a esta funcion; el objeto se va a la cabeza y desactiva el brillo
	//2) Desde la cabeza click IZ se une a la balza, cambia el parent y activa el brillo
	//2.5) Click derecho lo devuelve al agua y desactiva el brillo
	//3) cuando se rompe el fixedjoint se cae al agua, cambia el parent y desactiva el brillo
    #pragma warning disable 0649// <- evita el warning de "null" en unity
    LineRenderer line;
    #pragma warning restore 0649
    private GameObject Floaters;
    private GameObject Bote;
    private GameObject SoulFragment;
    private ParticleSystem deathEffect;
    private LineRenderer LinePrefab;
    public bool hasJoint = false;
    public float hpMAX = 100;
    public float HP = 100;

    //Variables que definen el joint del cuerpo (ajustables en el PREFAB)
    public float Break_Force = 1;

    void Start()
    {
        Floaters = GameObject.Find("Floaters");
        Bote = GameObject.Find("Bote");
        SoulFragment = GameObject.Find("SoulFragment");
        LinePrefab = Resources.Load<LineRenderer>("Effects/MagicConnector");
        deathEffect = Resources.Load<ParticleSystem>("Effects/DestroyExplosion");
        if (gameObject.transform.parent.name == "Bote")
        {
            Adopcion(gameObject.transform);
        }

    }


    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano();
    }

    public void Huerfano()//Se va pa'l agua
    {
        if (line != null) 
        {
            Destroy(line.gameObject);
        }
        if (hasJoint)
        {
            try {
                Destroy(gameObject.GetComponent<FixedJoint2D>());
            }
            catch (Exception e)
            {
                print("Error en Huerfano: no existe joint");
            }
            hasJoint = false;
        }
    	gameObject.transform.SetParent(Floaters.transform);
    	gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);//Cambia parámetro del material
        
    	gameObject.layer = 9;
    }

    public void Transicion(GameObject owner)//Lo tiene el paisano
    {
    	JointCheck();
        if (line != null) 
        {
            Destroy(line.gameObject);
        }
        gameObject.transform.SetParent(owner.transform);
        gameObject.transform.position = owner.transform.position + new Vector3(0, 0.4f, 0);
        JointCheck();
        try
        {
            gameObject.GetComponent<FixedJoint2D>().connectedBody = owner.GetComponent<Rigidbody2D>();
        }
        catch (Exception e)
        {
            print("Error en Transicion: no existe joint");
        }
        gameObject.layer = 8;
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);
    }

    public void Adopcion(Transform target)//Ahora es del bote
    {
        gameObject.transform.position = target.position;
        gameObject.transform.rotation = target.rotation;
        JointCheck();
        try
        {
            gameObject.GetComponent<FixedJoint2D>().connectedBody = SoulFragment.GetComponent<Rigidbody2D>();
        }
        catch(Exception e)
        {
            print("Error en Adopcion: no existe joint");
        }
        gameObject.transform.SetParent(Bote.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        gameObject.layer = 10;
        MakeLine();

    }

    // public void Adopcion(Vector2 puntero)//Ahora es del bote
    // {
    // 	JointCheck();
        
    // 	gameObject.transform.position = new Vector3( puntero[0], puntero[1], 0);
    // 	gameObject.transform.SetParent(Bote.transform);
    // 	gameObject.GetComponent<FixedJoint2D>().connectedBody = SoulFragment.GetComponent<Rigidbody2D>();
    //     gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
    //     gameObject.layer = 10;
    //     MakeLine();

    // }

    public void MakeLine()//Aca se crea la linea magica
    {

        if (line != null) 
        {
            Destroy(line.gameObject);
        }
        line = Instantiate(LinePrefab, new Vector3(0,0,0), Quaternion.identity);
        line.widthMultiplier = 0.05f;
        line.transform.SetParent(GameObject.Find("EffectHolder").transform);
        var renderer = line.GetComponent<Renderer>();
        renderer.sortingOrder = -1;
    }


    public void JointCheck()//Se fija q haya un joint, si no lo hay lo crea
    {

        if (!hasJoint)
    	    {	
    	    var joint = gameObject.AddComponent<FixedJoint2D>();
    	    joint.breakForce = Break_Force;
    	    hasJoint = true;
    	    }
        else
        {
            gameObject.GetComponent<FixedJoint2D>().enabled = true;
        }

    }

    public void DestroyObject()//Cuando el objeto se destruye
    {
        if (line != null) 
            {
                Destroy(line.gameObject);
            }
        var death = Instantiate(deathEffect, gameObject.transform.position, Quaternion.identity, GameObject.Find("EffectHolder").transform);
        Destroy(death.gameObject, 5f);
        Destroy(gameObject);
    }

    public void Damage(float DMG)
    {
        HP -= DMG;
        Mathf.Clamp(HP, 0, hpMAX);
        gameObject.GetComponent<Renderer>().material.SetFloat("_Health", HP / hpMAX * 100);
    }

    void Update()//Mueve la linea de acuerdo al objeto
    {
        if (HP < 1)//Se destruye el objeto
        {
            DestroyObject();
        }

        if (line != null)
        {
            line.SetPosition(0, SoulFragment.transform.position);
            line.SetPosition(1, Vector3.Lerp(SoulFragment.transform.position, gameObject.transform.position, 0.33f));
            line.SetPosition(2, Vector3.Lerp(SoulFragment.transform.position, gameObject.transform.position, 0.66f));
            line.SetPosition(3, gameObject.transform.position);
        }
    }
}
