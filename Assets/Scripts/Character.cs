using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected float speed = 10f;
    [SerializeField]
    protected int hp =1;
    protected float moveDirection = 1f;
    protected Animator animator;
    public int damageAmount = 1;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    virtual protected void Move()
    {   

    }

    virtual protected void Shoot()
    {

    }

    virtual protected void Death()
    {

    }

    virtual public void Damaged(int damage)
    {

    }
}
