using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : EnemyBase
{
    protected int nextMove = 1;
    public LayerMask checkLayer;
    public LayerMask groundLayer;
    [SerializeField] protected float maxMoveDistance = 5f;
    protected Vector3 startPosition;
    [SerializeField] protected float groundCheckLength = 1;
    [SerializeField] protected float wallCheckLength = 1;

    private void Start()
    {
        startPosition = transform.position;
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        if (enemyState == EnemyState.Patrolling || enemyState == EnemyState.Chasing)
        {
            PatrollingMove(); 
        }
    }


    protected override void Patrolling()
    {
     
    }

    protected override void Chasing()
    {
        if (playerTarget != null)
        {
            float directionToPlayer = playerTarget.transform.position.x - transform.position.x;

            if (directionToPlayer > 0)
            {
                nextMove = 1;
                sp.flipX = false;
            }
            else if (directionToPlayer < 0)
            {
                nextMove = -1;
                sp.flipX = true;
            }
            PatrollingMove();
        }
    }


    protected override void Attack()
    {
        
    }

    protected  void PatrollingMove()
    {
        anim.SetBool("move",true);
        rb.velocity = new Vector2(nextMove * moveSpeed, rb.velocity.y);

        Vector2 downVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        RaycastHit2D groundRayHit = Physics2D.Raycast(downVec, Vector3.down, groundCheckLength, groundLayer);

        Vector2 frontVec = new Vector2(rb.position.x + nextMove * wallCheckLength, rb.position.y);
        RaycastHit2D wallRayHit = Physics2D.Raycast(frontVec, Vector2.right * nextMove, 0.4f, checkLayer);

        Debug.DrawRay(downVec, Vector3.down * groundCheckLength, Color.red);
        Debug.DrawRay(frontVec, Vector2.right * nextMove * wallCheckLength, Color.blue);

        float curDistance = transform.position.x - startPosition.x;
        bool isTurn = false;

        if (groundRayHit.collider == null || wallRayHit.collider != null)
        {
            isTurn = true;
        }

        if (enemyState == EnemyState.Patrolling)
        {
            if (Mathf.Abs(curDistance) >= maxMoveDistance)
            {
                if (nextMove > 0 && curDistance > 0)
                {
                    isTurn = true;
                }
                else if (nextMove < 0 && curDistance < 0)
                {
                    isTurn = true;
                }
            }
        }

        if (isTurn)
        {
            nextMove *= -1;
            if (nextMove > 0)
            {
                sp.flipX = true;
            }
            else 
            {
                sp.flipX = false;
            }
        }

    }
}