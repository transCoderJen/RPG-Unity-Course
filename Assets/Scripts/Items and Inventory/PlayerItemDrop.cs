using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's Drop")]

    [Range(0,100)]
    [SerializeField] private float chanceToLooseEquipment;

    [Range(0,100)]
    [SerializeField] private float chanceToLooseMaterials;

    [Range(0,100)]
    [SerializeField] private float chanceToLooseInventory;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();
        List<InventoryItem> inventoryToLoose = new List<InventoryItem>();

        // Lose Inventory
        foreach(InventoryItem item in inventory.GetInventoryList())
        {
            if (Random.Range(1, 101) <= chanceToLooseInventory)
            {
                DropItem(item.data);
                inventoryToLoose.Add(item);
            }
        }

        for (int i = 0; i < inventoryToLoose.Count; i++)
        {
            inventory.RemoveItem(inventoryToLoose[i].data);
        }

        // Lose Equipment
        foreach(InventoryItem item in inventory.GetEquipmentList())
        {
            if (Random.Range(1, 101) <= chanceToLooseEquipment)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        // Lose stash materials
        foreach(InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(1, 101) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }
    }
}
