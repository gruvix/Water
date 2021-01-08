using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Mirror;

public class Floater : NetworkBehaviour
{

    LineRenderer line;
    private GameObject Floaters;
    private GameObject Bote;
    public GameObject Nucleo;
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
    public float Damping_Ratio = 10;

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();
		if (SceneManager.GetActiveScene().name == "Lobby") { return; }
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
             StartCoroutine(HammerTime());
             MakeLine();
         }

    }


    private void Start()
	{
        if (isServer && SceneManager.GetActiveScene().name != "Lobby")
        { 
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; 
            gameObject.GetComponent<SetDensity>().Set(); 
        }
        
        
    }

	[Client]
    public void OnJointBreak2D()//Mucha Violencia -> huerfanizado
    {
        Huerfano(0, transform);
        Debug.Log("Se rompio union por exeso de fuerza");
    }

    [ClientRpc]
    public void Huerfano(int dir, Transform player)//Se va pa'l agua
    {
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        
        if (line != null)
        {
            Destroy(line.gameObject);
        }
        if (isServer) { Destroy(Fjoint); }

        gameObject.transform.SetParent(Floaters.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 0);//Cambia parámetro del material
        gameObject.layer = 9;
        Vector2 speed = Mathf.Abs(dir) * (player.right * 6f * dir + transform.up * 3f);
        GetComponent<Rigidbody2D>().velocity = speed + player.GetComponent<Rigidbody2D>().velocity;
    }

    [ClientRpc]
    public void Transicion(GameObject owner)
	{
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        if (isServer) { Destroy(Fjoint); }

        gameObject.GetComponent<Rigidbody2D>().simulated = false;

        if (line != null)
        {
            Destroy(line.gameObject);
        }

        gameObject.transform.SetParent(owner.transform);
        gameObject.transform.position = owner.transform.position + new Vector3(0, 2f, 0);
        gameObject.transform.rotation = Quaternion.identity;


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

        if (isServer) { Destroy(Fjoint); }

        gameObject.transform.SetParent(Bote.transform);
        gameObject.GetComponent<Renderer>().material.SetInt("_Shine", 1);
        gameObject.layer = 10;

        if (gameObject.GetComponent<PlatformEffector2D>() != null)//Esto es para las plataformas
        {
            gameObject.GetComponent<PlatformEffector2D>().enabled = true;
        }
        
        gameObject.transform.SetPositionAndRotation(targetPos, targetRot);


        MakeLine();
        if (isServer) { StartCoroutine(HammerTime()); }
        else { gameObject.GetComponent<Rigidbody2D>().simulated = true; }
    }

    IEnumerator HammerTime()
    {
        yield return new WaitForSecondsRealtime(0.01f);
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

    [Server]
    public void MakeJoint()//Hace el joint
    {
        Fjoint = gameObject.AddComponent<FixedJoint2D>();
        Fjoint.connectedBody = Nucleo.GetComponent<Rigidbody2D>();
        Fjoint.dampingRatio = 100;
        Fjoint.frequency = 10000;
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
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
            Transform emitter = line.gameObject.transform.GetChild(0);
            emitter.rotation = Quaternion.Euler(new Vector3(0f, 0f, Vector2.SignedAngle(Nucleo.transform.position, gameObject.transform.position)+90));
            emitter.position = Nucleo.transform.position;
            var shape = emitter.GetComponent<ParticleSystem>().shape;
            float med = Vector2.Distance(Nucleo.transform.position, gameObject.transform.position)/2;
            shape.radius = med;
            shape.position = new Vector3(med, 0f, 0f);
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
