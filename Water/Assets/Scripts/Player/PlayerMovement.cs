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
    }

  	[Client]
    void FixedUpdate ()
    {
    	//Move Character
    	controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;

        if(Mathf.Abs(body.velocity.x) > 0.1f)
        {
        	animator.SetFloat("HorizontalSpeed", Mathf.Abs(body.velocity.x)*1.5f);
    	}
    	else
        {
            animator.SetFloat("HorizontalSpeed", 0);
        }
        if(Mathf.Abs(body.velocity.y) > .3f)//
        {
            animator.SetBool("OnAir", true);
        }
        else
        {
            animator.SetBool("OnAir", false);
        }
    }
}
