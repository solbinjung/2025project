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
            Debug.LogWarning($"{skill.skillName} ����� �̹� ��ϵǾ� �ֽ��ϴ�.");
            return;
        }
        // ù �� ���Կ� ��� �Ҵ� (Q���� �������)
        var keys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        foreach (var key in keys)
        {
            if (!skillMap.ContainsKey(key))
            {
                skillMap[key] = skill;
                Debug.Log($"��� '{skill.skillName}' �� {key}�� ��ϵ�");
                return;
            }
        }
    }

    public void UseSkill(KeyCode key)
    {
        if (skillMap.TryGetValue(key, out SkillData skill))
        {
            Debug.Log($"��� ���: {skill.skillName}");
            // ���� �ִϸ��̼�, ȿ�� ����
        }
    }
}
