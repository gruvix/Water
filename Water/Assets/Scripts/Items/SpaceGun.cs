using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpaceGun : NetworkBehaviour
{
    public AudioSource wepAudio;
    Transform firePointNormal, firePointFlip, firePoint;
    Vector3 mouse_pos;
    Vector3 object_pos;
    Transform target; //Desde quien apunta
    float angle;
    float radians;
    public GameObject bulletPrefab;
    public float playerHeight = 0.1f;//Altura a la q va el arma
    public float offset = 0.05f; //Que tan lejos está el arma del personaje
    public float compensation = -38f;//compensa que la punta del arma no está en la base del sprite
    private bool canshoot=true;
    public float cooldowntime=1f;

    void Start()
    {
        firePointNormal = this.gameObject.transform.GetChild(0);
        firePointFlip = this.gameObject.transform.GetChild(1);
        wepAudio = GetComponent<AudioSource>();
    }


    public void SetItem(Transform owner)
    {
        target = owner;
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    [Command]
    void CmdShoot(Vector3 pos, Quaternion rot)
	{
        GameObject bullet = Instantiate(bulletPrefab, pos, rot * Quaternion.Euler(0, 0, 270));
        NetworkServer.Spawn(bullet);
        RpcSpriteShoot();
    }

    [ClientRpc]
    void RpcSpriteShoot()
	{
        wepAudio.Play();
    }

    [Command]
    private void CmdUpdateWeapon(bool bul, Transform fp)
	{
        RpcUpdateWeapon(bul, fp);
	}

    [ClientRpc]
    private void RpcUpdateWeapon(bool bul, Transform fp)
	{
        gameObject.GetComponent<SpriteRenderer>().flipY = bul;
        firePoint = fp;
    }

    void Update()
    {
        if (!hasAuthority) { return; }
        if (target != null)
        {
     		mouse_pos = Input.mousePosition;
     		mouse_pos.z = 10; //The distance between the camera and object
     		object_pos = Camera.main.WorldToScreenPoint(target.position) + new Vector3(0, playerHeight, 0);
     		mouse_pos.x = mouse_pos.x - object_pos.x;
     		mouse_pos.y = mouse_pos.y - object_pos.y + compensation;
     		radians = Mathf.Atan2(mouse_pos.y, mouse_pos.x);
    		angle = radians * Mathf.Rad2Deg;
    		if (angle > 90 || angle < -90)
    		{
    			gameObject.GetComponent<SpriteRenderer>().flipY = true;
                firePoint = firePointFlip;
                CmdUpdateWeapon(true, firePointFlip);
    		}
    		else
    		{
    			gameObject.GetComponent<SpriteRenderer>().flipY = false;
                firePoint = firePointNormal;
                CmdUpdateWeapon(false, firePointNormal);
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        	gameObject.transform.position = target.position + new Vector3(Mathf.Cos(radians) * offset, Mathf.Sin(radians) * offset + playerHeight, 0) ;

        }
        if (Input.GetMouseButton(2))
        {
            //hacer que las balas se sincronicen con un ienumerator
            if (canshoot)
            {
                CmdShoot(firePoint.position, firePoint.rotation);
                canshoot = false;
                StartCoroutine(Cooldown(cooldowntime));
            }
        }
    }

    IEnumerator Cooldown(float cooltime)
    {

        yield return new WaitForSeconds(cooltime);
        canshoot = true;
        Debug.Log("Can shoot");
    }
}
