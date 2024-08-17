using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();

        if (entity != null)
            entity.onFlipped += FlipUI;
        if (myStats != null)
        {
            myStats.onHealthChanged += UpdateHealthUI;

            UpdateHealthUI();

        }
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }

    private void OnDisable()
    {
        if (entity != null)
            entity.onFlipped -= FlipUI;
        if (myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }
}
