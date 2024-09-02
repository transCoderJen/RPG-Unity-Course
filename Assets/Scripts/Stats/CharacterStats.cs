using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using System.Xml.Serialization;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using IEnumerator = System.Collections.IEnumerator;
using UnityEngine.Audio;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality, 
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightningDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; // 1 point increases damage by 1 and crit. power by 1%
    public Stat agility; // 1 point increases evasion and crit. chance by 1%
    public Stat intelligence; // 1 point increases magic damage and resistance by 3
    public Stat vitality; // 1 point increases health by 5

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
    public Stat fireDamage;  // does damage over time
    public Stat iceDamage; // also reduces targets armor by 20% 
    public Stat lightningDamage; // also reduces targets accuracy by 20%

    public bool isIgnited; // does damage over time
    public bool isChilled; // armor reduced by 20% 
    public bool isShocked; // reduce accuracy by 20%

    public float ailmentCooldown;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    [SerializeField] private GameObject shockStrikePrefab;
    
    private float igniteDamageCooldown = .6f;
    private float igniteDamageTimer;
    private int igniteDamage;

    public int currentHealth;

    public System.Action onHealthChanged = delegate { };
    public bool isDead { get; private set; }
    public bool vulnerable;
    private float vulnerabilityAmount;

    AudioSource burningAudio;
    public AudioMixerGroup soundEffectsGroup;
    public float fadeOutDuration = 1.0f;
    
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
        
        burningAudio = gameObject.AddComponent<AudioSource>();
        burningAudio.clip = AudioManager.instance.getSFXAudioSource(SFXSounds.burning).clip;
        burningAudio.outputAudioMixerGroup = soundEffectsGroup;
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer <= 0)
        {
            isIgnited = false;
            burningAudio.Stop();
        }

        if (chilledTimer <= 0)
            isChilled = false;

        if (shockedTimer <= 0)
            isShocked = false;
        
        if (isIgnited)
            ApplyIgniteDamage();

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
    }

    public void makeVulnerable(float _amount, float _duration)
    {
        vulnerabilityAmount = _amount;
        StartCoroutine(VulnereableForCoroutine(_duration));
    }

    private IEnumerator VulnereableForCoroutine(float _duration)
    {
        vulnerable = true;
        yield return new WaitForSeconds(_duration);
        vulnerable = false;
        vulnerabilityAmount = 0f;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer <= 0)
        {
            DecreaseHealthBy(igniteDamage);
            igniteDamageTimer = igniteDamageCooldown;
            if (currentHealth < 0 && !isDead)
            {
                burningAudio.Stop();
                Die();
            }
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats, bool _knockback, float _damagePercentage = 1f)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = Mathf.RoundToInt(GetTotalDamage() * _damagePercentage);

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage, _knockback);
        DoMagicDamage(_targetStats, _knockback);
    }

    public int GetTotalDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    public virtual void TakeDamage(int _damage, bool _knockback)
    {
        GetComponent<Entity>().DamageEffect(_knockback);
        DecreaseHealthBy(_damage);

        if (currentHealth <= 0 && !isDead)
            Die();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        Math.Min(GetMaxHealthValue(), currentHealth += _amount);
        if(onHealthChanged != null)
            onHealthChanged();
    }
    
    public virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= Mathf.RoundToInt(_damage * (1 + vulnerabilityAmount));

        onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Magical Damage and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats, bool knockback)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage, knockback);
        
        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        if ((_fireDamage > 0 || _iceDamage > 0 || _lightningDamage > 0) && !canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            System.Random random = new System.Random();
            int choice = random.Next(3);

            if (choice == 0)
                canApplyIgnite = true;
            else if (choice == 1)
                canApplyChill = true;
            else
                canApplyShock = true;
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return;

        if (_ignite)
        {
            burningAudio.Play();
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
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform);
            newShockStrike.GetComponent<ShockStrikeController>().Setup(1, this);
            
            List<CharacterStats> targetedEnemies = new List<CharacterStats>();

            targetedEnemies = FindClosestTargets();

            if (targetedEnemies != null)
            {

                if (targetedEnemies.Count > 0)
                {
                    for (int i = 0; i < targetedEnemies.Count; i++)
                    {
                        GameObject enemyShockStrike = Instantiate(shockStrikePrefab, transform);
                        enemyShockStrike.GetComponent<ShockStrikeController>().Setup(1, targetedEnemies[i]);
                    }
                }
            }
        }
    }

    private List<CharacterStats> FindClosestTargets()
    {
        if (GetComponent<Player>() != null)
            return null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

        List<CharacterStats> enemies = new List<CharacterStats>();

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null && hit.gameObject != gameObject)
                enemies.Add(hit.GetComponent<CharacterStats>());
        }

        return enemies;
    }
    #endregion

    #region Stat Calculations
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Math.Max(0, totalMagicalDamage);
        return totalMagicalDamage;
    }

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        
        if(_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= Math.Max(0, _targetStats.armor.GetValue());

        return totalDamage;
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;
    
    public virtual void OnEvasion()
    {

    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
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
    #endregion

    public Stat getStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.health:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicRes:
                return magicResistance;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightningDamage:
                return lightningDamage;
            default:
                throw new System.ArgumentException("Invalid stat type");
        }
    }
}
