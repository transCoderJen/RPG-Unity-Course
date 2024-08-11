using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldToEnableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float holdTime = 2.0f;  // Time in seconds to hold the click
    private float holdDuration = 0.0f;
    private bool isHolding = false;

    private Button button;
    public Image fillImage;  // Reference to the fill image

    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;  // Initially disable the button

        if (fillImage != null)
        {
            fillImage.fillAmount = 0.0f;  // Start with the fill amount at 0
        }
    }

    void Update()
    {
        if (isHolding)
        {
            holdDuration += Time.deltaTime;

            if (fillImage != null)
            {
                fillImage.fillAmount = holdDuration / holdTime;  // Update the fill amount
            }

            if (holdDuration >= holdTime)
            {
                button.interactable = true;
                isHolding = false;  // Stop holding after enabling the button
            }
        }
        else
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = 0.0f;  // Reset the fill amount when not holding
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdDuration = 0.0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdDuration = 0.0f;
    }
}
