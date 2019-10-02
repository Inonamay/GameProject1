using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected float speed = 10f;
    [SerializeField]
    protected int hp;
    protected float moveDirection = 1f;

    virtual protected void Move()
    {   

    }

    virtual protected void Shoot()
    {

    }

    virtual protected void Death()
    {

    }
}
