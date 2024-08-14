using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;


    public void ShowToolTip(string skillName, string skillDescription)
    {
        this.skillName.text = skillName;
        this.skillDescription.text = skillDescription;
        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
