using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointStatue : MonoBehaviour
{
    private bool isPlayerNearby = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.Instance.respawnPoint = transform;
            Debug.Log("체크포인트 갱신");
        }
    }
}
