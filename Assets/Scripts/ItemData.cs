using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemSystem/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemId;
    public string itemName;
    public string itemDescription;
    public string itemEffectDescription;
    public Sprite icon;

    public int maxStack;
}
