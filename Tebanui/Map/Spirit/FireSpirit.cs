using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : SpiritBase
{
    [SerializeField] private GameObject fireBall;
    [SerializeField] private float manaCostSkill1 = 10f;

    [SerializeField] private GameObject spawnPos;

    private Animator animator;
    float playerScaleX;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(SpiritManager.Instance !=null)
        {
            transform.position = SpiritManager.Instance.fireSpiritSpawnPos.transform.position;

        }
        if(GameManager.Instance != null)
        {
           playerScaleX = GameManager.Instance.player.GetComponent<Player>().transform.localScale.x;
        }

        Vector3 spiritScale = transform.localScale;

        spiritScale.x = Mathf.Abs(spiritScale.x) * Mathf.Sign(playerScaleX);

        transform.localScale = spiritScale;
    }

    void Update()
    {
        
    }

    public override void Skill1()
    {
        if (curLevel < 1)
        {
            return;
        }
        gameObject.SetActive(true);

        StartCoroutine(IE_Attack());
    }        

    private IEnumerator IE_Attack()
    {
        Vector2 fireDirection = Vector2.zero;

        float playerScaleX = GameManager.Instance.player.GetComponent<Player>().transform.localScale.x;

        yield return new WaitForSeconds(0.8f);

        GameObject fire = Instantiate(fireBall, spawnPos.transform.position, Quaternion.identity);

        if (playerScaleX > 0)
        {
            fireDirection = Vector2.right;
        }
        else
        {
            fireDirection = Vector2.left;
        }

        fire.GetComponent<FireBall>().Init(fireDirection);

        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    public override void Skill2()
    {
       
    }
}

