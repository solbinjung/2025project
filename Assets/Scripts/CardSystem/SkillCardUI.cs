using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;

    private SkillData skillData;

    public void Setup(SkillData data, System.Action<SkillData> onSelect)
    {
        skillData = data;
        iconImage.sprite = data.icon;
        nameText.text = data.skillName;
        descriptionText.text = data.skillDescription;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect?.Invoke(skillData));
    }
}