using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Floater : NetworkBehaviour
{
    public LineRenderer line;
    private GameObject Floaters;
    private GameObject Bote;
    private GameObject Nucleo;
    private ParticleSystem deathEffect;
    private LineRenderer LinePrefab;
    private bool fixedCheck = false;
    private bool makeLine = false;
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
            GetComponent<Rigidbody2D>().simulated = false;
            gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
            gameObject.layer = 10;

            if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
            {
                gameObject.GetComponent<PlatformEffector2D>().enabled = true;
            }
            StartCoroutine(HammerTime());

        }
    }

	[Client]
    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        CmdHuerfano(0, transform);
        Debug.Log("Se rompio union por exeso de fuerza");
    }

    [Command]
    public void CmdHuerfano(int dir, Transform player)//Se va pa'l agua
    {
        NetworkServer.UnSpawn(gameObject);
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        //gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        gameObject.GetComponent<SetDensity>().Set();
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        Destroy(Fjoint);
        //Fjoint.enabled = false;

        gameObject.transform.SetParent(Floaters.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);//Cambia parámetro del material
        gameObject.layer = 9;

        GetComponent<Rigidbody2D>().velocity =  player.right * 6f * dir + transform.up * 3f;
        NetworkServer.Spawn(gameObject);
    }

    [ClientRpc]
    public void RpcTransicion(GameObject owner)
	{

		Destroy(Fjoint);
        gameObject.GetComponent<Rigidbody2D>().simulated = false;

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


    [Server]
    public void Adopcion(Vector3 targetPos, Quaternion targetRot)//Ahora es del bote
    {
        //NetworkServer.UnSpawn(gameObject);
        Destroy(Fjoint);
        //Fjoint.enabled = false;

        gameObject.transform.SetParent(Bote.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        gameObject.layer = 10;

        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = true;
        }
        
        gameObject.transform.SetPositionAndRotation(targetPos, targetRot);
        StartCoroutine(HammerTime());
    }

    IEnumerator HammerTime()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        fixedCheck = true;
    }

    public void MakeLine()//Aca se crea la linea magica
    {
        if (line) { return; }
        line = Instantiate(LinePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        line.widthMultiplier = 1f;
        line.transform.SetParent(GameObject.Find("EffectHolder").transform);
        var renderer = line.GetComponent<Renderer>();
        renderer.sortingOrder = -1;
    }

    public void MakeJoint()//Hace el joint
    {
        if (!Fjoint) { Fjoint = gameObject.AddComponent<FixedJoint2D>(); }
        Fjoint.connectedBody = Nucleo.GetComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        //gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        gameObject.GetComponent<SetDensity>().Set();
        NetworkServer.Spawn(gameObject);
        makeLine = true;
        
    }


    [Command(ignoreAuthority = true)]
    public void CmdDestroy()
	{
        NetworkServer.Destroy(gameObject);
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

        if(makeLine)
		{
            makeLine = false;
            MakeLine();
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
