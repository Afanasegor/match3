using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameConroller : MonoBehaviour
{
    [SerializeField] private Text turnsText;
    [SerializeField] private Text scoresText;
    [SerializeField] private Text losePanelScoresText;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject uIPanels;
    [SerializeField] private Board board;

    private int Turns { get; set; }
    private int Scores { get; set; }
    private int indexOfScore = 1;
    private int minRecord;
    
    private void Start()
    {
        minRecord = 0;
        if (!PlayerPrefs.HasKey("MinRecord"))
        {
            PlayerPrefs.SetInt("MinRecord", minRecord);
        }
        minRecord = PlayerPrefs.GetInt("MinRecord");

        Debug.Log(PlayerPrefs.GetInt("MinRecord") + "&" + minRecord);

        Turns = 3;
        Scores = 0;
        turnsText.text = Turns.ToString();
        scoresText.text = Scores.ToString();
    }

    public void PlusTurns(int number)
    {
        Turns += number;
        turnsText.text = Turns.ToString();
    }

    public void MinusTurns()
    {
        Turns--;
        turnsText.text = Turns.ToString();
        CheckLose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="number">количество уничтоженных объектов</param>
    public void ScoreCount(int number)
    {
        int _plusScore = number * indexOfScore;
        Scores += _plusScore;
        scoresText.text = Scores.ToString();
    }

    private void CheckLose()
    {
        if (Turns == 0)
        {
            StartCoroutine(Lose());
        }
    }    
    
    IEnumerator Lose()
    {
        board.isLose = true;
        if (Scores > minRecord)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Переход на другую сцену");
            PlayerPrefs.SetInt("NewRecord", Scores);
            PlayerPrefs.Save();
            SceneManager.LoadScene(2);
        }
        else
        {
            losePanel.SetActive(true);
            losePanelScoresText.text = scoresText.text;
            uIPanels.SetActive(false);
            yield return new WaitForSeconds(0.7f);
            Debug.Log("U have lost!");
        }
    }

    public void Pause()
    {
        board.isPaused = true;
        Time.timeScale = 0;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        board.isPaused = false;
    }

    public int ReturnIndexOfComplexity()
    {
        if (Scores < 20)
            return 10;
        else if (Scores < 50)
            return 9;
        else if (Scores < 70)
            return 8;
        else if (Scores < 90)
            return 7;
        else if (Scores < 110)
            return 6;
        else if (Scores < 130)
            return 5;
        else if (Scores < 150)
            return 4;
        else if (Scores < 170)
            return 3;
        else if (Scores < 190)
            return 2;
        else if (Scores < 210)
            return 1;
        else
            return 0;
    }
}
