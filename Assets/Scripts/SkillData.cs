using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "CardSystem/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite icon;
    public string animationTriggerName;
}
