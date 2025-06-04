using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDrawUIManager : MonoBehaviour
{
    public GameObject skillCardPrefab;
    public Transform skillCards;
    public List<SkillData> allSkills;
    private PlayerController player;

    private PlayerSkillManager playerSkillManager;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerSkillManager = FindObjectOfType<PlayerSkillManager>();
    }

    public void OpenSkillDrawPanel()
    {
        gameObject.SetActive(true);

        if (player != null)
            player.canControl = false;
        // 기존 카드 제거(중복 방지)
        foreach (Transform child in skillCards)
        {
            Destroy(child.gameObject);
        }
        // 랜덤 카드 생성
        for (int i = 0; i < 3; i++)
        {
            SkillData skill = allSkills[Random.Range(0, allSkills.Count)];
            GameObject card = Instantiate(skillCardPrefab, skillCards);
            card.GetComponent<SkillCardUI>().Setup(skill, OnSkillSelected);
        }
    }
    public void CloseSkillDrawPanel()
    {
        gameObject.SetActive(false);

        // 플레이어 움직임 다시 허용
        if (player != null)
            player.canControl = true;
    }
    void OnSkillSelected(SkillData selected)
    {
        Debug.Log("선택된 기술: " + selected.skillName);
        
        playerSkillManager.AddSkill(selected); // 기술 저장
        CloseSkillDrawPanel();
    }
}
