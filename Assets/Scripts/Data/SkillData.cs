using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SkillData", menuName = "CardSystem/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public int id;
    public string skillName;
    public string skillDescription;
    public string skillEffectDescription;
    public Sprite icon;

    [Header("Animation")]
    public string animationTriggerName;
    public GameObject effectPrefab;

    [Header("Stats")]
    public int damage;
    public int mpCost;
    public float skillCooldown;
    public float attackRange;
}
