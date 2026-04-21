using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : SpiritBase
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
            GameManager.Instance.player.GetComponent<Player>().canDash = true;
        }

        //if (curLevel == 2)
        //{
        //    GameManager.Instance.player.GetComponent<Player>().maxAirJumps = 2;
        //}
    }
}
