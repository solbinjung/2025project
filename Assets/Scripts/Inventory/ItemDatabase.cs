using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> allItems;

    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 리스트에 있는 아이템들을 딕셔너리로 만들기
            foreach (ItemData item in allItems)
            {
                if (item != null && !itemDictionary.ContainsKey(item.name))
                {
                    itemDictionary.Add(item.name, item);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return null;

        ItemData itemData;
        if (itemDictionary.TryGetValue(itemName, out itemData))
        {
            return itemData;
        }

        Debug.LogWarning($"[ItemDatabase] '{itemName}' 이름을 가진 아이템을 찾을 수 없습니다!");
        return null;
    }
}
