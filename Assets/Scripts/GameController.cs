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
    bool start = false;
    FileStream file;
    float savedSpeed;
    [SerializeField] GameObject checkPoint;
    Vector3 checkpointPos;
    #region Enemy Variables
    [SerializeField] GameObject enemyPrefab;
    List<GameObject> enemies = new List<GameObject>();
    GameObject enemyParent;
    #endregion
    #region Platform Variables
    [SerializeField] GameObject[] platformPrefabs;
    GameObject platformParent;
    GameObject platform;
    List<GameObject> activePlatforms = new List<GameObject>();
    List<GameObject> nonActivePlatforms = new List<GameObject>();
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
    int savedScore;
    int highestScore = 0;
    float savedMultiplier;
    #endregion
#pragma warning restore 0649
#pragma warning restore 0414
    #region Player Variables
    PlayerController pc;
    float playerMagnitudeY;
    float playerMagnitudeX;
    #endregion
    #region Camera Variables
    float cameraSizeX;
    float cameraSizeY;
    CameraController cc;
    #endregion
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
        savedMultiplier = scoreMultiplier;
    }
    private void Start()
    {
        cc = Camera.main.GetComponent<CameraController>();
        cc.enabled = false;
        cameraSizeX = Camera.main.orthographicSize * Camera.main.aspect;
        cameraSizeY = Camera.main.orthographicSize;
        pc = FindObjectOfType<PlayerController>();
        scoreText.text = "Score: 0";
        Cursor.visible = false;
    }
    void Update()
    {
        playerMagnitudeX = -(pc.gameObject.transform.position.x - Camera.main.transform.position.x);
        playerMagnitudeY = -(pc.gameObject.transform.position.y - Camera.main.transform.position.y);
        
        if(!start && pc.gameObject.transform.position.x > Camera.main.transform.position.x + (cameraSizeX * 0.25f) && pc.gameObject.transform.position.y > Camera.main.transform.position.y + (cameraSizeY * 0.25f))
        {start = true;}
        if (start)
        {
            if (!cc.enabled)
            { cc.enabled = true; }
            if (scoreTimer > scoreTick)
            { CalcScore(); }
            scoreTimer += Time.deltaTime;
        }
        if (playerMagnitudeY > cameraSizeY || playerMagnitudeX > cameraSizeX)
        {
            if (!start)
            {
                pc.transform.position = Camera.main.transform.position + Vector3.forward + Vector3.up;
            }
            else
            {
                GameOver();
            }

        }
    }
    void CalcScore()
    {
        scoreTimer = 0;
        counter++;
        Score ++;
        if (counter % multiplierTimer == 0)
        {
            scoreMultiplier += 0.5f;
            counter = 0;
        }
        UpdateScoreText();
    }
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
        if (highestScore != 0)
        {
            scoreText.text += " Highest Reached Score: " + highestScore;
        }
        scoreText.text += " Multiplier: " + scoreMultiplier + "x";
    }
    public void GameOver()
    {
        pc.Hp--;
        if(pc.Hp < 1)
        {
            score = highestScore;
            ScoreSave();
            SceneManager.LoadScene(2);
            
        }
        else
        {
            if (score > highestScore)
            {highestScore = score;}
            score = savedScore;
            cc.Speed = savedSpeed;
            start = false;
            cc.enabled = false;
            scoreMultiplier = savedMultiplier;
            CheckPlatformsForDelete();
            for (int i = 0; i < nonActivePlatforms.Count; i++)
            {
                nonActivePlatforms[i].transform.position = Vector3.down * 20;
            }
            DestroyEntities("Checkpoint");
            DestroyEntities("Enemy");
            if (savedScore == 0)
            {
                activePlatforms.Clear();
                nonActivePlatforms.Clear();
                nonActivePlatforms.AddRange(GameObject.FindGameObjectsWithTag("Platform"));
                for (int i = 0; i < nonActivePlatforms.Count; i++)
                {
                    nonActivePlatforms[i].transform.position = Vector3.down * 20;
                }
                ResetMap();
                GenerateStart();
            }
            else
            {
               pc.gameObject.transform.position = Camera.main.transform.position + Vector3.up + Vector3.forward;
               nonActivePlatforms[0].transform.position = pc.transform.position + Vector3.down;
            }
            GameObject check = Instantiate(checkPoint);
            check.transform.position = pc.transform.position;
        }
    }
    void DestroyEntities(string tag)
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < entities.Length; i++)
        { Destroy(entities[i]); }
    }
    void ResetMap()
    {
        Camera.main.transform.position = Vector3.zero + Vector3.back * 10;
        pc.gameObject.transform.position = Vector3.left * 4.8f + Vector3.down * 1.8f;
        platform = Instantiate(platformPrefabs[0]);
        platform.transform.position = Vector3.left + Vector3.down;
        cc.ResetMountain();
    }
    void GenerateStart()
    {
        platform = Instantiate(platformPrefabs[0]);
        platform.transform.position = Camera.main.transform.position + Vector3.forward;
        activePlatforms.Add(platform);
        platform.transform.parent = platformParent.transform;

        for (int i = 0; i < 4; i++)
        {
            if(nonActivePlatforms.Count > 0)
            {
                platform = nonActivePlatforms[0];
                nonActivePlatforms.RemoveAt(0);
            }
            else
            {
                platform = Instantiate(platformPrefabs[0]);
            }
            
            platform.transform.position = Vector3.right * (Camera.main.transform.position.x + (i * 2.5f) + 2.5f) + Vector3.up * ((Camera.main.transform.position.y + (i * 1.25f) + 1.25f));
            activePlatforms.Add(platform);
        }
        UpdateScoreText();
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
    public void GeneratePlatforms()
    {
        for (int i = 0; i < 5; i++)
        {
            CreatePlatform(i);
            if (Random.Range(0, 101) < enemySpawnChance)
            { CreateEnemy();}
            if (i == 2 && Mathf.RoundToInt(Camera.main.transform.position.x) % 25 == 0)
            {
                GameObject temp = Instantiate(checkPoint);
                temp.transform.position = platform.transform.position + Vector3.up;
            }
        }
        CheckPlatformsForDelete();
    }
    void CreatePlatform(int offset)
    {
        float posX = cameraSizeX + Camera.main.transform.position.x - (offset * Random.Range(0.5f, 1.5f));
        float posY = cameraSizeY + Camera.main.transform.position.y + offset + 0.25f;
        if(nonActivePlatforms.Count > 0)
        {
            int platformPrefab = Random.Range(0, nonActivePlatforms.Count - 1);
            platform = nonActivePlatforms[platformPrefab];
            nonActivePlatforms.RemoveAt(platformPrefab);
        }
        else
        {
            platform = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Length)]);
        }
        platform.transform.position = new Vector3(posX, posY, 0);
        activePlatforms.Add(platform);
        platform.transform.parent = platformParent.transform;
    }
    void CreateEnemy()
    {
        GameObject temp = Instantiate(enemyPrefab);
        temp.transform.position = platform.transform.position + Vector3.up;
        enemies.Add(temp);
        temp.transform.parent = enemyParent.transform;
    }
    void CheckPlatformsForDelete()
    {
        GameObject[] platforms = activePlatforms.ToArray();
        float magnitude;
        for (int i = 0; i < platforms.Length; i++)
        {
            if(platforms[i] != null)
            {
                magnitude = Mathf.Abs(platforms[i].transform.position.magnitude - Camera.main.transform.position.magnitude);
                if (magnitude > cameraSizeY * 2.5f)
                {
                    activePlatforms.Remove(platforms[i]);
                    nonActivePlatforms.Add(platforms[i]);
                }
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] != null)
            {
                magnitude = Mathf.Abs(enemies[i].transform.position.magnitude - Camera.main.transform.position.magnitude);
                if (magnitude > cameraSizeY * 2.5f)
                { Destroy(enemies[i]); }
            }
        }
        GameObject[] checks = GameObject.FindGameObjectsWithTag("Checkpoint");
        for (int i = 0; i < checks.Length; i++)
        {
            if (checks[i] != null)
            {
                magnitude = Mathf.Abs(checks[i].transform.position.magnitude - Camera.main.transform.position.magnitude);
                if (magnitude > cameraSizeY * 2.5f)
                { Destroy(checks[i]); }
            }
        }
    }
    public int Score
    {
        get
        {return score;}
        set
        {
            if(value > 0)
            { score += Mathf.RoundToInt((value - score) * scoreMultiplier); }
        }
    }
    public void SetCheckPoint(Vector3 pos)
    {
        checkpointPos = pos;
        savedScore = score;
        savedSpeed = cc.Speed;
        savedMultiplier = scoreMultiplier;
    }
}
