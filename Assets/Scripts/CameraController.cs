using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameController gc;
    //Har modifierat lite i move koden och ändrade maxSpeed, satte move i update - Max
    private float speed;
    private float maxSpeed;
    private float speedModifier;


    private float moveTimer;
    private float accTimer;

    private float accTimerWait;
    private float moveTimerWait;

    private bool updated;
    void Awake()
    {
        moveTimer = 0f;
        moveTimerWait = 1f;

        accTimer = 0f;
        accTimerWait = 5f;

        speed = 1f;
        maxSpeed = 100f;
        speedModifier = .25f;
    }
    private void Start()
    {
        if (gc == null)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
        }

    }


    void Update()
    {
        //moveTimer += Time.deltaTime * speed;
        accTimer += Time.deltaTime;
        //if (moveTimer >= moveTimerWait)
        //{
        //    moveTimer -= moveTimerWait;
        //    Move();
        if (accTimer >= accTimerWait)
        {
            accTimer -= accTimerWait;
            if (speed < maxSpeed)
            {
                speed += speedModifier;
            }
        }

        //}
        Move();

    }

    void Move()
    {
        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed, transform.position.y + Time.deltaTime * speed, transform.position.z);
        if(Mathf.RoundToInt(transform.position.x) % 2 == 0 && updated)
        {
            gc.UpdatePlatforms();
            updated = false;
        }
        if(Mathf.RoundToInt(transform.position.x) % 2 == 1)
        {
            updated = true;
        }
    }
}
