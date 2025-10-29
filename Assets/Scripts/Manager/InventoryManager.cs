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

    private PlayerStats _playerStats;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }
        if (_playerStats == null)
        {
            Debug.LogError("Error");
        }
    }

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
    public void SwapSlots(int indexA, int indexB)
    {
        // 1. 두 인덱스가 유효한지 확인
        if (indexA < 0 || indexA >= inventory.Count ||
            indexB < 0 || indexB >= inventory.Count)
        {
            return;
        }

        // 2. 데이터 교환
        ItemSlot temp = inventory[indexA];
        inventory[indexA] = inventory[indexB];
        inventory[indexB] = temp;

        // 3. UI 업데이트 방송! (인벤토리 + 핫바 동시 갱신)
        OnInventoryChanged?.Invoke();
    }
    public void UseItem(int slotIndex)
    {
        // 1. 슬롯 인덱스가 유효한지, 아이템이 있는지 확인
        if (slotIndex < 0 || slotIndex >= inventory.Count ||
            inventory[slotIndex] == null || inventory[slotIndex].item == null)
        {
            Debug.Log($"[Inventory] {slotIndex}번 슬롯은 비어있습니다.");
            return;
        }
        // 아이템 데이터 가져오기
        ItemData itemToUse = inventory[slotIndex].item;

        //아이템 타입 체크(if) 제거 -> 바로 효과 실행
        Debug.Log($"[Inventory] {itemToUse.itemName} 아이템 사용!");

        if (itemToUse.healAmount > 0) // 체력
        {
            _playerStats.Heal(itemToUse.healAmount);
            Debug.Log($"체력이 {itemToUse.healAmount} 회복되었습니다.");
        }
        else if (itemToUse.restoreMpAmount > 0) // 마나
        {
            _playerStats.RestoreMp(itemToUse.restoreMpAmount);
            Debug.Log($"마나가 {itemToUse.restoreMpAmount} 회복되었습니다.");
        }
        else
        {
            // 사용할 수 있지만 아무 효과가 없는 아이템일 경우
            Debug.Log($"[Inventory] {itemToUse.itemName} 아이템은 특별한 효과가 없습니다.");
        }
        
        // 5. 아이템 1개 소모
        RemoveItem(itemToUse, 1);
    }

}