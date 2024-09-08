using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShockStrikeController : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();

    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;


    }


    void Update()
    {
        if (triggered || !targetStats)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation = Quaternion.identity;

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            triggered = true;
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySFX(SFXSounds.thunderStrike, null);

            Invoke("DamageAndSelfDestroy", .2f);

        }
    }

    private void DamageAndSelfDestroy()
    {
        bool knockback = true;

        if (targetStats.GetComponent<Player>() != null)
            knockback = false;
             
        targetStats.TakeDamage(damage, knockback);
        Destroy(gameObject, .4f);
    }
}
