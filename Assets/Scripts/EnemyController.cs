using System;
using System.Collections;
using UnityEngine;
public class EnemyController : Character
{
#pragma warning disable 0649

    Rigidbody2D rigidBody;
    PolygonCollider2D polygonCollider;

    [SerializeField] GameObject target;

    PlayerController targetScript;
    GameController gc;


    private enum CurrentState { Idle, MovingLeft, MovingRight, Attack }

    [SerializeField] private LayerMask PlatformLayerMask;
    private LayerMask PlayerLayerMask;

    private CurrentState currentState;
    private CurrentState prevState;

    private float idleTimer;
    [SerializeField] private float idleTimerTick;


    private bool dead;
    private float attackTimer;
    private float attackTimerCooldown;

    [SerializeField] private int scoreValue;

    private int spriteFlipper;

#pragma warning restore 0649
    private void Awake()
    {
        base.Start();

        rigidBody = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        dead = false;

        currentState = CurrentState.MovingRight;
        prevState = CurrentState.MovingRight;

        idleTimer = 0f;
        hp = 1;
        spriteFlipper = 1;

        attackTimer = attackTimerCooldown;
        attackTimerCooldown = 1f;

        PlayerLayerMask = LayerMask.GetMask(LayerMask.LayerToName(target.layer));
        targetScript = target.GetComponent<PlayerController>();


    }
    protected override void Start()
    {
        if (gc == null)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
        }

    }
    private void Update()
    {
        if (!dead)
        {
            moveDirection = 0;
            Move();
        }

    }


    public override void Move()
    {
        //Wacky shit ya'll

        if (CheckAttackDistance() && !animator.GetBool("Attack"))
        {
            Attack();
        }


        if (animator.GetBool("Attack"))
        {
            attackTimer += Time.deltaTime;
            animator.SetBool("Attack", true);
            currentState = CurrentState.Attack;

            if (attackTimer >= attackTimerCooldown)
            {
                if (CheckAttackDistance())
                {
                    targetScript.Damaged(1);
                }
                attackTimer -= attackTimerCooldown;
                animator.SetBool("Attack", false);
            }
        }

        else
        {

            if (!animator.GetBool("Moving"))
            {
                prevState = currentState;
                currentState = CurrentState.Idle;

                idleTimer += Time.deltaTime;
                animator.SetBool("Moving", false);
                if (idleTimer >= idleTimerTick)
                {
                    animator.SetBool("Moving", true);
                    idleTimer -= idleTimerTick;
                }


            }

            else
            {
                RaycastHit2D walkRight = Physics2D.Raycast(transform.position, new Vector2(.5f, -1f), .5f, PlatformLayerMask);
                RaycastHit2D walkLeft = Physics2D.Raycast(transform.position, new Vector2(-.5f, -1f), .5f, PlatformLayerMask);

                //Check for platform end
                if (currentState != CurrentState.Idle && ((walkLeft.collider != null && walkRight.collider == null) || (walkRight.collider != null && walkLeft.collider == null)))
                {
                    animator.SetBool("Moving", false);
                }
                else
                {
                    if (walkRight.collider != null && prevState != CurrentState.MovingLeft)
                    {
                        currentState = CurrentState.MovingRight;
                        moveDirection = 1;
                        spriteFlipper = -1;
                    }

                    if (walkLeft.collider != null && prevState != CurrentState.MovingRight)
                    {
                        currentState = CurrentState.MovingLeft;
                        moveDirection = -1;
                        spriteFlipper = 1;
                    }
                    animator.SetBool("Moving", true);
                }
                prevState = currentState;

            }
        }
        transform.position += Vector3.right * Time.deltaTime * speed * moveDirection;
        transform.localScale = new Vector3(spriteFlipper, 1, 1);
    }



    protected override void Death()
    {
        gc.Score += scoreValue;
        dead = true;
        StartCoroutine(Die());
    }

    public override void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Death();
        }
    }

    private bool CheckAttackDistance()
    {

        if (currentState == CurrentState.MovingRight || (currentState == CurrentState.Attack && prevState == CurrentState.MovingRight))
        {
            RaycastHit2D attackR = Physics2D.Raycast(transform.position, Vector2.right, .4f, PlayerLayerMask);
            if (attackR.collider != null)
            {
                return true;
            }

        }

        else if (currentState == CurrentState.MovingLeft || (currentState == CurrentState.Attack && prevState == CurrentState.MovingLeft))
        {
            RaycastHit2D attackL = Physics2D.Raycast(transform.position, Vector2.left, .4f, PlayerLayerMask);
            if (attackL.collider != null)
            {
                return true;
            }

        }
        return false;
    }

    private void Attack()
    {
        moveDirection = 0;
        currentState = CurrentState.Attack;

        animator.SetBool("Attack", true);

    }

    IEnumerator Die()
    {
        polygonCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        animator.SetBool("Moving", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Dead", true);


        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
