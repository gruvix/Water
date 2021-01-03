using System.Collections;
using UnityEngine;
using Mirror;

public class Cannon : NetworkBehaviour
{
    public AudioSource wepAudio;
    Transform firePoint;
    Vector3 mouse_pos;
    Vector3 object_pos;
    Transform target; //Desde quien apunta
    float angle;
    float radians;
    public GameObject bulletPrefab;
    public float playerHeight = 0.1f;//Altura a la q va el arma
    public float offset = 0.05f; //Que tan lejos está el arma del personaje
    public float compensation = -38f;//compensa que la punta del arma no está en la base del sprite
    private bool canshoot = true;
    public float cooldowntime = 1f;

    void Start()
    {
        firePoint = transform.GetChild(0).GetChild(0);
        wepAudio = GetComponent<AudioSource>();
    }


    public void SetItem()
    {
        target = GetComponent<ItemHandler>().owner;
        //gameObject.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;//AVISO siendo que el cañon va a ser estático esto no se deberia deshabilitar
        transform.GetChild(0).GetComponent<PolygonCollider2D>().enabled = false;
        angle = 60;
    }

    public void UnSetItem()
	{
        target = null;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        transform.GetChild(0).GetComponent<PolygonCollider2D>().enabled = true;
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
        gameObject.GetComponent<SpriteRenderer>().flipX = bul;
        firePoint = fp;
    }

    void Update()
    {
        if (!hasAuthority) { return; }
        if (target != null)
        {
            var prevAngle = angle;
            mouse_pos = Input.mousePosition;
            mouse_pos.z = 10; //The distance between the camera and object
            object_pos = Camera.main.WorldToScreenPoint(target.position) + new Vector3(0, playerHeight, 0);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y + compensation;
            radians = Mathf.Atan2(mouse_pos.y, mouse_pos.x);
            angle = Mathf.Clamp(radians * Mathf.Rad2Deg, -20, 70);
            if (angle > prevAngle + 1)
            {
                angle = prevAngle + 1;
            }
            else if (angle < prevAngle - 1)
            {
                angle = prevAngle - 1;
                gameObject.GetComponent<SpriteRenderer>().flipY = false;
                CmdUpdateWeapon(false, firePoint);
            }


            transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Clamp(angle, -30, 70)+180));
            gameObject.transform.position = target.position + new Vector3(Mathf.Cos(radians) * offset, Mathf.Sin(radians) * offset + playerHeight, 0);

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
