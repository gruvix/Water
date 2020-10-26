using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
	public CharacterController2D controller;
	public float runSpeed = 40f;
	float horizontalMove = 0f;
    bool jump = false;
    private Animator animator;
    private Rigidbody2D body;

    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    	horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

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
