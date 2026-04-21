using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVine : EnemyBase
{
    private int nextMove = 1;
    public LayerMask checkLayer;
    public LayerMask groundLayer;
    [SerializeField] private bool isActive = false;
    [SerializeField] private float maxMoveDistance = 5f;
    private Vector3 startPosition;
    [SerializeField] protected float groundCheckLength = 1;
    [SerializeField] protected float wallCheckLength = 1;

    [SerializeField] private Sprite idleSprit;

    private void Start()
    {
        Init();
        startPosition = transform.position;
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PatrollingMove();
    }
   

    protected override void Chasing() 
    {
        if (isActive)
        {
            anim.SetBool("move", true);
            return;
        }
        else
        {
            anim.SetTrigger("ready");
        }
    }

    //protected override void Patrolling()
    //{
    //    isActive = false;
    //    sp.sprite = idleSprit;
    //    anim.SetBool("move", false);
    //    rb.velocity = Vector2.zero;
    //}

    private void DoActive()
    {
        isActive = true;
        anim.SetBool("move", true);
    }

    protected void PatrollingMove()
    {
        if (!isActive || isRecoiling)
        {
            return;
        }
        anim.SetBool("move", true);
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

        if (enemyState != EnemyState.Patrolling)
        {
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