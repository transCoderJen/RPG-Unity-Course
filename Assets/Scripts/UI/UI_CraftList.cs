using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;
    [SerializeField] private List<ItemData_Equipment> craftArmor;
    [SerializeField] private List<ItemData_Equipment> craftAmulet;
    [SerializeField] private List<ItemData_Equipment> craftFlask;
    [SerializeField] private UI_CraftWindow craftWIndow;

 
    void Start()
    {
        SetupCraftList();
    }


    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);

            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData);
        SetupCraftList();
        craftWIndow.SetupCraftWindow(craftEquipment[0]);
    }
}
