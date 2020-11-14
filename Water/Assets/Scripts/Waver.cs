using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Waver : NetworkBehaviour
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
    public float Break_Force = 500;
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
        fixedCheck = true;
    }


    [Client]
    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano();
        Debug.Log("Se rompio union por exeso de fuerza");
    }

    [Client]
    public void Huerfano()//Se va pa'l agua
    {
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        DestroyJoint();
        gameObject.transform.SetParent(Floaters.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);//Cambia parámetro del material

        gameObject.layer = 9;
    }


    [Client]
    public void Transicion(GameObject owner)//Lo tiene el paisano
    {
        CmdSetAuthority(gameObject.GetComponent<NetworkIdentity>(), owner.GetComponent<NetworkIdentity>());
        if (!hasAuthority) { return; }
        Debug.Log("tengo autoridad");
        Destroy(Fjoint);
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        if (line != null)
        {
            Destroy(line.gameObject);
        }

        gameObject.transform.SetParent(owner.transform);
        gameObject.GetComponent<NetworkTransformChild>().target = owner.transform;
        gameObject.transform.position = owner.transform.position + new Vector3(0, 2f, 0);


        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = false;
        }

        gameObject.layer = 11;
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);
    }

    [Client]
    public void Adopcion(Transform target)//Ahora es del bote
    {
        if (!hasAuthority) { return; }
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.transform.SetParent(Bote.transform);
        gameObject.GetComponent<NetworkTransformChild>().target = Bote.transform;
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        gameObject.layer = 10;

        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = true;
        }

        //gameObject.transform.SetPositionAndRotation(target.position, target.rotation);
        //StartCoroutine(HammerTime());
        CmdAdoptPosition(target);
              
        MakeLine();
    }

    [Command]
    public void CmdAdoptPosition(Transform target)
    {
        RpcAdoptPosition(target);
    }
    [ClientRpc]
    public void RpcAdoptPosition(Transform target)
    {
        Destroy(Fjoint);
        gameObject.transform.SetPositionAndRotation(target.position, target.rotation);
        StartCoroutine(HammerTime());
    }

    IEnumerator HammerTime()
    {
        yield return new WaitForSeconds(0.01f);
        fixedCheck = true;
    }

    [Client]
    public void MakeLine()//Aca se crea la linea magica
    {
        if (!hasAuthority) { return; }
        if (line != null)
        {
            Destroy(line.gameObject);
        }
        line = Instantiate(LinePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        line.widthMultiplier = 0.05f;
        line.transform.SetParent(GameObject.Find("EffectHolder").transform);
        var renderer = line.GetComponent<Renderer>();
        renderer.sortingOrder = -1;
    }

    [Client]
    public void JointCheck()//Se fija q haya un joint, si no lo hay lo crea
    {

        if (Fjoint == null)
        {
            if (gameObject.GetComponent<FixedJoint2D>() == null)
            {
                Fjoint = gameObject.AddComponent<FixedJoint2D>();
            }
            else
            {
                Fjoint = gameObject.GetComponent<FixedJoint2D>();
            }
            Fjoint.breakForce = Break_Force;
            Fjoint.dampingRatio = Damping_Ratio;
            hasJoint = true;
        }
        else
        {
            Fjoint.enabled = true;
            hasJoint = true;
        }

    }

    [Client]
    public void DestroyJoint()
    {
        try
        {
            Destroy(gameObject.GetComponent<FixedJoint2D>());
            Debug.Log(gameObject.GetComponent<FixedJoint2D>());
            Fjoint = null;
        }
        catch (Exception e) { Debug.Log("No Joint to destroy" + e); }
        hasJoint = false;
    }


    [Client]
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
        if (fixedCheck)
        {
            JointCheck();
            Fjoint.connectedBody = SoulFragment.GetComponent<Rigidbody2D>();
            fixedCheck = false;
        }
    }

    [Command]
    void CmdSetAuthority(NetworkIdentity grabID, NetworkIdentity playerID)
    {
        grabID.AssignClientAuthority(connectionToClient);
        Debug.Log("dando autoridad de " + grabID + " a " + playerID);
    }

}
