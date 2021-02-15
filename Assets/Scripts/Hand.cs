using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private Board board;
    private Vector3 targetPosition;
    private float speed = 5f;
    private bool isOnTarget = false;

    
    private void Start()
    {
        board = FindObjectOfType<Board>();
        targetPosition = board.targetForHand;
        targetPosition.z = transform.position.z;
    }       

    private void FixedUpdate()
    {
        if (board.isFirstTurn && isOnTarget == false)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            if (transform.position == targetPosition)
            {
                transform.GetChild(1).gameObject.SetActive(true);
                isOnTarget = true;
            }
        }
    }
}
