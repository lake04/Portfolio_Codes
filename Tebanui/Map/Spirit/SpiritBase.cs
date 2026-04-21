using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpiritBase : MonoBehaviour
{
    [SerializeField] protected int maxLevel = 2;
    [SerializeField] protected int curLevel = 0;
    [SerializeField] protected int spiritSoul = 0;
    [SerializeField] protected int[] needSpiritSoul =  { 5,8 };

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public virtual void Skill1()
    {

    }
    public virtual void Skill2()
    {

    }

    public void GetSoul(int _amount)
    {
        spiritSoul += _amount;

        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if(curLevel< maxLevel)
        {
            if(spiritSoul> needSpiritSoul[curLevel])
            {
                curLevel++;
                PassiveSkill();
            }
        }
    }

    protected virtual void PassiveSkill()
    {

    }
}
