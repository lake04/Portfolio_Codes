using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private GameObject attackRangeSnow;

    private Animator anim;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();

        //StartCoroutine(Init());
        Destroy(gameObject, 1.5f);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 뿌리 공격");
            Player.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    //private IEnumerator Init()
    //{
    //    attackRangeSnow.SetActive(true);
    //    yield return new WaitForSeconds(1);
    //    attackRangeSnow.SetActive(false);

    //    anim.SetTrigger("isAttack");
    //    Destroy(gameObject, 1f);
    //}
}
