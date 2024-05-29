using System;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Buff,
    Equipable
}

public enum ConsumableType
{
    Health,
    Stamina
}

public enum BuffType
{
    Speed
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[Serializable]
public class ItemDataBuff
{
    public BuffType type;
    public float duration;
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

    [Header("Buff")]
    public ItemDataBuff[] buffs;
}