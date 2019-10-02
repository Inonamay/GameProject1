using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField] float timeToStart = 5;
    [SerializeField] GameObject enemyPrefab;
    PlayerController pc;
    #region Platform Variables
    [SerializeField] GameObject platformPrefab;
    float timer;
    float offsetX;
    float offsetY;
    GameObject platform;
    List<GameObject> activePlatforms = new List<GameObject>();
    #endregion
    #region Score Variables
    float scoreTimer;
    int score = 0;
    [SerializeField] Text scoreText;
    [SerializeField] int enemySpawnChance = 10;
    float scoreMultiplier = 1;
    [SerializeField] float multiplierTimer = 10.0f;
    [SerializeField] float scoreTick = 1;
    int counter = 0;
    #endregion
    // Start is called before the first frame update
    private void Awake()
    {
        timer = timeToStart;
        gameObject.tag = "GameController";
    }
    private void Start()
    {
        Camera.main.GetComponent<CameraController>().enabled = false;
        offsetX = Camera.main.orthographicSize * Camera.main.aspect;
        pc = FindObjectOfType<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if(pc.Hp < 1)
        {
            GameOver();
        }
        if(timer < 0)
        {
            if (!Camera.main.GetComponent<CameraController>().enabled)
            {Camera.main.GetComponent<CameraController>().enabled = true;}
            if (scoreTimer > scoreTick)
            {CalcScore();}
            scoreTimer += Time.deltaTime;
        }
        else
        {timer -= Time.deltaTime; }
    }
    void CalcScore()
    {
        scoreTimer = 0;
        counter++;
        Score ++;
        scoreText.text = "Score: " + score;
        if (counter % multiplierTimer == 0)
        { scoreMultiplier += 0.5f; }
    }
    void StartGame()
    {
        SceneManager.LoadScene(0);
    }
    void GameOver()
    {
        ScoreSave();
        SceneManager.LoadScene(1);
    }
    void GetToStartScreen()
    {
        SceneManager.LoadScene(2);
    }
    void ScoreSave()
    {
        FileStream file;
        StreamWriter writer;
        if (File.Exists(Application.persistentDataPath + "/ScoreSave.txt"))
        {
            file = File.Open(Application.persistentDataPath + "/ScoreSave.txt", FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string savedScores = reader.ReadToEnd();
            reader.Close();
            savedScores.Replace("Your Score!", "");
            string[] scores = savedScores.Split('.');
            int a = 0;
            for (int i = 11; i > 0; i++)
            {
                System.Int32.TryParse(scores[i], out a);
                if (a < score)
                {
                    scores[i] = score + " Your Score!";
                    break;
                }
            }
            file.Close();
            File.Delete(Application.persistentDataPath + "/ScoreSave.txt");
            file = File.Create(Application.persistentDataPath + "/ScoreSave.txt");
            writer = new StreamWriter(file);
            for (int i = 0; i < scores.Length; i++)
            { writer.Write("." + scores[i]);}
            
        }
        else
        {
            file = File.Create(Application.persistentDataPath + "/ScoreSave.txt");
            writer = new StreamWriter(file);
            for (int i = 0; i < 10; i++)
            {writer.Write(score + "."); }
            
        }
        writer.Close();
        file.Close();
    }
    void GeneratePlatforms()
    {
        for (int i = 0; i < 3; i++)
        {
            offsetY = Random.Range(Camera.main.orthographicSize * 0.5f, Camera.main.orthographicSize) + 4;
            platform = Instantiate(platformPrefab);
            platform.transform.position = new Vector3(offsetX + Camera.main.transform.position.x - (i * 4), offsetY + Camera.main.transform.position.y, 0);
            activePlatforms.Add(platform);
            if (Random.Range(0, 101) < enemySpawnChance)
            {
                GameObject temp = Instantiate(enemyPrefab);
                temp.transform.position = platform.transform.position + Vector3.up;
            }
        }
    }
    void CheckPlatformsForDelete()
    {
        GameObject[] platforms = activePlatforms.ToArray();
        for (int i = 0; i < platforms.Length; i++)
        {
            if(platforms[i] != null)
            {
                if (Mathf.Abs(platforms[i].transform.position.magnitude - Camera.main.transform.position.magnitude) > Camera.main.orthographicSize * 2.5f)
                {Destroy(platforms[i]);}
            }
        }
    }
    public void UpdatePlatforms()
    {
        CheckPlatformsForDelete();
        GeneratePlatforms();
    }
    public int Score
    {
        get
        {return score;}
        set
        {
            if(value > 0)
            {score = Mathf.RoundToInt(value * scoreMultiplier);}
        }
    }
}
