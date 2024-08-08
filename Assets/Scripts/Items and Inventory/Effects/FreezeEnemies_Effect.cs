using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze Enemies Effect", menuName = "Data/Item Effect/Freeze Enemies")]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _spawnPosition)
    {
        PlayerStats stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (stats.currentHealth > stats.GetMaxHealthValue() * .1f)
            return;
    
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_spawnPosition.position, 2);
        
        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().FreezeTimeFor(duration);
            }
        }
    }
}
