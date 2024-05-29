using System;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipable
}

public enum ConsumableType
{
    Health,
    Stamina,
    Speed
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("info")]
    public ItemType type;
    public GameObject dropPrefab;
    public Sprite icon;
    public string itemName;
    public string description;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}