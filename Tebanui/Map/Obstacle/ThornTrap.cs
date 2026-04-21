using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornTrap : MonoBehaviour
{
    [SerializeField] private int damage;



    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 충돌!");
        }
    }
}
