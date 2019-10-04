using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameController gc;
    //Har modifierat lite i move koden och ändrade maxSpeed, satte move i update - Max
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float speedModifier = .1f;
    int oneTimePlatformCheck = 0;
    int oneTimeMoveTileCheck = 0;
    [SerializeField] GameObject tile1;
    [SerializeField] GameObject tile2;
    [SerializeField] GameObject tile3;
    [SerializeField] GameObject tile4;
    [SerializeField] GameObject tile5;
    [SerializeField] GameObject tile6;
    int tileCycler = 1;
    private float moveTimer;
    private float accTimer = 0;
    GameObject town;
    GameObject forest;
    GameObject mountains;
    [SerializeField] private float accTimerWait = 30f;
    private float moveTimerWait;
    void Awake()
    {
        moveTimer = 0f;
        moveTimerWait = 1f;
    }
    private void Start()
    {
        if (gc == null)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
        }
        town = transform.GetChild(1).gameObject;
        forest = transform.GetChild(2).gameObject;
        mountains = transform.GetChild(0).gameObject;
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
            accTimer = 0;
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
        Vector3 distance = new Vector3(40.67f, 40.67f, 0);
        int posX = Mathf.RoundToInt(transform.position.x);
        if(town != null)
        {
            town.transform.position += Vector3.down * 0.0005f + Vector3.left * 0.0005f;
            if (town.transform.localPosition.x < -5)
            {
                forest.transform.position += Vector3.down * 0.0005f + Vector3.left * 0.0005f;
            }
        }
        if(forest == null || forest.transform.localPosition.x < -10)
        {
            if(town != null)
            {Destroy(town);}
            if(forest != null) { Destroy(forest); }
            mountains.transform.position += Vector3.down * 0.0001f + Vector3.left * 0.0001f;
        }
        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed, transform.position.y + Time.deltaTime * speed, transform.position.z);
        if (posX % 2 == 0 && oneTimePlatformCheck != posX)
        {
            gc.UpdatePlatforms();
            oneTimePlatformCheck = posX;
        }
        if(posX != 0 && posX != oneTimeMoveTileCheck && posX % 7 == 0)
        {
            tileCycler = tileCycler % 6;
            switch (tileCycler)
            {
                case 1: tile1.transform.position += distance; break;
                case 2: tile2.transform.position += distance; break;
                case 3: tile3.transform.position += distance; break;
                case 0: tile6.transform.position += distance; break;
                case 4: tile4.transform.position += distance; break;
                case 5: tile5.transform.position += distance; break;
            }
            tileCycler++;
            oneTimeMoveTileCheck = posX;
        }
    }
}
