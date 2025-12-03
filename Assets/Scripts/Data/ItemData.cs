using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public int id;
    public string itemName;
    public string itemDescription;
    public string itemEffectDescription;
    public Sprite icon;
    public bool isStackable=true;

    [Header("Item Effect")]
    public int healAmount = 0;
    public int restoreMpAmount = 0;
}
