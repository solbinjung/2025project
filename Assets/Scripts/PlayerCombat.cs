using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    private Queue<Action> inputQueue = new Queue<Action>();

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

    public enum PlayerState // 행동 중첩 방지
    {
        Idle,
        Attacking,
        Dodging,
        Blocking
    }
    
    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canCombat) return;

        // 상태가 Idle이면 입력 큐 먼저 확인
        if (currentState == PlayerState.Idle && inputQueue.Count > 0)
        {
            var nextAction = inputQueue.Dequeue();
            nextAction?.Invoke();
            return;
        }

        if (currentState != PlayerState.Idle) return;

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1)) // 우클릭: 공격
        {
            QueueAction(TryAttack);
        }
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스: 회피
        {
            QueueAction(() => StartCoroutine(Dodge()));
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) // 좌쉬프트: 방어
        {
            QueueAction(() => StartCoroutine(Block()));
        }
    }
    void QueueAction(Action action)
    {
        if (currentState == PlayerState.Idle)
            action?.Invoke();
        else if (inputQueue.Count < 1)  // 이미 하나 대기 중이면 더 안 넣음
            inputQueue.Enqueue(action);
    }
    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        currentState = PlayerState.Attacking;
        lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack0");
        Debug.Log("기본 공격");

        StartCoroutine(ResetStateAfter(attackCooldown)); // 쿨타임 동안 Idle 전환 대기
    }
    IEnumerator Dodge() 
    {
        currentState = PlayerState.Dodging;
        isDodging = true;
        Vector3 cachedDirection = transform.forward; // 캐릭터 회전이 변하기 전에 방향을 캐싱
        m_animator.SetTrigger("isDodging");

        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            transform.position += dodgeDirection.normalized * (dodgeDistance / dodgeDuration) * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
        currentState = PlayerState.Idle;
    }
    IEnumerator Block()
    {
        currentState = PlayerState.Blocking;
        isBlocking = true;
        m_animator.SetBool("isBlocking", true);
        Debug.Log("방어 시작");

        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        m_animator.SetBool("isBlocking", false);
        Debug.Log("방어 종료");

        currentState = PlayerState.Idle;
    }
    IEnumerator ResetStateAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = PlayerState.Idle;
    }
    public bool IsBlocking() => isBlocking;
    public bool IsDodging() => isDodging;
}