using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI statDescription;
    public void ShowToolTip(StatType statType)
    {
        if (statType == StatType.strength)
            statDescription.text = "Strength: 1 point increases damage by 1 and crit. power by 1%.";
        else if (statType == StatType.agility)
            statDescription.text = "Agility: 1 point increases evasion and crit. chance by 1%.";
        else if (statType == StatType.intelligence)
            statDescription.text = "Intelligence: 1 point increases magic damage and resistance by 3.";
        else if (statType == StatType.vitality)
            statDescription.text = "Vitality: 1 point increases health by 5.";
        else if (statType == StatType.damage)
            statDescription.text = "Damage: Determines the amount of damage dealt to enemies.";
        else if (statType == StatType.critChance)
            statDescription.text = "Crit Chance: The probability of landing a critical hit.";
        else if (statType == StatType.critPower)
            statDescription.text = "Crit Power: Increases the damage multiplier on critical hits. Default value is 150%.";
        else if (statType == StatType.health)
            statDescription.text = "Max Health: The total amount of health points a character can have.";
        else if (statType == StatType.armor)
            statDescription.text = "Armor: Reduces physical damage taken from attacks.";
        else if (statType == StatType.evasion)
            statDescription.text = "Evasion: The chance to avoid incoming physical attacks.";
        else if (statType == StatType.magicRes)
            statDescription.text = "Magic Resistance: Reduces magic damage taken from spells.";
        else if (statType == StatType.fireDamage)
            statDescription.text = "Fire Damage: Deals damage over time to the target.\n\n*Only the highest elemental value is applied";
        else if (statType == StatType.iceDamage)
            statDescription.text = "Ice Damage: Reduces the target's armor by 20% and deals damage.\n\n*Only the highest elemental value is applied";
        else if (statType == StatType.lightningDamage)
            statDescription.text = "Lightning Damage: Reduces the target's accuracy by 20% and deals damage.\n\n*Only the highest elemental value is applied";
        
        AdjustPosition();
        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);


}
