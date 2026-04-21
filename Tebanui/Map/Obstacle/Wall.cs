using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;
    [SerializeField] private GameObject dieEffect;
    private SpriteRenderer sprite;


    void Start()
    {
        curHp = maxHp;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }


    public void TakeDamage(float _damage)
    {
        curHp-=_damage;
        if (curHp <=0)
        {
            sprite.enabled = false;
            Instantiate(dieEffect,transform.position,Quaternion.identity);
            Destroy(gameObject,0.4f);
        }
    }
}
