using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class NetWaver : NetworkBehaviour
{
    //ESTE SCRIPT VA EN UN OBJETO
    //1) Cuando es clickeado EL PERSONAJE llama a esta funcion; el objeto se va a la cabeza y desactiva el brillo
    //2) Desde la cabeza click IZ se une a la balza, cambia el parent y activa el brillo
    //2.5) Click derecho lo devuelve al agua y desactiva el brillo
    //3) cuando se rompe el fixedjoint se cae al agua, cambia el parent y desactiva el brillo
    //#pragma warning disable 0649// <- evita el warning de "null" en unity
    LineRenderer line;
    #pragma warning restore 0649
    private GameObject Floaters;
    private GameObject Bote;
    private GameObject SoulFragment;
    private ParticleSystem deathEffect;
    private LineRenderer LinePrefab;
    private bool fixedCheck = false;
    public FixedJoint2D Fjoint;
    public bool hasJoint = false;
    public float hpMAX = 100;
    public float HP = 100;

    //Variables que definen el joint del cuerpo (ajustables en el PREFAB)
    public float Break_Force = 50;
    public float Damping_Ratio = 1;

    [Client]
    private void Start()
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
        Debug.Log("llego a Start");
    }
    [Client]
    public override void OnStartClient()
    {
        Start();
        Debug.Log("llego a OnStartClient()");
    }
    
    [Client]
    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano();
        Debug.Log("Se rompio union por exeso de fuerza");
    }

    //no estoy seguro de como implementar esto en red. Tal ves trabajar sobre la destruccion y el spawn
    [Client]
    public void Huerfano()//Se va pa'l agua
    {

    }

    [Client]
    public void Adopcion(Transform target)//Ahora es del bote
    {

    }

    [Client]
    public void MakeLine()//Aca se crea la linea magica
    {

    }

    [Client]
    public void JointCheck()//Se fija q haya un joint, si no lo hay lo crea
    {

    }

    [Client]
    public void DestroyJoint()
    {

    }


    [Client]
    public void DestroyObject()//Cuando el objeto se destruye
    {

    }


    [Client]
    public void Damage(float DMG)
    {
        HP -= DMG;
        Mathf.Clamp(HP, 0, hpMAX);
        gameObject.GetComponent<Renderer>().material.SetFloat("_Health", HP / hpMAX * 100);
    }

    [Client]
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

    [Client]
    void FixedUpdate()
    {

    }
}
