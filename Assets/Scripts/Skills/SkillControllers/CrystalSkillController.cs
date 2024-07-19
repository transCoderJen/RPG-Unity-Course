using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    public Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalTimer;
    private bool canExplode;
    private bool canMoveToEnemy;
    private float moveSpeed;
    private bool canGrow;
    private float growSpeed;
    private float maxSize;
    private Transform targetEnemy;
    private bool enemyLocked;
    private bool canTeleport = true;
    private bool moveTowardsClosestEnemy;
    

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMoveToEnemy, float _moveSpeed, float _growSpeed, float _maxSize, bool _moveTowardsClosestEnemy)
    {
        crystalTimer = _crystalDuration;
        canExplode = _canExplode;
        canMoveToEnemy = _canMoveToEnemy;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        moveTowardsClosestEnemy = _moveTowardsClosestEnemy;
    }

    private void Update()
    {
        if (canMoveToEnemy)
            canTeleport = false;
        else
            canTeleport = true;

        crystalTimer -= Time.deltaTime;
        if (crystalTimer < 0)
        {
            FinishCrystal();
        }

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        if(moveTowardsClosestEnemy)
            FindClosestEnemyLogic();
        else
        {
            FindRandomEnemyLogic();
        }
    }

    private void FindRandomEnemyLogic()
    {
        if (canMoveToEnemy && !enemyLocked)
        {
            float radius = SkillManager.instance.blackhole.getBlackHoleSize();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            List<Transform> enemies = new List<Transform>();

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    enemies.Add(hit.transform);
                }
            }

            if (enemies.Count > 0)
            {
                int randomIndex = Random.Range(0, enemies.Count);
                targetEnemy = enemies[randomIndex];
                enemyLocked = true;
            }
        }

        if (enemyLocked)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            // Check for collision
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D enemyCollider = targetEnemy.GetComponent<Collider2D>();

            if (playerCollider.bounds.Intersects(enemyCollider.bounds))
            {
                canExplode = true;
                FinishCrystal();
            }
        }
    }

    private void FindClosestEnemyLogic()
    {

        if (canMoveToEnemy && !enemyLocked)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 150);
            float closestDistance = Mathf.Infinity;

            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        targetEnemy = hit.transform;
                    }
                }
            }

            if (targetEnemy != null)
                enemyLocked = true;
        }

        if (enemyLocked)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            // Check for collision
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D enemyCollider = targetEnemy.GetComponent<Collider2D>();

            if (playerCollider.bounds.Intersects(enemyCollider.bounds))
            {
                canExplode = true;
                FinishCrystal();
            }
        }
    }

    public bool canTeleportTo() => canTeleport;
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        
        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().DamageEffect();
        }
    }
    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            selfDestroy();
    }

    public void selfDestroy() => Destroy(gameObject);
}

