using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjProps : MonoBehaviour
{
    [SerializeField] private float speed;
    private Board board;

    public SpriteRenderer sprite; // ссылка на компонент SpriteRenderer объекта

    public int Id { get; set; } // Id объекта - соответствует цвету
    public int X { get; set; } // координата объекта по X
    public int Y { get; set; } // координата объекта по Y

    public Vector3 targetPosition; // поле, для обозначения целевой позиции объекта при перемещении
    public bool isMoving; // булевая переменная, для обозначения процесса, когда объект движется    

    //private Vector2 touchPos;    

    private void Start()
    {
        board = FindObjectOfType<Board>();
        X = (int)transform.position.x;
        Y = (int)transform.position.y;        
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// Метод клика по объекту
    /// </summary>
    private void OnMouseDown()
    {
        //touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board.CheckLines(this);

        //Debug.Log(touchPos);
    }
}
