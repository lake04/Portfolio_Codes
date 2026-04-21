using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapArrow : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector2 direction;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameObject arrow = Instantiate(arrowPrefab, spawnPos.position,Quaternion.identity);
            arrow.GetComponent<Arrow>().Init(direction);
        }
    }
}
