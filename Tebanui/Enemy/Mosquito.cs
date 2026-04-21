using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mosquito : EnemyBase
{
    [Header("Setting")]
    [SerializeField] private float escapeSpeed = 6f;
    [SerializeField] private float descentSpeed = 6f;

    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;   

    [SerializeField] private float xRange;
    private int nextMove = 1;

    private bool isCurrentlyEscaping = false;
    private Coroutine descentCoroutine;

    private float waveTimer = 0f;
    private Vector3 startPos;

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
        MoveWave();
    }

    protected override void Attack() 
    {
        if (descentCoroutine == null && !isCurrentlyEscaping)
        {
            descentCoroutine = StartCoroutine(Descent());
        }
    }

    private void MoveWave()
    {
        if (isCurrentlyEscaping) return; 

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
            sp.flipX = true;
        }
        else if (transform.position.x >= startPos.x + xRange)
        {
            nextMove = -1;
            sp.flipX = false;

        }
    }

    private IEnumerator Descent()
    {
        if (enemyState != EnemyState.Hurt && enemyState != EnemyState.Die)
        {
            if (playerTarget == null && GameManager.Instance != null)
                playerTarget = GameManager.Instance.player;

            if (playerTarget == null)
            {
                descentCoroutine = null;
                ChangeState(EnemyState.Patrolling);
                yield break;
            }

            Vector3 targetPosition = playerTarget.transform.position;

            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    descentSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

        descentCoroutine = null;
        ChangeState(EnemyState.Patrolling);
    }

    private IEnumerator Escape()
    {
        if (isCurrentlyEscaping) yield break;

        isCurrentlyEscaping = true;

        Vector2 escapeDir = (Vector2.up + Random.insideUnitCircle * 0.2f).normalized;
        Vector2 escapeTarget = (Vector2)transform.position + escapeDir * 1f;

        while (Vector2.Distance(transform.position, escapeTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                escapeTarget,
                escapeSpeed * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        isCurrentlyEscaping = false;
        ChangeState(EnemyState.Patrolling);
    }


    public override void TakeDamage(float damage, Vector2 _hitDirecticon, float _hitForce)
    {
        base.TakeDamage(damage, _hitDirecticon,_hitForce);

        if (!isDead && !isCurrentlyEscaping)
        {
            if (descentCoroutine != null)
            {
                StopCoroutine(descentCoroutine);
                descentCoroutine = null;
            }
            StartCoroutine(Escape());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCurrentlyEscaping)
        {
            // Player player = collision.GetComponent<Player>();
            // if (player != null) player.TakeDamage(attackDamage);

            StartCoroutine(Escape());
        }
    }
}