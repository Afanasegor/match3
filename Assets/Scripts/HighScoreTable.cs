//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class HighScoreTable : MonoBehaviour
//{
//    [SerializeField] private Transform table;
//    [SerializeField] private Transform row;
//    private List<Transform> highscoreEntryTransformList;

//    private void Awake()
//    {
//        row.gameObject.SetActive(false);
        
//        /*
//        highscoreEntryList = new List<HighscoreEntry>()
//        {
//            new HighscoreEntry{ score = 1234, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 329, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 57, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 444, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 937, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 675, date = DateTime.Now.ToShortDateString()},
//            new HighscoreEntry{ score = 999, date = DateTime.Now.ToShortDateString()}
//        };*/

//        /*
//        string jsonString = PlayerPrefs.GetString("highscoreTable");
//        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);*/


//        for (int i = 0; i < highscoreEntryList.Count; i++)
//        {
//            for (int j = i+1; j < highscoreEntryList.Count; j++)
//            {
//                if (highscoreEntryList[j].score > highscoreEntryList[i].score)
//                {
//                    // swap
//                    HighscoreEntry copy = highscoreEntryList[i];
//                    highscoreEntryList[i] = highscoreEntryList[j];
//                    highscoreEntryList[j] = copy;
//                }
//            }
//        }

//        highscoreEntryTransformList = new List<Transform>();
//        foreach (HighscoreEntry item in highscoreEntryList)
//        {
//            CreateHighscoreEntry(item, table, highscoreEntryTransformList);
//        }
        

//        Highscores highscores = new Highscores {highscoreEntryList = highscoreEntryList };
//        string json = JsonUtility.ToJson(highscores);
//        PlayerPrefs.SetString("highscoreTable", json);
//        PlayerPrefs.Save();
//        Debug.Log(PlayerPrefs.GetString("highscoreTable"));
//    }

//    private void CreateHighscoreEntry(HighscoreEntry highscoreEntry, Transform table, List<Transform> transformList)
//    {
//        float rowHeight = 200f;

//        Transform entryTransform = Instantiate(row, table);
//        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
//        entryRectTransform.anchoredPosition = new Vector2(0, -rowHeight * transformList.Count);
//        entryTransform.gameObject.SetActive(true);

//        int number = transformList.Count + 1;
//        entryTransform.Find("NumberText").GetComponent<Text>().text = number.ToString();


//        string date = highscoreEntry.date;
//        entryTransform.Find("DateText").GetComponent<Text>().text = date;

//        int score = highscoreEntry.score;
//        entryTransform.Find("ScoresText").GetComponent<Text>().text = score.ToString();

//        transformList.Add(entryTransform);
//    }

//    private class Highscores
//    {
//        public List<HighscoreEntry> highscoreEntryList;
//    }

//    [System.Serializable]
//    private class HighscoreEntry
//    {
//        public int score;
//        public string date;
//    }
//}
