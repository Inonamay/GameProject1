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
    public Vector3 dir;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    virtual public void Move()
    {   

    }

    virtual public void Shoot()
    {

    }

    virtual protected void Death()
    {

    }

    virtual public void Damaged(int damage)
    {

    }
}
