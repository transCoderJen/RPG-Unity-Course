using System.Collections;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [Header("Clone Creation")]
    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool createCloneOnCounter;

    [Header("Clone Duplication")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private int duplicationChancePercent;

    [Header("Crystal Spawn")]
    [SerializeField] private bool canSpawnCrystal;
    [SerializeField] private GameObject crystalPrefab;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if(canSpawnCrystal)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        
        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, canDuplicateClone, duplicationChancePercent);
    }

    public bool CanSpawnCrystal() => canSpawnCrystal;
    
    public void CreateCloneOnDashStart()
    {
        if(createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if(createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnCounter(Transform _clonePosition, Vector3 _offset)
    {
        StartCoroutine(Wait(.4f));
        if(createCloneOnCounter)
            CreateClone(_clonePosition, _offset);
    }

    private IEnumerator Wait(float _seconds){
        yield return new WaitForSeconds(_seconds);
    }
}
