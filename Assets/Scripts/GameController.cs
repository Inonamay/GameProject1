using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
#pragma warning disable 0414
#pragma warning disable 0649
    #region Enemy Variables
    [SerializeField] GameObject enemyPrefab;
    List<GameObject> enemies = new List<GameObject>();
    GameObject enemyParent;
    #endregion
    #region Platform Variables
    [SerializeField] GameObject[] platformPrefabs;
    GameObject platformParent;
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
#pragma warning restore 0649
#pragma warning restore 0414
    #region Player Variables
    PlayerController pc;
    float playerMagnitudeY;
    float playerMagnitudeX;
    #endregion
    bool start = false;
    FileStream file;
    int savedScore;
    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.tag = "GameController";
        if (platformParent == null)
        {
            platformParent = new GameObject("Platforms");
            GameObject[] platformsactive = GameObject.FindGameObjectsWithTag("Platform");
            for (int i = 0; i < platformsactive.Length; i++)
            {
                platformsactive[i].transform.parent = platformParent.transform;
                activePlatforms.Add(platformsactive[i]);
            }
        }
        if (enemyParent == null)
        {
            enemyParent = new GameObject("Enemies");
        }
    }
    private void Start()
    {
        Camera.main.GetComponent<CameraController>().enabled = false;
        offsetX = Camera.main.orthographicSize * Camera.main.aspect;
        offsetY = Camera.main.orthographicSize;
        pc = FindObjectOfType<PlayerController>();
        scoreText.text = "Score: 0";
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        playerMagnitudeX = -(pc.gameObject.transform.position.x - Camera.main.transform.position.x);
        playerMagnitudeY = -(pc.gameObject.transform.position.y - Camera.main.transform.position.y);
        if (playerMagnitudeY > Camera.main.orthographicSize || playerMagnitudeX > Camera.main.orthographicSize * Camera.main.aspect)
        { GameOver(); }
        if (pc.Hp < 1)
        { GameOver(); }
        if(!start && pc.gameObject.transform.position.x > Camera.main.transform.position.x + (offsetX * 0.25f) && pc.gameObject.transform.position.y > Camera.main.transform.position.y + (offsetY * 0.25f))
        {start = true;}
        if (start)
        {
            if (!Camera.main.GetComponent<CameraController>().enabled)
            { Camera.main.GetComponent<CameraController>().enabled = true; }
            if (scoreTimer > scoreTick)
            { CalcScore(); }
            scoreTimer += Time.deltaTime;
        }
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
    void GameOver()
    {
        if(pc.Hp < 1)
        {
            ScoreSave();
            try
            {
                SceneManager.LoadScene(2);
            }
            catch
            {
                SceneManager.LoadScene("MainScene");
            }
        }
        else
        {
            score = savedScore;
            start = false;
            Camera.main.GetComponent<CameraController>().enabled = false;
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
            for (int i = 0; i < platforms.Length; i++)
            {
                Destroy(platforms[i]);
            }
            platform = Instantiate(platformPrefabs[0]);
            platform.transform.position = Camera.main.transform.position + Vector3.forward;
            activePlatforms.Add(platform);
            platform.transform.parent = platformParent.transform;
            pc.gameObject.transform.position = Camera.main.transform.position + Vector3.up + Vector3.forward;
            pc.Hp -= 1;
            for (int i = 0; i < 4; i++)
            {
                platform = Instantiate(platformPrefabs[0]);
                platform.transform.position = Vector3.right * (Camera.main.transform.position.x + (i * 2.5f) + 2.5f) + Vector3.up * ((Camera.main.transform.position.y + (i * 1.25f) + 1.25f));
                activePlatforms.Add(platform);
            }
        }
       
       
    }
    void ScoreSave()
    {
        string[] scores;
        if (File.Exists(Application.persistentDataPath + "/ScoreSave.txt"))
        {
            scores = UpdateExistingScore();
            File.Delete(Application.persistentDataPath + "/ScoreSave.txt");
            UpdateSaveFile(scores);
        }
        else
        {
            scores = new string[10] {score + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "" };
            UpdateSaveFile(scores);
        }
    }
    void UpdateSaveFile(string[] scores)
    {
        StreamWriter writer;
        file = File.Create(Application.persistentDataPath + "/ScoreSave.txt");
        writer = new StreamWriter(file);
        for (int i = 0; i < 10; i++)
        { writer.Write(scores[i] + "."); }
        writer.Write(score);
        writer.Close();
        file.Close();
    }
    string[] UpdateExistingScore()
    {
        file = File.Open(Application.persistentDataPath + "/ScoreSave.txt", FileMode.Open);
        StreamReader reader = new StreamReader(file);
        string savedScores = reader.ReadToEnd();
        reader.Close();
        savedScores.Trim();
        string[] scores = savedScores.Split('.');
        int a = 0;
        bool foundplace = false;
        for (int i = 0; i < scores.Length - 1; i++)
        {
            System.Int32.TryParse(scores[i], out a);
            if (a < score && !foundplace)
            {
                int b = i + 1;
                string nextstring = scores[i];
                string savedstring;
                while (b != scores.Length - 1)
                {
                    savedstring = scores[b];
                    scores[b] = nextstring;
                    nextstring = savedstring;
                    b++;
                }
                scores[i] = score + "";
                foundplace = true;
            }
        }
        file.Close();
        return scores;
    }
    void GeneratePlatforms()
    {
        
        for (int i = 0; i < 5; i++)
        {
            CreatePlatform(i);
            if (Random.Range(0, 101) < enemySpawnChance)
            { CreateEnemy();}
        }
    }
    void CreatePlatform(int offset)
    {
        
        float posX = offsetX + Camera.main.transform.position.x - (offset * Random.Range(0.5f, 1.5f));
        float posY = offsetY + Camera.main.transform.position.y + offset + 0.25f;
        platform = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Length)]);
        platform.transform.position = new Vector3(posX, posY, 0);
        activePlatforms.Add(platform);
        platform.transform.parent = platformParent.transform;
    }
    void CreateEnemy()
    {
        GameObject temp = Instantiate(enemyPrefab);
        temp.transform.position = platform.transform.position + Vector3.up + Vector3.back;
        enemies.Add(temp);
        temp.transform.parent = enemyParent.transform;
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
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] != null)
            {
                if (Mathf.Abs(enemies[i].transform.position.magnitude - Camera.main.transform.position.magnitude) > Camera.main.orthographicSize * 2.5f)
                { Destroy(enemies[i]); }
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
            {score = value;}
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SetCheckPoint()
    {

    }

}
