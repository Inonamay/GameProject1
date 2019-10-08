using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] Text highscoreText;
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
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void MenuScreen()
    {
        SceneManager.LoadScene(0);
    }
    public void ResetScore()
    {
        File.Delete(Application.persistentDataPath + "/ScoreSave.txt");
        scores = new string[11] { 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "", 0 + "" };
        UpdateScoreBoard();
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
