using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ParrySkill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry Restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    public bool restoreUnlocked {get; private set; }
    [Range(0f,1f)]
    [SerializeField] private float restorePercentage;

    [Header("Parry Clone")]
    [SerializeField] private UI_SkillTreeSlot cloneUnlockButton;
    public bool cloneUnlocked {get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int RestoreHealthAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restorePercentage);
            player.stats.IncreaseHealthBy(RestoreHealthAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.OnFullyFilled += UnlockParry;
        restoreUnlockButton.OnFullyFilled += UnlockRestore;
        cloneUnlockButton.OnFullyFilled += UnlockClone;
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
        {
            parryUnlocked = true;
            inGameUI.UnlockParry();
        }
    }

    private void UnlockRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    private void UnlockClone()
    {
        if (cloneUnlockButton.unlocked)
            cloneUnlocked = true;
    }

    public void MakeMirageOnParry(Transform _spawnTransform, Vector3 _offset)
    {
        if(cloneUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_spawnTransform, _offset);
    }

}
