using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager Instance { get; private set; }

    public Dictionary<KeyCode, SkillData> skillMap = new();

    public List<SkillData> ownedSkills = new List<SkillData>();
    public static event Action<int> OnOwnedSkillsChanged;

    [Header("References")]
    public Animator animator;
    public Transform effectPoint;

    [Header("UI Slots")]
    public Image QSlotImage;
    public Image WSlotImage;
    public Image ESlotImage;
    public Image RSlotImage;
    public Image TSlotImage;

    [Header("Cooldown Overlays")]
    public Image QCooldownOverlay;
    public Image WCooldownOverlay;
    public Image ECooldownOverlay;
    public Image RCooldownOverlay;
    public Image TCooldownOverlay;

    // 슬롯 매핑
    private Dictionary<KeyCode, Image> keyToSlotImage;
    private Dictionary<KeyCode, Image> keyToCooldownOverlay;

    // 각 키의 쿨다운 종료 시간 저장
    private Dictionary<KeyCode, float> cooldownTimers = new();

    private PlayerStats _playerStats;
    
    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        keyToSlotImage = new Dictionary<KeyCode, Image>
        {
            { KeyCode.Q, QSlotImage },
            { KeyCode.W, WSlotImage },
            { KeyCode.E, ESlotImage },
            { KeyCode.R, RSlotImage },
            { KeyCode.T, TSlotImage }
        };

        keyToCooldownOverlay = new Dictionary<KeyCode, Image>
        {
            { KeyCode.Q, QCooldownOverlay },
            { KeyCode.W, WCooldownOverlay },
            { KeyCode.E, ECooldownOverlay },
            { KeyCode.R, RCooldownOverlay },
            { KeyCode.T, TCooldownOverlay }
        };

        // 시작할 때 쿨다운 오버레이는 다 꺼진 상태로
        foreach (var overlay in keyToCooldownOverlay.Values)
        {
            overlay.fillAmount = 0;
        }
    }

    // 새 게임
    public void ResetSkills()
    {
        Debug.Log("PlayerSkillManager: Resetting all skills and UI...");

        // 모든 스킬 데이터 초기화
        ownedSkills.Clear();
        skillMap.Clear();
        cooldownTimers.Clear();

        // 핫바 UI 이미지 초기화 (아이콘 제거)
        foreach (var key in keyToSlotImage.Keys)
        {
            if (keyToSlotImage[key] != null)
            {
                keyToSlotImage[key].sprite = null;
                keyToSlotImage[key].enabled = false; // 아이콘 이미지 자체를 비활성화
            }
        }

        // 쿨다운 UI 초기화
        foreach (var key in keyToCooldownOverlay.Keys)
        {
            if (keyToCooldownOverlay[key] != null)
            {
                keyToCooldownOverlay[key].fillAmount = 0;
            }
        }
    }

    public void AddSkill(SkillData skill)
    {

        if (ownedSkills.Contains(skill))
        {
            Debug.LogWarning($"{skill.skillName} 스킬은 이미 보유 중입니다.");
            return;
        }

        ownedSkills.Add(skill);

        OnOwnedSkillsChanged?.Invoke(ownedSkills.Count);


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
                }
                return; 
            }
        }
        Debug.LogWarning($"보유 스킬 {skill.skillName} 추가. (핫바가 꽉 차서 장착은 못함)");
    }

    public void UseSkill(KeyCode key)
    {
        if (skillMap.TryGetValue(key, out SkillData skill))
        {
            var stats = GetComponent<PlayerStats>();

            // 쿨다운
            if (cooldownTimers.TryGetValue(key, out float readyTime))
            {
                if (Time.time < readyTime)
                {
                    float remain = readyTime - Time.time;
                    Debug.Log($"[{skill.skillName}] 아직 쿨다운 중... {remain:F1}초 남음");
                    return;
                }
            }

            // 마나 소비
            if (stats.CurrentMp < skill.mpCost)
            {
                Debug.Log("마나가 부족합니다!");
                return;
            }
            _playerStats.CostMp(skill.mpCost);

            Debug.Log($"기술 사용: {skill.skillName}");

            // 애니메이션 실행
            if (animator && !string.IsNullOrEmpty(skill.animationTriggerName))
            {
                animator.SetTrigger(skill.animationTriggerName);
            }

            // 기술 이펙트
            PlayEffect(skill);

            // 데미지 처리
            ApplySkillDamage(skill);

            // 쿨다운 등록
            cooldownTimers[key] = Time.time + skill.skillCooldown;

            // 오버레이 시작
            if (keyToCooldownOverlay.TryGetValue(key, out var overlay))
            {
                overlay.fillAmount = 1;
            }
        }
    }

    private void Update()
    {
        // 쿨다운 오버레이 갱신
        foreach (var pair in skillMap)
        {
            KeyCode key = pair.Key;
            SkillData skill = pair.Value;

            if (cooldownTimers.TryGetValue(key, out float readyTime))
            {
                if (keyToCooldownOverlay.TryGetValue(key, out var overlay))
                {
                    float remain = readyTime - Time.time;
                    if (remain > 0)
                    {
                        overlay.fillAmount = remain / skill.skillCooldown; // 1→0 감소
                    }
                    else
                    {
                        overlay.fillAmount = 0; // 쿨 종료
                    }
                }
            }
        }
    }
    private void ApplySkillDamage(SkillData skill)
    {
        // 공격 범위 내 적 찾기
        Collider[] hits = Physics.OverlapSphere(transform.position, skill.attackRange);
        foreach (Collider hit in hits)
        {
            EnemyStats enemyStats = hit.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                Vector3 hitDir = (enemyStats.transform.position - transform.position).normalized;
                enemyStats.TakeDamage(skill.damage, hitDir);
            }
        }
    }
    private void PlayEffect(SkillData skill)
    {
        if (skill.effectPrefab != null && effectPoint != null)
        {
            GameObject effect = Instantiate(skill.effectPrefab, effectPoint.position, effectPoint.rotation);
            effect.transform.forward = transform.forward;
            Destroy(effect, 1f);
        }
    }
}

