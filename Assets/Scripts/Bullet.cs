using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    public Rigidbody2D rb;
    public float timer = 0;
    public float direction;

    public void Start()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        direction = -pc.Direction;
        GetComponent<Rigidbody2D>();
        transform.localScale = new Vector3(direction, 1, 1);
    }
    public void Update()

    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            Destroy(gameObject);
        }
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyController>().Damaged(1);
            Destroy(gameObject);

        }
    }
}  

