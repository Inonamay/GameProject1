using System;
using UnityEngine;
public class EnemyController : Character
{
    Rigidbody2D rigidBody;
    PolygonCollider2D polygonCollider;
    CircleCollider2D circleCollider;

    GameController gc;

    private enum CurrentState { Idle, MovingLeft, MovingRight, Attack }

    [SerializeField] private LayerMask PlatformLayerMask;
    [SerializeField] private LayerMask PlayerLayerMask;


    [SerializeField] private int scoreValue;


    private CurrentState currentState;
    private CurrentState prevState;
    private float timer;
    [SerializeField] private float timerTick;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();


        currentState = CurrentState.MovingRight;
        prevState = CurrentState.MovingLeft;

        timer = 0f;
        scoreValue = 100;
        hp = 3;

    }
    protected override void Start()
    {
        base.Start();
        animator.SetBool("Moving", true);
        if (gc == null)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
        }
    }
    private void Update()
    {
        Move();
        CheckAttackDistance();
    }


    protected override void Move()
    {
        //Wacky shit ya'll
        if (!animator.GetBool("Moving"))
        {
            prevState = currentState;
            currentState = CurrentState.Idle;

            timer += Time.deltaTime;
            animator.SetBool("Moving", false);
            if (timer >= timerTick)
            {
                animator.SetBool("Moving", true);
                timer -= timerTick;
            }
        }
        else
        {
            RaycastHit2D walkRight = Physics2D.Raycast(transform.position, new Vector2(.5f, -1f), .5f, PlatformLayerMask);
            RaycastHit2D walkLeft = Physics2D.Raycast(transform.position, new Vector2(-.5f, -1f), .5f, PlatformLayerMask);

            //Check for platform end
            if (prevState != CurrentState.Idle && ((walkLeft.collider != null && walkRight.collider == null) || (walkRight.collider != null && walkLeft.collider == null)))
            {
                prevState = currentState;
                animator.SetBool("Moving", false);

            }
            else
            {
                if (walkRight.collider != null && prevState != CurrentState.MovingLeft)
                {
                    currentState = CurrentState.MovingRight;
                    animator.SetBool("Moving", true);
                    moveDirection = 1;

                }

                if (walkLeft.collider != null && prevState != CurrentState.MovingRight)
                {

                    currentState = CurrentState.MovingLeft;
                    animator.SetBool("Moving", true);
                    moveDirection = -1;
                }

                prevState = currentState;
                transform.position += Vector3.right * Time.deltaTime * speed * moveDirection;

                transform.localScale = new Vector3(-moveDirection, 1, 1);

            }


        }
    }

    protected override void Death()
    {
        gc.Score += scoreValue;
        Destroy(gameObject);
    }

    public override void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Death();
        }
    }

    private void CheckAttackDistance()
    {
        if (currentState == CurrentState.MovingRight)
        {
            RaycastHit2D attackR = Physics2D.Raycast(transform.position, Vector2.right, .5f, PlayerLayerMask);
            if (attackR.collider != null)
            {
                Debug.Log("Player!!");
            }

            Debug.DrawRay(transform.position, Vector3.right * .5f, Color.red);
        }

        else if (currentState == CurrentState.MovingLeft)
        {
            RaycastHit2D attackL = Physics2D.Raycast(transform.position, Vector2.left, .5f, PlayerLayerMask);


            Debug.DrawRay(transform.position, Vector3.left * .5f, Color.red);
            if (attackL.collider != null)
            {
                Debug.Log("Player!!");
            }

        }

    }

    private void Attack()
    {

    }
}
