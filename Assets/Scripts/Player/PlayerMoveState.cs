public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (player.IsGroundDetected())
            player.fx.CreateDustParticles(DustParticleType.Running);
        AudioManager.instance.PlaySFX(SFXSounds.footsteps, null);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(SFXSounds.footsteps);
    }
    
    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if(xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);
    }

}
