using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Floater : NetworkBehaviour
{
    //ESTE SCRIPT VA EN UN OBJETO
    //1) Cuando es clickeado EL PERSONAJE llama a esta funcion; el objeto se va a la cabeza y desactiva el brillo
    //2) Desde la cabeza click IZ se une a la balza, cambia el parent y activa el brillo
    //2.5) Click derecho lo devuelve al agua y desactiva el brillo
    //3) cuando se rompe el fixedjoint se cae al agua, cambia el parent y desactiva el brillo
    //#pragma warning disable 0649// <- evita el warning de "null" en unity
    LineRenderer line;
    private GameObject Floaters;
    private GameObject Bote;
    private GameObject Nucleo;
    private ParticleSystem deathEffect;
    private LineRenderer LinePrefab;
    private bool fixedCheck = false;
    public FixedJoint2D Fjoint;
    public bool hasJoint = false;
    public float hpMAX = 100;
    [SyncVar]
    public float HP = 100;

    //Variables que definen el joint del cuerpo (ajustables en el PREFAB)
    public float Break_Force = 50;
    public float Damping_Ratio = 1;

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();
        Floaters = GameObject.Find("Floaters");
        Bote = GameObject.Find("Bote");
        Nucleo = GameObject.Find("Nucleo");
        LinePrefab = Resources.Load<LineRenderer>("Effects/MagicConnector");
        deathEffect = Resources.Load<ParticleSystem>("Effects/DestroyExplosion");
        if (gameObject.transform.parent.name == "Bote")//Hace una adopcion como personalizada
        {
            gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
            gameObject.layer = 10;

            if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
            {
                gameObject.GetComponent<PlatformEffector2D>().enabled = true;
            }
            gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            StartCoroutine(HammerTime(Bote));
            MakeLine();
        }
    }

    [Client]
    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano(0, transform);
        Debug.Log("Se rompio union por exeso de fuerza");
    }

    //no estoy seguro de como implementar esto en red. Tal ves trabajar sobre la destruccion y el spawn
    [ClientRpc]
    public void Huerfano(int dir, Transform player)//Se va pa'l agua
    {
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        Destroy(Fjoint);
        gameObject.transform.SetParent(Floaters.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);//Cambia parámetro del material
        gameObject.layer = 9;

        GetComponent<Rigidbody2D>().velocity =  player.right * 6f * dir + transform.up * 3f;
    }

    [ClientRpc]
    public void Transicion(GameObject owner)
	{
        if (line != null)
        {
            Destroy(line.gameObject);
        }
        Destroy(Fjoint);
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        gameObject.transform.SetParent(owner.transform);
        gameObject.transform.position = owner.transform.position + new Vector3(0, 2f, 0);


        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = false;
        }

        gameObject.layer = 11;
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);
    }


    [ClientRpc]
    public void Adopcion(Vector3 targetPos, Quaternion targetRot)//Ahora es del bote
    {
        Destroy(Fjoint);
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.transform.SetParent(Bote.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        gameObject.layer = 10;

        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = true;
        }
        gameObject.transform.SetPositionAndRotation(targetPos, targetRot);
        StartCoroutine(HammerTime(Bote));
        MakeLine();
    }

    IEnumerator HammerTime(GameObject bote)
    {
        yield return new WaitForSeconds(0.01f);
        fixedCheck = true;
    }

    [Client]
    public void MakeLine()//Aca se crea la linea magica
    {
        line = Instantiate(LinePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        line.widthMultiplier = 1f;
        line.transform.SetParent(GameObject.Find("EffectHolder").transform);
        var renderer = line.GetComponent<Renderer>();
        renderer.sortingOrder = -1;
    }

    public void MakeJoint()//Se fija q haya un joint, si no lo hay lo crea
    {
        Fjoint = gameObject.AddComponent<FixedJoint2D>();
        Fjoint.connectedBody = Nucleo.GetComponent<Rigidbody2D>();
    }

    [Command(ignoreAuthority = true)]
    public void CmdDestroy()
	{
        Debug.Log("CMD destroy");
        //RpcDestroyObject();
        Destroy(gameObject);
	}


    [ClientRpc]
    public void RpcDestroyObject()//Cuando el objeto se destruye
    {
        Debug.Log("rpc destroy");
        Destroy(gameObject);
        if (line != null)
        {
            Destroy(line.gameObject);
        }
    }

    public void Damage(float DMG)
    {
        CmdDamage(DMG);
    }

    [Command(ignoreAuthority = true)]
    private void CmdDamage(float DMG)
	{
        HP -= DMG;
        HP = Mathf.Clamp(HP, 0, hpMAX);
        RpcDamage();
        if (HP == 0)//Se destruye el objeto
        {
            Debug.Log("0 hp detected");
            CmdDestroy();//No funciona
        }
    }

    [ClientRpc]
    private void RpcDamage()
	{
        gameObject.GetComponent<Renderer>().material.SetFloat("_Health", HP / hpMAX * 100);
    }

    void Update()//Mueve la linea de acuerdo al objeto
    {
        if (line != null)
        {
            line.SetPosition(0, Nucleo.transform.position);
            line.SetPosition(1, gameObject.transform.position);
            var shape = line.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().shape;
            var z = Vector2.Angle(Nucleo.transform.position, gameObject.transform.position);
            shape.rotation.Set(0, 0, z);
        }
    }

    void FixedUpdate()
    {
        if (fixedCheck)
        {
            fixedCheck = false;
            MakeJoint();
            
        }
    }

	private void OnDisable()
	{
        if (line != null)
        {
            Destroy(line.gameObject);
        }
    }

	private void OnDestroy()
	{
        if (line != null)
        {
            Destroy(line.gameObject);
        }
    }
}
