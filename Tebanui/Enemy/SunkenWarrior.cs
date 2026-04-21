using System.Collections;
using UnityEngine;

public enum SunkenWarriorStat
{
    Idle,
    Move,
    Attack,
    Transform,
    Die
}

public enum BossPhase
{
    Phase1,
    Phase2,
}

public class SunkenWarrior : MonoBehaviour
{
    public SunkenWarriorStat stat;
    public BossPhase phase = BossPhase.Phase1;
    private bool isStart = true;

    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float moveRange = 8.0f; 

    [Header("Attack")]
    [SerializeField] private float dashSpeed = 15.0f; 
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float skill1Cooldown = 2.0f; 
    [SerializeField] private float attackCooldown = 2.0f; 
    [SerializeField] private float dashDuration = 0.3f;
    private Transform startPos;

    private int lastPhasePatternIndex = -1;

    private bool isAttack = true;

    [Header("피격 처리")]
    [SerializeField] private float recoilLength;
    [SerializeField] private float recoilFactor;
    [SerializeField] private bool isRecoiling = false;

    private bool isDead = false;
    private SpriteRenderer sp;

    private Rigidbody2D rb;
    private GameObject playerTarget;
    private Animator anim;

    private bool isFacingRight = false;

    [Header("Prefab")]
    [SerializeField] private GameObject harpoonPrefab;
    [SerializeField] private GameObject skill2HarpoonPrefab;
    [SerializeField] private Transform harpoonSpawnPos;

    [SerializeField] private GameObject waterDrillPrefab;
    [SerializeField] private Transform waterDrillSpawnPos1;
    [SerializeField] private Transform waterDrillSpawnPos2;
    [SerializeField] private GameObject dieEffect;
    private FlashSprite flashSprite;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameManager.Instance.player;
        anim = GetComponent<Animator>();
        flashSprite = GetComponent<FlashSprite>();
        ChangeState(SunkenWarriorStat.Idle);

        startPos = transform;
        curHp = maxHp;
    }

    void Update()
    {
        if (playerTarget == null) return;

        if(stat == SunkenWarriorStat.Transform)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.transform.position);

        if (distanceToPlayer < moveRange && stat != SunkenWarriorStat.Attack)
        {
            if(isStart)
            {
                StartCoroutine(PhaseController());
                isStart = false;
            }
            ChangeState(SunkenWarriorStat.Move);
        }
        else if (stat != SunkenWarriorStat.Attack)
        {
            ChangeState(SunkenWarriorStat.Idle);
        }

        float targetX = playerTarget.transform.position.x;
        float myX = transform.position.x;

        if (targetX < myX && isFacingRight)
        {
            Flip();
        }
        else if (targetX > myX && !isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        switch (stat)
        {
            case SunkenWarriorStat.Transform:
                rb.velocity = Vector2.zero;
                break;
                
            case SunkenWarriorStat.Idle:
                rb.velocity = Vector2.zero;

                if(phase == BossPhase.Phase1)
                {
                    anim.SetBool("1PMove", false);
                    anim.SetBool("1PIdle", true);
                }
                else
                {
                    anim.SetBool("2PMove", false);
                    anim.SetBool("2PIdle", true);
                }
                break;

            case SunkenWarriorStat.Move:
                Move();
                break;
            case SunkenWarriorStat.Attack:
           
                break;
        }
    }

    private void Move()
    {
        if (phase == BossPhase.Phase1)
        {
            anim.SetBool("1PMove", true);
            anim.SetBool("1PIdle", false);

        }
        else
        {
            anim.SetBool("2PMove", true);
            anim.SetBool("2PIdle", false);
        }
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTarget.transform.position,
            moveSpeed * Time.fixedDeltaTime 
        );

       
    }

    private void UpdatePhase()
    {
        if (curHp <= maxHp * 0.5f && phase == BossPhase.Phase1)
        {
            phase = BossPhase.Phase2;
            StartCoroutine(PhaseChage());
        }
    }

    private IEnumerator PhaseController()
    {
        yield return new WaitForSeconds(1.0f);

        while (stat != SunkenWarriorStat.Die)
        {
            if (stat == SunkenWarriorStat.Transform || stat == SunkenWarriorStat.Attack)
            {
                yield return null;
                continue;
            }

            if (phase == BossPhase.Phase1)
            {
                yield return StartCoroutine(Phase1Routine());
            }
            else
            {
                yield return StartCoroutine(Phase2Routine());
            }

        }
    }


    private IEnumerator Phase1Routine()
    {
        ChangeState(SunkenWarriorStat.Attack);
        int patternCount = 3;
        int random;

        random = Random.Range(0, patternCount);

        if (random == lastPhasePatternIndex)
        {
            random = (random + 1) % patternCount;
        }
        lastPhasePatternIndex = random;

        isAttack = false;


        switch (random)
        {
            case 0:
                StartCoroutine(SkillBasicStab());
                break;

            case 1:
                StartCoroutine(Skill1AttackDash());
                break;

            case 2:
                StartCoroutine(SkillHarpoon());
                break;
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }

    private IEnumerator PhaseChage()
    {
        ChangeState(SunkenWarriorStat.Transform);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        int spawnCount = 10;

        for (int i = 0; i < 10; i++)
        {
            GameObject spawnObject = Instantiate(waterDrillPrefab);
            spawnObject.transform.position = transform.position;
            spawnObject.transform.rotation = Quaternion.identity;

            float x = Mathf.Cos(Mathf.PI * 2 * i / spawnCount);
            float y = Mathf.Sin(Mathf.PI * 2 * i / spawnCount);

            Vector2 dirVec = new Vector2(x, y);

            spawnObject.GetComponent<ProjectileBase>().Init(dirVec);

            float rotZ = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
            spawnObject.transform.rotation = Quaternion.Euler(0f, 0f, -rotZ);
        }

        yield return new WaitForSeconds(0.5f);

        ChangeState(SunkenWarriorStat.Idle);
    }

    #region 패턴1
    private IEnumerator SkillBasicStab()
    {
        rb.velocity = Vector2.zero;

        //TODO : 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator Skill1AttackDash()
    {
        anim.SetBool("1pIdle",true);
        yield return new WaitForSeconds(0.3f);
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;
        anim.SetTrigger("1pDash");

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator SkillHarpoon()
    {
        if (phase == BossPhase.Phase1)
        {
            anim.SetBool("1PMove", false);
            anim.SetBool("1PIdle", true);
        }
        else
        {
            anim.SetBool("2PMove", false);
            anim.SetBool("2PIdle", true);
        }

        yield return new WaitForSeconds(0.6f);
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        //SoundManager.instance.PlaySFX(SoundType.SFX_SHOOT);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject harpoonCIone = Instantiate(harpoonPrefab, harpoonSpawnPos.position, Quaternion.identity);
        harpoonCIone.GetComponent<Harpoon>().Init(direction, harpoonSpawnPos);
        harpoonCIone.transform.rotation =  Quaternion.Euler(0,0,angle);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(1f);
        //Destroy(harpoonCIone);

        ChangeState(SunkenWarriorStat.Move);
    }
    #endregion


    #region 패턴 2
    private IEnumerator Phase2Routine()
    {
        int patternCount = 3;
        int random;

        random = Random.Range(0, patternCount);

        if (random == lastPhasePatternIndex)
        {
            random = (random + 1) % patternCount;
        }
        lastPhasePatternIndex = random;

        isAttack = false;

        switch (random)
        {
            case 0:
                StartCoroutine(Skill2BasicStab());
                break;

            case 1:
                StartCoroutine(SkillHarpoonAndDash());
                break;

            case 2:
                StartCoroutine(Skill2WaterDrill());
                break;

            case 3:

                break;
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttack = true;
    }

    private IEnumerator Skill2BasicStab()
    {
        rb.velocity = Vector2.zero;


        //TODO : 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator Skill2WaterDrill()
    {
        if (phase == BossPhase.Phase1)
        {
            anim.SetBool("1PMove", false);
            anim.SetBool("1PIdle", true);
        }
        else
        {
            anim.SetBool("2PMove", false);
            anim.SetBool("2PIdle", true);
        }

        yield return new WaitForSeconds(0.15f);
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject waterDrillCIone = Instantiate(waterDrillPrefab, waterDrillSpawnPos1.position, Quaternion.identity);
        GameObject waterDrillCIone2 = Instantiate(waterDrillPrefab, waterDrillSpawnPos2.position, Quaternion.identity);
        waterDrillCIone.GetComponent<WaterDrill>().Init(direction);
        waterDrillCIone2.GetComponent<WaterDrill>().Init(direction);

        waterDrillCIone.transform.rotation = Quaternion.Euler(0, 0, angle);
        waterDrillCIone2.transform.rotation = Quaternion.Euler(0, 0, angle);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(0.7f);
     
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator SkillHarpoonAndDash()
    {
        if (phase == BossPhase.Phase1)
        {
            anim.SetBool("1PMove", false);
            anim.SetBool("1PIdle", true);
        }
        else
        {
            anim.SetBool("2PMove", false);
            anim.SetBool("2PIdle", true);
        }

        yield return new WaitForSeconds(0.15f);
        Vector2 direction = (playerTarget.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //TODO : 작살 던지는 애니메이션 실행
        GameObject harpoonCIone = Instantiate(skill2HarpoonPrefab, harpoonSpawnPos.position,Quaternion.identity);
        harpoonCIone.GetComponent<Harpoon>().Init(direction, harpoonSpawnPos);
        harpoonCIone.transform.rotation = Quaternion.Euler(0, 0, angle);
        //TODO : 작살 회수하는 애니메이션 실행

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DashToTarget(harpoonCIone.transform.position, 0.3f));
        Destroy(harpoonCIone);
        ChangeState(SunkenWarriorStat.Move);
    }

    private IEnumerator DashToTarget(Vector3 targetPosition, float duration)
    {
        anim.SetBool("2pIdle",true);
        yield return new WaitForSeconds(0.1f);
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        anim.SetTrigger("2pDash");
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        transform.position = targetPosition;
    }

    #endregion

    protected void ChangeState(SunkenWarriorStat _newState)
    {
        if (_newState != SunkenWarriorStat.Attack || stat != SunkenWarriorStat.Attack)
        {
            stat = _newState;
        }
    }

    public  void TakeDamage(float _damage, Vector2 _hitDirecticon, float _hitForce)
    {
        if (isDead) return;
        Debug.Log("공격 받음");
        curHp -= _damage;

        if (flashSprite != null)
        {
            flashSprite.Flash();
        }
        UpdatePhase();
        if (curHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        ChangeState(SunkenWarriorStat.Die);

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

         rb.velocity = Vector2.zero;
         rb.isKinematic = true;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        Instantiate(dieEffect, transform.position, Quaternion.identity);
        StartCoroutine(End());

        Destroy(gameObject);

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator End()
    {
        yield return new WaitForSeconds(1f);
        LoadingManager.instance.Loading("End");
    }
}