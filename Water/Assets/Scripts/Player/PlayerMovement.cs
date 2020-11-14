using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
	public CharacterController2D controller;
	public float runSpeed = 40f;
	float horizontalMove = 0f;
    bool jump = false;
    bool antijump = false;
    private Animator animator;
    private Rigidbody2D body;
    public Camera cam;

    [Client]
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cam.GetComponent<CameraManager>().target = gameObject.transform;
        cam.enabled = true;
    }


  	[Client]
    void Update()
    {

    	if (!hasAuthority){return;}
    	horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            antijump = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            antijump = false;
        }
        
    }

  	[Client]
    void FixedUpdate ()
    {
        //Move Character
        if (hasAuthority) {
            CmdMoveSync(horizontalMove,jump,antijump);
            jump = false;
        }
    	
        if(Mathf.Abs(body.velocity.x) > 0.1f)
        {
        	animator.SetFloat("HorizontalSpeed", Mathf.Abs(body.velocity.x)*1.5f);
    	}
    	else
        {
            animator.SetFloat("HorizontalSpeed", 0);
        }
        if(Mathf.Abs(body.velocity.y) > 10f)//
        {
            animator.SetBool("OnAir", true);
        }
        else
        {
            animator.SetBool("OnAir", false);
        }
        

    }

    [Command]
    private void CmdMoveSync(float horizontalMove_, bool jump_, bool antijump_)
    {
        RcpMove(horizontalMove_, jump_, antijump_);
    }

    [ClientRpc]
    private void RcpMove(float horizontalMove_,bool jump_,bool antijump_)
    {
        controller.Move(horizontalMove_ * Time.fixedDeltaTime, false, jump_, antijump_);
        //transform.Translate(posicion);
    }
}