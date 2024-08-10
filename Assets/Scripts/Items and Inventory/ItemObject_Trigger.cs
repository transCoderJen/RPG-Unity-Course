using System.Collections;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();
    private bool isTriggerActive = false;
    private float delayTime = 1.0f;

    private void Start()
    {
        StartCoroutine(ActivateTriggerAfterDelay());
    }

    private IEnumerator ActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        isTriggerActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggerActive && collision.GetComponent<Player>() != null)
        {
            if (collision.GetComponent<CharacterStats>().isDead)
                return;
                
            myItemObject.PickupItem();
        }
    }
}
