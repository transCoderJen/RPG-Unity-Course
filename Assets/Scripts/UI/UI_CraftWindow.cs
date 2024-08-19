using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;
    public ItemData_Equipment defaultItem;
    private ItemData_Equipment selectedItem;

    private void Start()
    {
        SetupCraftWindow(defaultItem);
        selectedItem = defaultItem;
    }

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        selectedItem = _data;
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            materialImage[i].sprite = _data.craftingMaterials[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();
            
            materialSlotText.text= _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;

        }

        itemIcon.sprite = _data.icon;
        itemName.text = _data.itemName;

        itemDescription.text = _data.GetDescription();

        
    }

    public void CraftItem()
    {
        Inventory.instance.CanCraft(selectedItem, selectedItem.craftingMaterials);
    }
}
