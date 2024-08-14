using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected UI ui;
    private Image skillImage;

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;


    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    [SerializeField] private UI_SkillTreeSlot[] unlocks;
    [SerializeField] private UI_SkillTreeSlot[] locks;


    public event Action OnFullyFilled;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
        transform.parent.gameObject.name = skillName;
        skillImage = transform.parent.gameObject.GetComponent<Image>();
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage.color = lockedSkillColor;


        ui = GetComponentInParent<UI>();
    }

    public void UnlockSkillSlot()
    {
        if (!PlayerManager.instance.HaveEnoughMoney(skillPrice))
            return;
            
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        OnFullyFilled?.Invoke();

        skillImage.color = Color.white;
        // GetComponentInChildren<UI_SkillTreeSlot>().gameObject.SetActive(false);

        for (int i = 0; i < unlocks.Length; i++)
        {
            unlocks[i].gameObject.SetActive(true);
        }

         for (int i = 0; i < locks.Length; i++)
        {
            locks[i].gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillName, skillDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }
}
