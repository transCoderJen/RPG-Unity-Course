using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrebaf;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Crystal Simple")]
    [SerializeField] private UI_SkillTreeSlot UnlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Crystal Mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;

    [Header("Moving Crsytal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool moveTowardsClosestEnemy;

    [Header("Multi Stacking Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiCrystalButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    private float multiStackTimer;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();


    public override bool CanUseSkill(bool _useSkill = true)
    {
        return base.CanUseSkill(_useSkill);
    }

    public override void UseSkill()
    {
        base.UseSkill();
        if(canUseMultiStacks)
            CanUseMultiCrystal();
        else
        {
            if (currentCrystal == null)
            {
                CreateCrystal();
            }
            else
            {
                if (currentCrystal.GetComponent<CrystalSkillController>().canTeleportTo())
                {
                    // swap player and crystal position
                    Vector2 playerOriginalPos = player.transform.position;
                    player.transform.position = currentCrystal.transform.position;
                    currentCrystal.transform.position = playerOriginalPos;

                    if(cloneInsteadOfCrystal)
                    {
                        player.skill.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                        Destroy(currentCrystal);
                    }
                    else
                        currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
                }
            }
        }
    }

    public void CreateCrystal()
    {
        if (crystalUnlocked)
        {
            currentCrystal = Instantiate(crystalPrebaf, player.transform.position, Quaternion.identity);
            CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, growSpeed, maxSize, moveTowardsClosestEnemy, player);
        }
    }

    protected override void Start()
    {
        base.Start();
        RefillCrystal();

        UnlockCrystalButton.OnFullyFilled += UnlockCrystal;
        unlockCloneButton.OnFullyFilled += UnlockCloneInsteadOfCrystal;
        unlockExplosiveButton.OnFullyFilled += UnlockExplosiveCrystal;
        unlockMovingCrystalButton.OnFullyFilled += UnlockMovingCrystal;
        unlockMultiCrystalButton.OnFullyFilled += UnlockMultiCrystal;
    }

    private void UnlockCrystal()
    {
        if (UnlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCloneInsteadOfCrystal()
    {
        if (unlockCloneButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiCrystal()
    {
        if (unlockMultiCrystalButton.unlocked)
            canUseMultiStacks = true;
    }

    protected override void Update()
    {
        base.Update();
        multiStackTimer -= Time.deltaTime;
        if (multiStackTimer <= 0 && crystalLeft.Count < amountOfStacks)
        {
            crystalLeft.Add(crystalPrebaf);
            multiStackTimer = multiStackCooldown;
        }

    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks && crystalLeft.Count > 0)
        {
            
            GameObject crystalToSpawn = crystalLeft[crystalLeft.Count -1];
            GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
            CrystalSkillController newCrystalScript = newCrystal.GetComponent<CrystalSkillController>();

            newCrystalScript.SetupCrystal(3.5f, true, true, moveSpeed, growSpeed, maxSize, moveTowardsClosestEnemy, player);

            crystalLeft.RemoveAt(crystalLeft.Count - 1);
            return true;
        }

        return false;
    }
    private void RefillCrystal()
    {
        for (int i = 0; i < amountOfStacks; i++)
        {
            crystalLeft.Add(crystalPrebaf); 
        }
    }
}
