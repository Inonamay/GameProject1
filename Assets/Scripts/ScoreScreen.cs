using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    string[] scores;
    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text = "";
        FileStream file = File.Open(Application.persistentDataPath + "/ScoreSave.txt", FileMode.Open);
        StreamReader reader = new StreamReader(file);
        string savedScores = reader.ReadToEnd();
        scores = savedScores.Split('.');
        reader.Close();
        file.Close();
        UpdateScoreBoard();
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    public void BackToMenu()
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
            if (a == b && !highScore)
            {
                highscoreText.text += " High Score!";
                highScore = true;
            }
            highscoreText.text += "\n";
        }
        highscoreText.text += "\n" + "Your Score: " + scores[10];
    }
}
