using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeSkill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot dodgeUnlockButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked { get; private set; }

    [Header("Dodge Mirage")]
    [SerializeField] private UI_SkillTreeSlot dodgeMirageUnlockButton;
    public bool dodgeMirageUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();

        dodgeUnlockButton.OnFullyFilled += UnlockDodge;
        dodgeMirageUnlockButton.OnFullyFilled += UnlockDodgeMirage;
    }

    private void UnlockDodge()
    {
        if (dodgeUnlockButton.unlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockDodgeMirage()
    {
        if (dodgeMirageUnlockButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
    }



}
