using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager : MonoBehaviour
{
    public Dictionary<KeyCode, SkillData> skillMap = new();

    public Animator animator;

    // UI ���� �̹��� ����
    public Image QSlotImage;
    public Image WSlotImage;
    public Image ESlotImage;
    public Image RSlotImage;
    public Image TSlotImage;

    // Ű�� �ش��ϴ� ���� �̹��� ����
    private Dictionary<KeyCode, Image> keyToSlotImage;

    private void Awake()
    {
        keyToSlotImage = new Dictionary<KeyCode, Image>
        {
            { KeyCode.Q, QSlotImage },
            { KeyCode.W, WSlotImage },
            { KeyCode.E, ESlotImage },
            { KeyCode.R, RSlotImage },
            { KeyCode.T, TSlotImage }
        };
    }

    public void AddSkill(SkillData skill)
    {
        if (skillMap.ContainsValue(skill))
        {
            Debug.LogWarning($"{skill.skillName} ����� �̹� ��ϵǾ� �ֽ��ϴ�.");
            return;
        }

        var keys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        foreach (var key in keys)
        {
            if (!skillMap.ContainsKey(key))
            {
                skillMap[key] = skill;
                Debug.Log($"��� '{skill.skillName}' �� {key}�� ��ϵ�");

                // UI ���� �̹��� ����
                if (keyToSlotImage.TryGetValue(key, out var slotImage) && skill.icon != null)
                {
                    slotImage.sprite = skill.icon;
                    slotImage.color = Color.white; 
                }

                return;
            }
        }
    }
    public void UseSkill(KeyCode key)
    {
        if (skillMap.TryGetValue(key, out SkillData skill))
        {
            Debug.Log($"��� ���: {skill.skillName}");
            // �ִϸ��̼� ����
            if (animator && !string.IsNullOrEmpty(skill.animationTriggerName))
            {
                animator.SetTrigger(skill.animationTriggerName);
            }
        }
    }
}
