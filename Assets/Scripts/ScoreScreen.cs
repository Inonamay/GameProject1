using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text = "";
        FileStream file = File.Open(Application.persistentDataPath + "/ScoreSave.txt", FileMode.Open);
        StreamReader reader = new StreamReader(file);
        string savedScores = reader.ReadToEnd();
        string[] scores = savedScores.Split('.');
        for (int i = 0; i < scores.Length - 1; i++)
        {
            highscoreText.text += (i + 1) + ". " + scores[i] + "\n";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
