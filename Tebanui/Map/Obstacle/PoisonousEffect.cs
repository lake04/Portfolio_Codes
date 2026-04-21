using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousEffect : MonoBehaviour
{
    [SerializeField] private float destoryTime;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,destoryTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player µ¶ °ø°Ý!");
        }
    }
}
