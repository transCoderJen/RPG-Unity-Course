using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.CanUseSkill(false) && player.skill.blackhole.blackholeUnlocked)   
            stateMachine.ChangeState(player.blackHole);

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSword);

        if(Input.GetKeyDown(KeyCode.Q) && SkillManager.instance.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttack);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        if (!player.IsGroundDetected() && !player.IsPlatformDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.S) && player.IsPlatformDetected())
            stateMachine.ChangeState(player.airState);

        else if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.ChangeState(player.jumpState);  
    }

    private bool HasNoSword()
    {
        if(!player.sword)
        {
            return true;
        }

        if(!player.sword.GetComponent<SwordSkillController>().canRotate)
            player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
