using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private float returnSpeed;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;
    public bool canRotate = true;
    private bool isReturning;

    [Header("Pierce Info")]
    [SerializeField] private int pierceAmount;

    [Header("Bounce Info")]
    private float bounceSpeed;
    private float pauseDuration;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;
    private float freezeTimeDuration;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed, float _bounceSpeed)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        bounceSpeed = _bounceSpeed;

        anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -2, 2);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _pauseDuration)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        pauseDuration = _pauseDuration;

        enemyTarget = new List<Transform>();
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public bool getCanRotate() => canRotate;

    public void ReturnSword()
    {
        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                
                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>(), false);
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .05f)
            {
                // Stop bouncing to wait for the pause duration
                isBouncing = false;
                StartCoroutine(PauseBeforeNextTarget());
            }
        }
    }

    private IEnumerator PauseBeforeNextTarget()
    {
        SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>(), false);

        yield return new WaitForSeconds(pauseDuration);

        targetIndex++;
        bounceAmount--;

        if (bounceAmount <= 0)
        {
            isBouncing = false;
            isReturning = true;
        }
        else
        {
            if (targetIndex >= enemyTarget.Count)
            {
                targetIndex = 0;
            }

            isBouncing = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (!isBouncing || !isSpinning)
            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                SwordSkillDamage(enemy, false);
            }

        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy, bool knockback)
    {
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>(), knockback);

        if (player.skill.sword.timeStopUnlocked)
        {
            enemy.FreezeTimeFor(freezeTimeDuration);
            if (player.skill.sword.vulnerableUnlocked)
                enemy.stats.makeVulnerable(player.skill.sword.vulnerabilityPercentage, freezeTimeDuration);
        }

        Inventory.instance.GetEquipment(EquipmentType.Amulet)?.Effect(enemy.transform);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }
        
        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if(isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
