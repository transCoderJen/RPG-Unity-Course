using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Item Effect/Heal")]
public class Heal_Effect : ItemEffect
{
    [SerializeField] private  GameObject auraPrefab;
    [SerializeField] private GameObject healPrebaf;

    [Range(0f, 1f)]
    [SerializeField] private float healPercent;
    
    public override void ExecuteEffect(Transform _spawmnPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        playerStats.IncreaseHealthBy(healAmount);

        GameObject newAura = Instantiate(auraPrefab, _spawmnPosition.position, Quaternion.identity);
        GameObject newHeal = Instantiate(healPrebaf, new Vector3(_spawmnPosition.position.x, _spawmnPosition.position.y + 1), Quaternion.identity);
        Destroy(newAura, 2);
        Destroy(newHeal, 2);
    }
}
