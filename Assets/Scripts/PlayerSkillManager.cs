using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public Dictionary<KeyCode, SkillData> skillMap = new();

    public void AddSkill(SkillData skill)
    {
        if (skillMap.ContainsValue(skill))
        {
            Debug.LogWarning($"{skill.skillName} 기술이 이미 등록되어 있습니다.");
            return;
        }
        // 첫 빈 슬롯에 기술 할당 (Q부터 순서대로)
        var keys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        foreach (var key in keys)
        {
            if (!skillMap.ContainsKey(key))
            {
                skillMap[key] = skill;
                Debug.Log($"기술 '{skill.skillName}' 이 {key}에 등록됨");
                return;
            }
        }
    }

    public void UseSkill(KeyCode key)
    {
        if (skillMap.TryGetValue(key, out SkillData skill))
        {
            Debug.Log($"기술 사용: {skill.skillName}");
            // 추후 애니메이션, 효과 연결
        }
    }
}
