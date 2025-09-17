using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "CardSystem/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public string skillName;
    public string skillDescription;
    public Sprite icon;

    [Header("Animation")]
    public string animationTriggerName;
    public GameObject effectPrefab;

    [Header("Stats")]
    public int damage;
    public int mpCost; 
}
