using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class AbyssRay : EnemyBase
{
    [Header("Setting")]
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;

    [SerializeField] private float xRange;
    [SerializeField] private int nextMove = 1;

    private float waveTimer = 0f;
    private Vector3 startPos;

    [SerializeField] private GameObject electricShockWavePrefab;
    [SerializeField] private Transform spawnPos;

    void Start()
    {
        startPos = transform.position;
    }

    protected override void Patrolling()
    {
        MoveWave();
    }


    protected override void Chasing()
    {
        ChasingMove();
    }

    protected override void Attack()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        isAttack = false;
        GameObject electricShockWave = Instantiate(electricShockWavePrefab, spawnPos);

        Vector2 dir = (playerTarget.transform.position - spawnPos.position).normalized;
        electricShockWave.GetComponent<ProjectileBase>().Init(dir);

        yield return new WaitForSeconds(3f);
        isAttack = true;
    }

    private void MoveWave()
    {
        float curPos = Vector2.Distance(transform.position, startPos);
        if (detectionRange < curPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
        }
        else
        {
            waveTimer += Time.deltaTime * waveFrequency;
            float yOffset = Mathf.Cos(waveTimer) * waveAmplitude;

            Vector3 targetPos = new Vector3(
                transform.position.x + nextMove * moveSpeed * Time.deltaTime,
                startPos.y + yOffset,
                0
            );

            transform.position = targetPos;

            if (transform.position.x <= startPos.x - xRange)
            {
                nextMove = 1;
                Flip();
            }
            else if (transform.position.x >= startPos.x + xRange)
            {
                nextMove = -1;
                Flip();
            }
        }



    }

    private void ChasingMove()
    {
        float directionToPlayer = playerTarget.transform.position.x - transform.position.x;
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.transform.position, moveSpeed * Time.deltaTime);

        if (directionToPlayer > 0)
        {
            nextMove = 1;
        }
        else if (directionToPlayer < 0)
        {
            nextMove = 11;
        }

        Flip();
    }

    private void Flip()
    {
        if (nextMove != 0)
        {
            Vector3 newScale = transform.localScale;

            newScale.x = Mathf.Sign(nextMove) * 2;

            // localScaleÀ» Àû¿ë
            transform.localScale = newScale;
        }
    }

}
