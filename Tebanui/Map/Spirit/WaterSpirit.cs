using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpirit : SpiritBase
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void PassiveSkill()
    {
        if (curLevel == 1)
        {
            GameManager.Instance.player.GetComponent<Player>().swimSpeed = 5f;
        }

        if (curLevel == 2)
        {
        }
    }
}
