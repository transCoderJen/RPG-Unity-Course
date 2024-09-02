using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage, bool _knockback = false)
    {
        if (_damage >= player.stats.GetMaxHealthValue() * .3f)
            _knockback = true;

        base.TakeDamage(_damage, _knockback);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    public override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);
        Inventory inventory = Inventory.instance;
        if (inventory.canUseArmor())
        {
            ItemData_Equipment armor =inventory.GetEquipment(EquipmentType.Armor);
            if (armor != null)
                armor.Effect(player.transform);
        }
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }
}

