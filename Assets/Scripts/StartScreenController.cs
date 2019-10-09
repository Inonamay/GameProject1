using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    [SerializeField] Sprite gameOver;
    [SerializeField] Sprite victory;
    string[] scores;
    void Start()
    {
        Cursor.visible = true;
        highscoreText.text = "";
        if (File.Exists(Application.persistentDataPath + "/ScoreSave.txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/ScoreSave.txt", FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string savedScores = reader.ReadToEnd();
            scores = savedScores.Split('.');
            reader.Close();
            file.Close();
            UpdateScoreBoard();
        }
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            if(PlayerPrefs.GetInt("Victory") == 0)
            {
                transform.GetChild(0).GetComponent<Image>().sprite = gameOver;
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().sprite = victory;
            }
            
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void MenuScreen()
    {
        SceneManager.LoadScene(0);
    }
    public void ScoreScreen()
    {
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
        Application.Quit();
    }
    void UpdateScoreBoard()
    {
        int a;
        int b;
        bool highScore = false;
        System.Int32.TryParse(scores[10], out b);
        for (int i = 0; i < 10; i++)
        {
            System.Int32.TryParse(scores[i], out a);
            highscoreText.text += (i + 1) + ". " + scores[i];
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) && a != 0 && a == b && !highScore)
            {
                highscoreText.text += " Top " + (i+1) + "!" ;
                highScore = true;
            }
            highscoreText.text += "\n";
        }
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            highscoreText.text += "\n" + "Latest Score: " + scores[10];
        }
    }
}
