using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using System.Xml.Serialization;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; // 1 point increases damage by 1 and crit. power by 1%
    public Stat agility; // 1 point increases evasion and crit. chance by 1%
    public Stat intelligence; // 1 point increases magic damage and resistance by 3
    public Stat vitality; // 1 point increases health by 3

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower; // default value 150%

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited; // does damage over time
    public bool isChilled; // armor reduced by 20% 
    public bool isShocked; // reduce accuracy by 20%

    public float ailmentCooldown;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    [SerializeField] private GameObject thunderStrikePrefab;
    
    private float igniteDamageCooldown = .6f;
    private float igniteDamageTimer;
    private int igniteDamage;

    public int currentHealth;

    public System.Action onHealthChanged = delegate { };
    
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer <= 0)
            isIgnited = false;

        if (chilledTimer <=0)
            isChilled = false;

        if (shockedTimer <= 0)
            isShocked = false;

        if (igniteDamageTimer<= 0 && isIgnited)
        {
            DecreaseHealthBy(igniteDamage);
            igniteDamageTimer = igniteDamageCooldown;

            if (currentHealth < 0)
                Die();
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
        DoMagicDamage(_targetStats);
    }

    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= Math.Max(0, _targetStats.armor.GetValue());

        return totalDamage;
    }

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        if ((_fireDamage > 0 || _iceDamage > 0 || _lightningDamage > 0) && !canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            System.Random random = new System.Random();
            int choice = random.Next(3);

            if(choice == 0)
                canApplyIgnite = true;
            else if(choice == 1)
                canApplyChill = true;
            else
                canApplyShock = true;   
        }

        if(canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Math.Max(0, totalMagicalDamage);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return;

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentCooldown;

            fx.IgniteFxFor(ailmentCooldown);
        }

        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = ailmentCooldown;

            float _slowPercentage = .35f;
        
            GetComponent<Entity>().SlowEntityBy(_slowPercentage, ailmentCooldown);
            fx.ChillFxFor(ailmentCooldown);
        }

        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = ailmentCooldown;

            fx.ShockFxFor(ailmentCooldown);
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform);
            newThunderStrike.GetComponent<ThunderStrikeController>().Setup(1, this);
            
            List<CharacterStats> targetedEnemies = new List<CharacterStats>();

            targetedEnemies = FindClosestTargets();

            if (targetedEnemies != null)
            {

                if (targetedEnemies.Count > 0)
                {
                    for (int i = 0; i < targetedEnemies.Count; i++)
                    {
                        GameObject enemyThunderStrike = Instantiate(thunderStrikePrefab, transform);
                        enemyThunderStrike.GetComponent<ThunderStrikeController>().Setup(1, targetedEnemies[i]);
                    }
                }
            }
        }
    }

    private List<CharacterStats> FindClosestTargets()
    {
        if (GetComponent<Player>() != null)
            return null;
        // CharacterStats closestEnemy = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

        List<CharacterStats> enemies = new List<CharacterStats>();

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null && hit.gameObject != gameObject)
            {
                enemies.Add(hit.GetComponent<CharacterStats>());
                
            }
        }
        
        return enemies;
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        Debug.Log(_damage);

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        onHealthChanged();

    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;
    
    protected virtual void Die()
    {

    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCriticalPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float totalCriticalDamage = _damage * totalCriticalPower;

        return (int)totalCriticalDamage;
    }
}
