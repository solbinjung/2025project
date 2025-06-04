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
        // ���� ī�� ����(�ߺ� ����)
        foreach (Transform child in skillCards)
        {
            Destroy(child.gameObject);
        }
        // ���� ī�� ����
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

        // �÷��̾� ������ �ٽ� ���
        if (player != null)
            player.canControl = true;
    }
    void OnSkillSelected(SkillData selected)
    {
        Debug.Log("���õ� ���: " + selected.skillName);
        
        playerSkillManager.AddSkill(selected); // ��� ����
        CloseSkillDrawPanel();
    }
}
