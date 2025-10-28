using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int amount;

    // 생성자
    public ItemSlot(ItemData _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
}

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public static event Action OnInventoryChanged;

    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 14; // UI 슬롯 개수와 동일

    public List<ItemSlot> inventory = new List<ItemSlot>();


    public void AddItem(ItemData itemToAdd, int amountToAdd)
    {
        if (itemToAdd.isStackable)
        {
            ItemSlot existingSlot = inventory.Find(slot => slot.item == itemToAdd);
            if (existingSlot != null)
            {
                existingSlot.amount += amountToAdd;
            }
            else
            {
                if (inventory.Count < inventorySize)
                    inventory.Add(new ItemSlot(itemToAdd, amountToAdd));
                else
                    Debug.LogWarning("인벤토리 꽉 참!");
            }
        }
        else
        {
            if (inventory.Count < inventorySize)
                for (int i = 0; i < amountToAdd; i++)
                    inventory.Add(new ItemSlot(itemToAdd, 1));
            else
                Debug.LogWarning("인벤토리 꽉 참!");
        }

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        ItemSlot slotToRemove = inventory.Find(slot => slot.item == itemToRemove);
        if (slotToRemove != null)
        {
            slotToRemove.amount -= amountToRemove;
            if (slotToRemove.amount <= 0)
                inventory.Remove(slotToRemove);
        }
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemData itemToFind, int requiredAmount = 1)
    {
        ItemSlot slot = inventory.Find(s => s.item == itemToFind);
        if (slot != null && slot.amount >= requiredAmount)
            return true;

        return false;
    }
}