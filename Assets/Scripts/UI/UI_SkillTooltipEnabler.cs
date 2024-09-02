using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillTooltipEnabler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected UI ui;
    private int skillPrice;
    private string skillName;
    private string skillDescription;

    private void Awake()
    {
        skillPrice = GetComponentInChildren<UI_SkillTreeSlot>().GetSkillPrice();
        skillName = GetComponentInChildren<UI_SkillTreeSlot>().GetSkillName();
        skillDescription = GetComponentInChildren<UI_SkillTreeSlot>().GetSkillDescription();
        ui = GetComponentInParent<UI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillName, skillDescription, skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }

}
