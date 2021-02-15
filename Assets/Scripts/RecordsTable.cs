using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

public class RecordsTable : MonoBehaviour
{
    [SerializeField] private Transform table;
    [SerializeField] private Transform row;
    private int capacity = 7;
    private List<Transform> highscoreEntryTransformList;
    private int minRecord;
    Highscores highscores;

    private int newRecordScore;
    private string newRecordDate;

    private void Awake()
    {       
        row.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Первая инициализация таблицы
        if (highscores == null)
        {            
            Debug.Log("Initializing table with default values...");

            DownloadTable(highscores);            

            // Перезагрузка Prefs
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }
        
        SortingListByScore(highscores);

        if (PlayerPrefs.HasKey("NewRecord"))
        {
            AddNewRecord(PlayerPrefs.GetInt("NewRecord"));
            PlayerPrefs.DeleteKey("NewRecord");
        }

        jsonString = PlayerPrefs.GetString("highscoreTable");
        highscores = JsonUtility.FromJson<Highscores>(jsonString);
        
        SortingListByScore(highscores);

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, table, highscoreEntryTransformList);
        }

        minRecord = highscores.highscoreEntryList[highscores.highscoreEntryList.Count - 1].score;
        PlayerPrefs.SetInt("MinRecord", minRecord);
    }

    // Создание физической таблицы на сцене
    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform _table, List<Transform> transformList)
    {
        float rowHeight = 200f;

        Transform entryTransform = Instantiate(this.row, _table);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -rowHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int number = transformList.Count + 1;
        entryTransform.Find("NumberText").GetComponent<Text>().text = number.ToString();

        string date = highscoreEntry.date;
        entryTransform.Find("DateText").GetComponent<Text>().text = date;

        int score = highscoreEntry.score;
        entryTransform.Find("ScoresText").GetComponent<Text>().text = score.ToString();

        if (date == newRecordDate && score == newRecordScore)
        {
            entryTransform.Find("NumberText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("DateText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("ScoresText").GetComponent<Text>().color = Color.green;

            newRecordDate = "";
            newRecordScore = 0;
        }

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string date)
    {
        // Создание Highscore
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, date = date };

        // сохранение Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }
        
        // Добавление рекорда в таблицу
        if (highscores.highscoreEntryList.Count < capacity)
        {
            highscores.highscoreEntryList.Add(highscoreEntry);
        }
        else if (highscores.highscoreEntryList[capacity - 1] != null && highscoreEntry.score > highscores.highscoreEntryList[capacity - 1].score)
        {            
            highscores.highscoreEntryList.RemoveAt(capacity-1);
            highscores.highscoreEntryList.Add(highscoreEntry);
        }

        // сохранение обновленной таблицы рекордов
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    // TODO: Позже удалить
    public void DeleteKeyHighScore()
    {
        PlayerPrefs.DeleteKey("highscoreTable");
        PlayerPrefs.DeleteKey("MinRecord");
        PlayerPrefs.DeleteKey("NewRecord");
    }
    
    // Добавление новго рекорда с присвоением даты
    public void AddNewRecord(int score)
    {
        newRecordScore = score;
        newRecordDate = DateTime.Now.ToShortDateString();
        AddHighscoreEntry(newRecordScore, newRecordDate);
    }

    private void SortingListByScore(Highscores highscores)
    {
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // сортировка
                    HighscoreEntry copy = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = copy;
                }
            }
        }

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    // Загрузка таблицы через CSV файл
    private void DownloadTable(Highscores highscores)
    {
        string recordsData = LoadCSV();

        string[] data = recordsData.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ';' });

            if (row[0] != "")
            {
                int _score;
                string _date;

                int.TryParse(row[2], out _score);
                _date = DateTime.Parse(row[1]).ToShortDateString();

                AddHighscoreEntry(_score, _date);
            }
        }
    }

    // Возможность вытащить csv файл как с андроида, так и с ПК...
    private string LoadCSV()
    {
        string sFilePath = Path.Combine(Application.streamingAssetsPath, "Records" + ".csv");
        string recordsDataCopy;
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(sFilePath);
            www.SendWebRequest();
            while (!www.isDone) ;
            recordsDataCopy = www.downloadHandler.text;
        }
        else
        {
            recordsDataCopy = Resources.Load<TextAsset>("Records").text;
        }
        return recordsDataCopy;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string date;
    }    
}