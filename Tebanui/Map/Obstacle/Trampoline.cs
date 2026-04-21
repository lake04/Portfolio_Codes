using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float bounceForce;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player√Êµπ");
            collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounceForce,ForceMode2D.Impulse);
        }
    }
}
