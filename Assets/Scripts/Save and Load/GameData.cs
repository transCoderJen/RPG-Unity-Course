using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;
    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;
    public string lastCheckpointId;
    

    public int lostCurrencyAmount;
    public float lostCurrencyX;
    public float lostCurrencyY;

    public SerializableDictionary<string, bool> checkpoints;

    public GameData()
    {
        this.lostCurrencyAmount = 0;
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        lastCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
    }
}
