using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    public bool canCombat = true;
    public float attackCooldown = 0.5f;
    public float dodgeDistance = 2f;
    public float dodgeDuration = 0.3f;
    public float blockDuration = 1.0f;

    private bool isDodging = false;
    private bool isBlocking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private Vector3 dodgeDirection;

    private PlayerController m_playerController;
    private Animator m_animator;

    void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canCombat || isDodging || isBlocking) return;

        HandleInput();
    }
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) // 우클릭: 공격
        {
            TryAttack();
        }
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스: 회피
        {
            StartCoroutine(Dodge());
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) // 좌측 쉬프트: 방어
        {
            StartCoroutine(Block());
        }
    }
    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack0");
        Debug.Log("기본 공격");
    }
    IEnumerator Dodge()
    {
        isDodging = true;
        dodgeDirection = transform.forward;

        m_animator.SetTrigger("isDodging");
        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            transform.position += dodgeDirection.normalized * (dodgeDistance / dodgeDuration) * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }
    IEnumerator Block()
    {
        isBlocking = true;
        m_animator.SetBool("isBlocking", true);
        Debug.Log("방어 시작");

        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        m_animator.SetBool("isBlocking", false);
        Debug.Log("방어 종료");
    }
    public bool IsBlocking() => isBlocking;
    public bool IsDodging() => isDodging;
}