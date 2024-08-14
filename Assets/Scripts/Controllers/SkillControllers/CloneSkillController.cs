using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloneSkillController : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;

    [SerializeField] private float colorLossSpeed;
    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private bool canDuplicateClone;
    private int duplicationChancePercent;
    private int facingDir = 1;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLossSpeed));

            if(sr.color.a <= 0)
                Destroy(gameObject);
        }

    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, bool _canDuplicateClone, int _duplicationChancePercent, Player _player)
    {

        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        canDuplicateClone = _canDuplicateClone;
        duplicationChancePercent = _duplicationChancePercent;
        player = _player;
        FaceClosestTarget();
    }

     private void AnimationTrigger()
    {
        cloneTimer = -1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        
        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float damagePercentage = .3f;

                if (player.skill.clone.aggressiveMirageUnlocked)
                    damagePercentage = .8f;
            
                player.stats.DoDamage(hit.GetComponent<CharacterStats>(), true, damagePercentage);

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);
                
                if (player.skill.clone.aggressiveMirageUnlocked && weaponData != null)
                    weaponData.Effect(hit.transform);

                if(canDuplicateClone && Random.Range(0, 100) < duplicationChancePercent)
                {
                    SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1 * facingDir, 0));
                }

            }
        }
    }

    private void FaceClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
