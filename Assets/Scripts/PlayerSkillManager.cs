using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager : MonoBehaviour
{
    public Dictionary<KeyCode, SkillData> skillMap = new();

    public Animator animator;

    // UI 슬롯 이미지 매핑
    public Image QSlotImage;
    public Image WSlotImage;
    public Image ESlotImage;
    public Image RSlotImage;
    public Image TSlotImage;

    // 키에 해당하는 슬롯 이미지 매핑
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
            Debug.LogWarning($"{skill.skillName} 기술이 이미 등록되어 있습니다.");
            return;
        }

        var keys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        foreach (var key in keys)
        {
            if (!skillMap.ContainsKey(key))
            {
                skillMap[key] = skill;
                Debug.Log($"기술 '{skill.skillName}' 이 {key}에 등록됨");

                // UI 슬롯 이미지 갱신
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
            Debug.Log($"기술 사용: {skill.skillName}");
            // 애니메이션 실행
            if (animator && !string.IsNullOrEmpty(skill.animationTriggerName))
            {
                animator.SetTrigger(skill.animationTriggerName);
            }
        }
    }
}
