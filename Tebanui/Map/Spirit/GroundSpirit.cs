using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpirit : SpiritBase
{
    [SerializeField] private LayerMask checkLayer;       
    [SerializeField] private Vector3 checkRange;      
    [SerializeField] private float groundCheckRayLength = 0.5f;

    [SerializeField] private GameObject stonSpawnPos;
    [SerializeField] private GameObject stonPrefab;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Skill1()
    {
        if(CanPlaceRock(stonSpawnPos.transform.position))
        {
            GameObject ston = Instantiate(stonPrefab,stonSpawnPos.transform.position,Quaternion.identity);
        }


    }

    public override void Skill2()
    {

   

    }


    protected override void PassiveSkill()
    {
        if (curLevel == 1)
        {
        }
    }


    public bool CanPlaceRock(Vector3 _pos)
    {
        Collider2D hit = Physics2D.OverlapBox(_pos, checkRange, 0, checkLayer);

        if (hit != null)
        {
            if (hit.CompareTag("Wall"))
            {
                return false;
            }
        }
 
        Vector2 rayOrigin = new Vector2(_pos.x, _pos.y - checkRange.y / 2f); 

        RaycastHit2D groundHit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            groundCheckRayLength, 
            checkLayer 
        );

        if (groundHit.collider != null)
        {
            if (groundHit.collider.CompareTag("Ground"))
            {
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (stonSpawnPos != null)
        {
            Vector3 spawnPos = stonSpawnPos.transform.position;
            spawnPos.y += 1;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spawnPos, checkRange);

            Vector3 rayOrigin = new Vector3(spawnPos.x, spawnPos.y - checkRange.y / 2f);
            Vector3 rayEnd = rayOrigin + Vector3.down * groundCheckRayLength;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayOrigin, rayEnd);
        }
    }

}
