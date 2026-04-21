using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireSpiritRemnant : GroundEnemy
{

    protected override void FixedUpdate()
    {
        if (isDead) return;

        HandleState();

    }

    protected override void Patrolling()
    {
        anim.SetBool("idle", true);
        rb.velocity = Vector2.zero;
    }

    protected override void Chasing()
    {
        if (playerTarget != null)
        {
            if(enemyState == EnemyState.Attacking)
            {
                return;
            }
            float directionToPlayer = playerTarget.transform.position.x - transform.position.x;

            if (directionToPlayer > 0)
            {
                nextMove = 1;
                sp.flipX = true;
            }
            else if (directionToPlayer < 0)
            {
                nextMove = -1;
                sp.flipX = false;
            }
            PatrollingMove();
        }
    }


    protected override void Attack()
    {
        ChangeState(EnemyState.Attacking);
        StartCoroutine(Explosion());
    }
    

    private IEnumerator Explosion()
    {
        isAttack = false;
        ChangeState(EnemyState.Attacking);
        rb.velocity = Vector3.zero;
        Debug.Log("ÆøÆÈ!!!!");

        anim.SetBool("boom",true);

        yield return new WaitForSeconds(2f);

        anim.SetBool("boom2", true);

        yield return new WaitForSeconds(1f);
        Die();
        isAttack = true;
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        Destroy(gameObject);

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }
}
