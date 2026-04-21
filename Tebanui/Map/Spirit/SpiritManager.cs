using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    public static SpiritManager Instance;


    [SerializeField] private FireSpirit fireSpirit;
    [SerializeField] private WindSpirit windSpirit;
    [SerializeField] private WaterSpirit waterSpirit;
    [SerializeField] private GroundSpirit groundSpirit;

    [SerializeField] private GameObject fireSpiritPrefab;
    public GameObject fireSpiritSpawnPos;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            fireSpirit.Skill1();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            groundSpirit.Skill1();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.CompareTag("Spirit"))
        {
            Spirit sprite = collision.GetComponent<Spirit>();
            
            switch(sprite.spiritType)
            {
                case SpiritType.Fire:
                    fireSpirit.GetSoul(sprite.spiritAmput);
                    break;

                case SpiritType.Water:
                    waterSpirit.GetSoul(sprite.spiritAmput);
                    break;

               case SpiritType.Ground:
                    groundSpirit.GetSoul(sprite.spiritAmput);
                    break;

                case SpiritType.Wind:
                    windSpirit.GetSoul(sprite.spiritAmput);
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
}
