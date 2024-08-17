using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    protected float cooldownTimer;
    protected Player player;
    public UI_InGame inGameUI;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill(bool _useSkill = true)
    {
        if (cooldownTimer < 0)
        {
            if (_useSkill)
            {
                UseSkill();
                cooldownTimer = cooldown;
            }
            return true;
        }
        
        return false;
    }

    public virtual void resetTimer()
    {
        cooldownTimer = cooldown;
    }

    public virtual void UseSkill()
    {
        
    }
}
