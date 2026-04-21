using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private GameObject destroyEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>().TakeDamage(damage,Vector2.zero,0.5f);
        }
        else if(collision.CompareTag("DestroyWall"))
        {
            collision.GetComponent<Wall>().TakeDamage(1f);
        }
       
        Instantiate(destroyEffect,transform.position,Quaternion.identity);
        Destroy(gameObject,0.7f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(damage, Vector2.zero, 0.5f);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("DestroyWall"))
        {
            collision.gameObject.GetComponent<Wall>().TakeDamage(1f);
        }

        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.7f);
    }
}
