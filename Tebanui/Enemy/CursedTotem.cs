using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedTotem : EnemyBase
{
    [SerializeField]
    private GameObject rootPrefab;

    void Start()
    {
        
    }

    protected override void Update()
    {
        float directionToPlayer = playerTarget.transform.position.x - transform.position.x;

        if (directionToPlayer > 0)
        {
            sp.flipX = true;
        }
        else if (directionToPlayer < 0)
        {
            sp.flipX = false;
        }
    }
  

    protected override void Chasing()
    {
        if (isAttack)
        {
            StartCoroutine(IEAttack());
        }
    }

 
    protected override void Patrolling()
    {
       
    }

   IEnumerator IEAttack()
    {
        isAttack = false;
     
        anim.SetTrigger("attack");
      
        yield return new WaitForSeconds(2f);
        
        isAttack = true;
    }
    
    private void SpawnRoot()
    {
        Debug.Log("µ¢±¼ ¼ÒÈ¯");
        Vector3 pos = playerTarget.transform.position;
        pos.y = -2f;
        GameObject rootClone = Instantiate(rootPrefab, pos, Quaternion.identity);
    }
}


