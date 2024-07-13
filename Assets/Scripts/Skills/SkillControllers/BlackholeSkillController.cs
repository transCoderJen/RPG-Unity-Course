using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public bool canGrow;
    public bool canShrink;
    public float shrinkSpeed;

    private bool canCreateHotkeys = true;
    private bool cloneAttackInitiated;
    public int amountOfAttacks = 4;
    public float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotkeys = new List<GameObject>();


    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.R))
        {
            cloneAttackInitiated = true;
            canCreateHotkeys = false;
            DestroyHotHeys();
        }

        if (cloneAttackTimer < 0 && cloneAttackInitiated && targets.Count > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);
            
            float xOffset;

            if (Random.Range(0,100) < 50)
                xOffset = 1;
            else
                xOffset = -1;
            
            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                canShrink = true;
                cloneAttackInitiated = false;
            }
        }
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            // targets.Add(collision.transform);
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHoykey(collision);
        }
    }

    private void DestroyHotHeys()
    {
        if(createdHotkeys.Count >0)
        {
            foreach (var hotkey in createdHotkeys)
            {
                Destroy(hotkey);
            }
        }
    }
    private void CreateHoykey(Collider2D collision)
    {
        if(keyCodeList.Count <= 0 || !canCreateHotkeys)
            return;

        GameObject newHotkey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);

        createdHotkeys.Add(newHotkey);

        KeyCode chosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chosenKey);

        BlackholeHotkeyController newHotkeyScript = newHotkey.GetComponent<BlackholeHotkeyController>();
        newHotkeyScript.SetupHotkey(chosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
