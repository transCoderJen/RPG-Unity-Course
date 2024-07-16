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
    private Transform closestEnemy;
    private bool enemyLocked;
    private bool canTeleport = true;
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMoveToEnemy, float _moveSpeed, float _growSpeed, float _maxSize)
    {
        crystalTimer = _crystalDuration;
        canExplode = _canExplode;
        canMoveToEnemy = _canMoveToEnemy;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
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
        
        if (canMoveToEnemy && !enemyLocked)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 150);
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
                enemyLocked = true;
        }

        if (enemyLocked)
        { 
            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed * Time.deltaTime);
            // Check for collision
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D enemyCollider = closestEnemy.GetComponent<Collider2D>();

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
                hit.GetComponent<Enemy>().Damage();
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

