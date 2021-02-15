using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjProps : MonoBehaviour
{
    [SerializeField] private float speed;
    private Board board;

    public SpriteRenderer sprite; // ������ �� ��������� SpriteRenderer �������

    public int Id { get; set; } // Id ������� - ������������� �����
    public int X { get; set; } // ���������� ������� �� X
    public int Y { get; set; } // ���������� ������� �� Y

    public Vector3 targetPosition; // ����, ��� ����������� ������� ������� ������� ��� �����������
    public bool isMoving; // ������� ����������, ��� ����������� ��������, ����� ������ ��������    

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
    /// ����� ����� �� �������
    /// </summary>
    private void OnMouseDown()
    {
        //touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board.CheckLines(this);

        //Debug.Log(touchPos);
    }
}
