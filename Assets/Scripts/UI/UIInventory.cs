using UnityEngine;
using TMPro;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;

    private ItemData selectedItem;
    private int selectedItemIdx = 0;

    private int curEquipIdx;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelectedItemWindow();
    }

    private void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);

            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }

        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }

        return null;
    }

    private void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectedItem(int idx)
    {
        if (slots[idx].item == null)
        {
            return;
        }

        selectedItem = slots[idx].item;
        selectedItemIdx = idx;

        selectedItemName.text = selectedItem.itemName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable || selectedItem.type == ItemType.Buff);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[idx].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[idx].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;

                    case ConsumableType.Stamina:
                        condition.IncreaseStamina(selectedItem.consumables[i].value);
                        break;
                }
            }

            RemoveSelectedItem();
        }
        else if (selectedItem.type == ItemType.Buff)
        {
            for (int i = 0; i < selectedItem.buffs.Length; i++)
            {
                switch (selectedItem.buffs[i].type)
                {
                    case BuffType.Speed:
                        controller.SetSpeedBuff(selectedItem.buffs[i].duration, selectedItem.buffs[i].value);
                        break;
                }
            }

            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        slots[selectedItemIdx].quantity--;

        if (slots[selectedItemIdx].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIdx].item = null;
            selectedItemIdx = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    //public void OnEquipButton()
    //{
    //    if (slots[curEquipIdx].equipped)
    //    {
    //        UnEquip(curEquipIdx);
    //    }

    //    slots[selectedItemIdx].equipped = true;
    //    curEquipIdx = selectedItemIdx;
    //    CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
    //    UpdateUI();

    //    SelectedItem(selectedItemIdx);
    //}

    //private void UnEquip(int idx)
    //{
    //    slots[idx].equipped = false;
    //    CharacterManager.Instance.Player.equip.UnEquip();
    //    UpdateUI();

    //    if (selectedItemIdx == idx)
    //    {
    //        SelectedItem(selectedItemIdx);
    //    }
    //}

    //public void OnUnEquipButton()
    //{
    //    UnEquip(selectedItemIdx);
    //}
}
