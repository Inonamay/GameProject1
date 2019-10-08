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
    int direction = 1;
    float lastFramePosY;
    float posY;
    bool falling = false;
    bool hasReleasedSpace;
    public int maxHealth;

    float timer = 0;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (moveDirection != 0)
        {
            animator.SetBool("Moving", true);

            if (moveDirection < 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            transform.localScale = Vector3.up + Vector3.forward + Vector3.right * direction;
        }
        else { animator.SetBool("Moving", false); }
    }
    private void FixedUpdate()
    {
        animator.SetFloat("Shoot", 0);
        posY = transform.position.y;

        PlayerInput();
        if (!isGrounded && Input.GetAxisRaw("Jump") != 1)
        {
            rb.AddForce(Vector3.down * 10);
        }
        if (Input.GetAxisRaw("Jump") == 0)
        {
            hasReleasedSpace = true;
        }
        if (posY < lastFramePosY)
        {
            animator.SetBool("Falling", true);
            animator.SetBool("Jump", false);
            animator.SetBool("Grounded", false);
            falling = true;
        }
        Collider2D col = Physics2D.OverlapCircle(transform.position + Vector3.down * 0.35f, 0.1f, mask);
        if (falling && col != null)
        {
            isGrounded = true;
            animator.SetBool("Falling", false);
            animator.SetBool("Grounded", true);
            falling = false;
            if(col.gameObject.name != "GroundeTile_001")
            {
                transform.position = Vector3.right * transform.position.x + Vector3.up * (col.transform.position.y + 0.25f);
            }
           
        }
        if (Input.GetAxisRaw("Fire1") != 0)
        {
            animator.SetFloat("Shoot", 1f);
            animator.SetBool("DoneShooting", true);
        }
        else
        {
            animator.SetFloat("Shoot", 0f);
            animator.SetBool("DoneShooting", false);
        }
        lastFramePosY = posY;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameController gc = FindObjectOfType<GameController>();
        if (collision.gameObject.tag == "Checkpoint")
        {
           
            gc.SetCheckPoint(collision.transform.position);

        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameController gc = FindObjectOfType<GameController>();
        if (collision.gameObject.tag == "Enemy")
        {
            gc.GameOver();
        }
    }
    private void PlayerInput()
    {

        moveDirection = Input.GetAxisRaw("Horizontal");

        transform.position += Vector3.right * moveDirection * Time.deltaTime * speed;
        if (Input.GetAxisRaw("Jump") == 1 && isGrounded && hasReleasedSpace)
        {
            Jump();
        }


    }

    private void Jump()
    {
        animator.SetBool("Jump", true);
        animator.SetBool("Grounded", false);

        rb.AddForce(Vector3.up * jumpForce);


        //rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.y);
        isGrounded = false;
        hasReleasedSpace = false;
    }
    public void HealPlayer(int healAmount)
    {
        hp += healAmount;

        if(hp > maxHealth)
        {
            hp = maxHealth;
        }
    }
    public int Hp
    {
        get
        {
           
            return hp;
        }
        set
        {
            hp = value;
        }
    }

    public override void Damaged(int damage)
    {
        hp -= damage;
    }
}
