using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [Header("Размеры поля")]
    [SerializeField] private int width; 
    [SerializeField] private int height; 
    [Space]
    [SerializeField] private GameObject objPrefab; //префаб объекта 
    [SerializeField] private Color[] color; //возможные цвета объекта
    [SerializeField] private GameObject explosion;    
       
    private ObjProps[,] allObj; // массив объектов, класс "ObjProps", чтобы сразу обращаться к свойствам объекта
    private List<ObjProps> destroyObj; // коллекция объектов, которые необходимо удалить
    private bool isChecking = false; // булевая переменная, для обозначения процесса проверки линий
    private AudioSource explosionSound;
    

    public GameConroller gameConroller;
    public bool isLose = false;
    public bool isPaused = false;
    public bool isFirstTurn = true;
    public GameObject handPrefab;
    public Vector3 targetForHand;
    public Text text; // убрать.. (нужно было для теста)

    private void Start()
    {
        SetUp();
        CheckFirstTurn();
        handPrefab = Instantiate(handPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
    }

    /// <summary>
    /// Первоначальная инициализация объектов при старте игры
    /// </summary>
    private void SetUp()
    {
        allObj = new ObjProps[width, height];
        destroyObj = new List<ObjProps>();
        explosionSound = GetComponent<AudioSource>();

        Vector3 _position; // = Vector3.zero;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _position = new Vector3(x, y);

                GameObject obj = Instantiate(objPrefab, _position, Quaternion.identity, transform);
                obj.name = "(" + x + "," + y + ")";

                allObj[x, y] = obj.GetComponent<ObjProps>();
                InitObject(allObj[x,y]);
            }
        }        
    }    
    
    /// <summary>
    /// Метод, удаляющий (перемещающий) необходимые объекты
    /// </summary>
    /// <param name="_destroyObj"></param>
    public void DeleteObject(List<ObjProps> _destroyObj)
    {
        foreach (var obj in _destroyObj)
        {
            obj.gameObject.SetActive(false);
            PlayExplosionSound();
            GameObject _explosion = Instantiate(explosion, obj.transform.position, Quaternion.identity);
            Move(obj, obj.X, obj.Y);
            StartCoroutine(DestroyObj(obj, _explosion));            
            isChecking = false;
        }
        destroyObj.Clear();
    }

    /// <summary>
    /// Корутина, которая делает задержку при появлении нового объекта
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator DestroyObj(ObjProps obj, GameObject _explosion)
    {        
        InitObject(obj);
        yield return new WaitForSeconds(0.5f);
        Destroy(_explosion);
        obj.gameObject.SetActive(true);
    }    

    /// <summary>
    /// Метод, меняющий фактическое местоположение объектов
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void Move(ObjProps obj, int x, int y)
    {
        for (int i = y; i + 1 < height; i++)
        {
            allObj[x, i + 1].Y = i;
            allObj[x, i + 1].targetPosition = new Vector2(x, i);
            allObj[x, i + 1].isMoving = true;
        }
        allObj[x, y].Y = height-1;
        allObj[x, y].targetPosition = new Vector2(x, height - 1);
        obj.gameObject.transform.position = new Vector3(x, height-1);
        SwippingInArray(x, y);
    }

    /// <summary>
    /// Метод, меняющий местами логическое расположение объектов в массиве
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void SwippingInArray(int x, int y)
    {
        for (int i = y; i + 1 < height; i++)
        {
            ObjProps copyObj = allObj[x, i];
            allObj[x, i] = allObj[x, i + 1];
            allObj[x, i + 1] = copyObj;
        }        
    }

    /// <summary>
    /// Инициализация объекта
    /// </summary>
    /// <param name="obj"></param>
    private void InitObject(ObjProps obj)
    {
        int indexOfComplexity = gameConroller.ReturnIndexOfComplexity();
        int rnd = Random.Range(0, color.Length - indexOfComplexity);
        obj.Id = rnd;
        obj.sprite.color = color[rnd];
    }

    /// <summary>
    /// Метод проверки на линии
    /// </summary>
    /// <param name="obj"></param>
    public void CheckLines(ObjProps obj)
    {
        if (!isLose && !isPaused && !isChecking)
        {
            isChecking = true;

            if (isFirstTurn)
            {
                foreach (ObjProps _obj in destroyObj)
                {
                    GameObject light = _obj.transform.GetChild(0).gameObject;
                    light.SetActive(false);
                }
                destroyObj.Clear();
                isFirstTurn = false;
                Destroy(handPrefab);
            }

            List<ObjProps> verticalLines = new List<ObjProps>();
            List<ObjProps> horizontalLines = new List<ObjProps>();

            int _x = obj.X;
            int _y = obj.Y;            

            // Проверка совпадений по вертикали, ниже выбранного объекта
            if (_y - 1 >= 0)
            {
                for (int i = _y - 1; i >= 0; i--)
                {
                    if (allObj[_x, i].Id == allObj[_x, _y].Id)
                    {
                        verticalLines.Add(allObj[_x, i]);
                    }
                    else
                    {
                        break;
                    }
                }
                verticalLines.Reverse();
            }

            destroyObj.Add(obj); // Добавление самого объекта в список для уничтожения

            // Проверка совпадений по вертикали, выше выбранного объекта
            if (_y + 1 < height)
            {
                for (int i = _y + 1; i < height; i++)
                {
                    if (allObj[_x, i].Id == allObj[_x, _y].Id)
                    {
                        verticalLines.Add(allObj[_x, i]);
                    }
                    else
                    {
                        break;
                    }
                }

            }

            // Проверка совпадений по горизонтали, левее выбранного объекта
            if (_x - 1 >= 0)
            {
                for (int i = _x - 1; i >= 0; i--)
                {
                    if (allObj[i, _y].Id == allObj[_x, _y].Id)
                    {
                        horizontalLines.Add(allObj[i, _y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Проверка совпадений по горизонтали, правее выбранного объекта
            if (_x + 1 < width)
            {
                for (int i = _x + 1; i < width; i++)
                {
                    if (allObj[i, _y].Id == allObj[_x, _y].Id)
                    {
                        horizontalLines.Add(allObj[i, _y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }


            // Добавление объектов в основной список для уничтожения (Разделил на 2 списка, т.к. дальше потребуется считать очки)
            if (verticalLines.Count > 1)
            {
                // TODO: Добавить подсчет очков
                foreach (ObjProps item in verticalLines)
                {
                    destroyObj.Add(item);
                }                
            }

            if (horizontalLines.Count > 1)
            {
                // TODO: добавить подсчет очков
                foreach (ObjProps item in horizontalLines)
                {
                    destroyObj.Add(item);
                }                
            }

            gameConroller.ScoreCount(destroyObj.Count);
            CountTurns(horizontalLines);
            CountTurns(verticalLines);

            verticalLines.Clear();
            horizontalLines.Clear();

            if (destroyObj.Count == 1)
                gameConroller.MinusTurns();

            DeleteObject(destroyObj);
        }
    }

    public void PlayExplosionSound()
    {
        explosionSound.Play();
    }

    public void CountTurns<T>(List<T> lines)
    {
        int countObj = lines.Count;
        if (countObj > 1)
        {
            switch (countObj)
            {
                case 2:
                    gameConroller.PlusTurns(2);
                    break;
                case 3:
                    gameConroller.PlusTurns(3);
                    break;
                default:
                    gameConroller.PlusTurns(4);
                    break;
            }
        }        
    }


    //подсветить бомбу...
    private void LightBomb(ObjProps obj)
    {
        GameObject light = obj.transform.GetChild(0).gameObject;
        light.SetActive(true);
        light.GetComponent<SpriteRenderer>().color = obj.GetComponent<SpriteRenderer>().color;
    }

    public void CheckFirstTurn()
    {
        int j = -1;

        // проверка на наличие линий по горизонтали
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x + 2 < width && j < 0 && allObj[x + 1, y].Id == allObj[x, y].Id && allObj[x + 2, y].Id == allObj[x, y].Id)
                {
                    j = allObj[x, y].Id;
                }

                if (j == allObj[x, y].Id)
                {
                    destroyObj.Add(allObj[x, y]);
                }
                else
                {
                    j = -1;
                }
            }
            j = -1;
        }

        j = -1;
        
        // проверка на наличие линий по вертикали
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                if (x + 2 < height && j < 0 && allObj[y, x + 1].Id == allObj[y, x].Id && allObj[y, x + 2].Id == allObj[y, x].Id)
                {
                    j = allObj[y, x].Id;
                }

                if (j == allObj[y, x].Id)
                {
                    destroyObj.Add(allObj[y, x]);
                }
                else
                {
                    j = -1;
                }
            }
            j = -1;
        }
        
        if (destroyObj.Count > 0)
        {
            targetForHand = destroyObj[0].transform.position;
            foreach (ObjProps obj in destroyObj)
            {
                LightBomb(obj);
            }
        }
        else
        {
            int _x = Random.Range(0, width);
            int _y = Random.Range(0, height);
            ObjProps _obj = allObj[_x, _y];
            destroyObj.Add(_obj);
            LightBomb(_obj);
            targetForHand = _obj.transform.position;
        }
                  
    }
}