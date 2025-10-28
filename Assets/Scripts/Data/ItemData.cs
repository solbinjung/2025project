using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemId;
    public string itemName;
    public string itemDescription;
    public string itemEffectDescription;
    public Sprite icon;

    public int maxStack;
    public bool isStackable=true;
}
