using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousMushroom : MonoBehaviour
{
    public GameObject effect;

    // Start is called before the first frame update
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
            Instantiate(effect,transform.position,Quaternion.identity);
        }
    }
}
