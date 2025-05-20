using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.ZeroVelocity();
        
        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x < player.transform.position.x && player.facingDir == 1)
            player.Flip();
        else if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x && player.facingDir == -1)
            player.Flip();

        if(Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);
    }
}
