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

    // 슬롯 비우기
    public void Clear()
    {
        item = null;
        amount = 0;
    }

    // 데이터 덮어쓰기
    public void Set(ItemSlot slotData)
    {
        item = slotData.item;
        amount = slotData.amount;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public static event Action OnInventoryChanged;

    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 14; // UI 슬롯 개수

    // List를 고정 크기로 초기화
    public List<ItemSlot> inventory = new List<ItemSlot>();

    private PlayerStats _playerStats;
    
    void Start()
    {
        // 저장된 데이터가 있다면 인벤토리 복구
        if (SaveLoadManager.Instance.currentSaveData != null)
        {
            SaveLoadManager.Instance.ApplyInventoryHandler();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 인벤토리 리스트를 고정 크기로 초기화
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void InitializeInventory()
    {
        inventory = new List<ItemSlot>(inventorySize);
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new ItemSlot(null, 0)); // 빈 슬롯으로 초기화
        }
    }

    //PlayerStat 연결
    public void RegisterPlayerStats(PlayerStats stats)
    {
        _playerStats = stats;
    }
    // 씬이 파괴시 PlayerStats가 자신을 등록 해제
    public void UnregisterPlayerStats()
    {
        _playerStats = null;
    }
    // 아이템 추가
    public void AddItem(ItemData itemToAdd, int amountToAdd)
    {
        if (itemToAdd == null) return;

        int addedAmount = 0; // 추가된 양

        // 기존 슬롯 먼저 검색하여 동일 아이템 합치기
        if (itemToAdd.isStackable)
        {
            foreach (ItemSlot slot in inventory)
            {
                if (slot.item == itemToAdd)
                {
                    slot.amount += amountToAdd;
                    addedAmount = amountToAdd;
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }
        }
        // 기존 슬롯이 없으면 빈 슬롯 검색
        for (int j = 0; j < (itemToAdd.isStackable ? 1 : amountToAdd); j++)
        {
            int amountPerSlot = itemToAdd.isStackable ? amountToAdd : 1;

            bool foundSlot = false;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i].item == null) // 빈 슬롯 찾기
                {
                    inventory[i] = new ItemSlot(itemToAdd, amountPerSlot);
                    addedAmount += amountPerSlot;
                    foundSlot = true;
                    break;
                }
            }
            if (!foundSlot)
            {
                Debug.LogWarning("인벤토리 꽉 참!");
                break;
            }
        }

        if (addedAmount > 0) OnInventoryChanged?.Invoke();
    }

    // 사용자가 마우스로 직접 특정 칸을 조작할 경우 > 아이템 소모 등
    public void RemoveItemAt(int slotIndex, int amountToRemove)
    {
        if (slotIndex < 0 || slotIndex >= inventorySize || inventory[slotIndex].item == null)
            return;

        ItemSlot slotToRemove = inventory[slotIndex];
        if (slotToRemove != null)
        {
            slotToRemove.amount -= amountToRemove;
            if (slotToRemove.amount <= 0)
                slotToRemove.Clear();
        }
        OnInventoryChanged?.Invoke();
    }

    // 시스템이 자동으로 아이템을 차감할 경우
    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        ItemSlot slotToRemove = inventory.Find(slot => slot.item == itemToRemove);

        if (slotToRemove != null)
        {
            slotToRemove.amount -= amountToRemove;
            if (slotToRemove.amount <= 0)
            {
                slotToRemove.Clear();
            }
        }
        OnInventoryChanged?.Invoke();
    }

    // 아이템 얻기
    public bool HasItem(ItemData itemToFind, int requiredAmount = 1)
    {
        ItemSlot slot = inventory.Find(s => s.item == itemToFind);
        if (slot != null && slot.amount >= requiredAmount)
            return true;
        return false;
    }

    // 아이템을 슬롯 내에서 위치 옮기기
    public void SwapSlots(int indexA, int indexB)
    {
        // 두 인덱스가 inventorySize 범위 내에 있는지 확인
        if (indexA < 0 || indexA >= inventorySize || indexB < 0 || indexB >= inventorySize)
        {
            Debug.LogError($"인덱스 범위 오류");
            return;
        }
        // 데이터 교환
        ItemSlot temp = inventory[indexA];
        inventory[indexA] = inventory[indexB];
        inventory[indexB] = temp;

        // UI 업데이트
        OnInventoryChanged?.Invoke();
    }
    // 아이템 사용
    public void UseItem(int slotIndex)
    {
        if (_playerStats == null)
        {
            Debug.LogError("아이템을 사용할 수 없습니다");
            return;
        }
        // 슬롯 인덱스가 유효한지, 아이템이 있는지 확인
        if (slotIndex < 0 || slotIndex >= inventorySize || inventory[slotIndex] == null || inventory[slotIndex].item == null)
        {
            return;
        }

        ItemData itemToUse = inventory[slotIndex].item;
        Debug.Log($"{itemToUse.itemName} 아이템 사용");

        if (itemToUse.healAmount > 0) // 체력
        {
            _playerStats.Heal(itemToUse.healAmount);
        }
        else if (itemToUse.restoreMpAmount > 0) // 마나
        {
            _playerStats.RestoreMp(itemToUse.restoreMpAmount);
        }

            // 아이템 1개 소모
            RemoveItemAt(slotIndex, 1);
    }

    public void LoadInventory(List<int> loadedIDs, List<int> loadedAmounts, List<ItemData> itemDatabase)
    {
        // 기존 인벤토리 초기화
        inventory.Clear();

        // 저장된 개수만큼 반복하며 슬롯 재구축
        for (int i = 0; i < loadedIDs.Count; i++)
        {
            int id = loadedIDs[i];
            int amount = loadedAmounts[i];

            // 빈 슬롯(ID=-1)
            if (id == -1)
            {
                inventory.Add(new ItemSlot(null, 0));
            }
            else
            {
                // DB에서 ID로 원본 아이템 데이터 찾기
                ItemData data = itemDatabase.Find(x => x.id == id);

                if (data != null)
                {
                    inventory.Add(new ItemSlot(data, amount));
                }
                else
                {
                    // ID는 있는데 DB에 없으면 빈 슬롯 처리
                    inventory.Add(new ItemSlot(null, 0));
                }
            }
        }

        // 3. UI 강제 갱신
        OnInventoryChanged?.Invoke();
    }
}