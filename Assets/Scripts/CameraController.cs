using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameController gc;
    #region Speed Variables
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float speedModifier = .1f;
    private float accTimer = 0;
    #endregion
    int oneTimePlatformCheck = 0;
    int oneTimeMoveTileCheck = 0;
#pragma warning disable 0649
    #region Mountain Tiles & variables
    [SerializeField] GameObject tile1;
    [SerializeField] GameObject tile2;
    [SerializeField] GameObject tile3;
    [SerializeField] GameObject tile4;
    [SerializeField] GameObject tile5;
    [SerializeField] GameObject tile6;
    int tileCycler = 1;
    #endregion
#pragma warning restore 0649
    
    GameObject town;
    GameObject forest;
    GameObject mountains;
    [SerializeField] private float accTimerWait = 30f;
    Vector3[] startPosMountain;
    void Awake()
    {
        startPosMountain = new Vector3[6] {tile1.transform.position, tile2.transform.position, tile3.transform.position, tile4.transform.position, tile5.transform.position, tile6.transform.position };
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
        accTimer += Time.deltaTime;
        if (accTimer >= accTimerWait)
        {
            accTimer = 0;
            if (speed < maxSpeed)
            {
                speed += speedModifier;
            }
        }
        Move();
    }
    void Move()
    {
        Vector3 distance = new Vector3(40.49f, 40.49f, 0);
        int posX = Mathf.RoundToInt(transform.position.x);
        if(town != null)
        {
            town.transform.position += Vector3.down * 0.0005f + Vector3.left * 0.0005f;
            if (town.transform.localPosition.x < -5)
            {
                forest.transform.position += Vector3.down * 0.0005f + Vector3.left * 0.0005f;
            }
        }
        if(forest == null || forest.transform.localPosition.x < -5)
        {
            if(town != null)
            {Destroy(town);}
            if(forest != null) { Destroy(forest); }
            mountains.transform.position += Vector3.down * 0.002f + Vector3.left * 0.002f;
        }
        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed, transform.position.y + Time.deltaTime * speed, transform.position.z);
        if (posX % 2 == 0 && oneTimePlatformCheck != posX)
        {
            gc.GeneratePlatforms();
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
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            if(value > 0)
            {
                speed = value;
            }
        }
    }
    public void ResetMountain()
    {
        tileCycler = 1;
        tile1.transform.position = startPosMountain[0];
        tile2.transform.position = startPosMountain[1];
        tile3.transform.position = startPosMountain[2];
        tile4.transform.position = startPosMountain[3];
        tile5.transform.position = startPosMountain[4];
        tile6.transform.position = startPosMountain[5];
    }
}
