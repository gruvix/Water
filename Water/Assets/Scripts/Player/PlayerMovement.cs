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

        animator.SetFloat("HorizontalSpeed", Mathf.Abs(body.velocity.x));
        if(Mathf.Abs(body.velocity.y) > 0)
        {
            animator.SetBool("OnAir", true);
        }
        else
        {
            animator.SetBool("OnAir", false);
        }
    }
}
