using System.Collections;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    
    [Header("Clone Attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    public bool canAttack { get; private set; }

    [Header("Aggressive Mirage")]
    [SerializeField] private UI_SkillTreeSlot aggressiveMirageUnlockButton;
    public bool aggressiveMirageUnlocked;

    [Header("Multiple Clone")]
    [SerializeField] private UI_SkillTreeSlot multipleCloneUnlockButton;
    public bool canDuplicateClone { get; private set; }
    [SerializeField] private int duplicationChancePercent;

    [Header("Crystal Spawn")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool canSpawnCrystal { get; private set; }
    [SerializeField] private GameObject crystalPrefab;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.OnFullyFilled += UnlockCloneAttack;
        aggressiveMirageUnlockButton.OnFullyFilled += UnlockAggressiveMirage;
        multipleCloneUnlockButton.OnFullyFilled += UnlockMultipleMirage;
        crystalUnlockButton.OnFullyFilled += UnlockCrystal;
    }

    #region Unlock Skills
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
            canAttack = true;
    }

    private void UnlockAggressiveMirage()
    {
        if (aggressiveMirageUnlockButton.unlocked)
            aggressiveMirageUnlocked = true;
    }

    private void UnlockMultipleMirage()
    {
        if (multipleCloneUnlockButton.unlocked)
            canDuplicateClone = true;
    }

    private void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            canSpawnCrystal = true;
    }
    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if(canSpawnCrystal)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        
        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, canDuplicateClone, duplicationChancePercent, player);
    }

    public bool CanSpawnCrystal() => canSpawnCrystal;
    
    

    public void CreateCloneWithDelay(Transform _clonePosition, Vector3 _offset)
    {
        StartCoroutine(Wait(.4f));
        CreateClone(_clonePosition, _offset);
    }

    private IEnumerator Wait(float _seconds){
        yield return new WaitForSeconds(_seconds);
    }
}
