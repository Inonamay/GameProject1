using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    [SerializeField] private float jumpForce = 0f;
    private bool isGrounded;
    Rigidbody2D rb;
    private Vector3 jump;
    [SerializeField] private LayerMask mask;

    private void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        if (Physics2D.OverlapCircle(transform.position, 0.5f, mask))
        {
            isGrounded = true;
        }
        PlayerInput();
        Jump();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hp = 0;
    }


    private void PlayerInput()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");


        transform.position += new Vector3(moveDirection * Time.deltaTime * speed, 0);
    }

    private void Jump()
    { 

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.y);
            isGrounded = false;
        }
    }

    public int Hp
    {
        get
        {
            //health
            return hp;//health
        }
        set
        {

        }
    }
}
