using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // 필드
    private readonly Queue<Action> _inputQueue = new Queue<Action>();

    [Header("Settings")]
    [SerializeField] private bool canCombat = true;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float dodgeDistance = 2f;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float blockDuration = 1.0f;
    [SerializeField] private int damage = 10;

    private bool isAttackActive = false;
    private bool isDodging = false;
    private bool isBlocking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private Vector3 dodgeDirection;

    private PlayerController playerController;
    private Animator animator;
    private CharacterStats stats;
    private MeleeHitBox hitbox;

    public enum PlayerState
    {
        Idle,
        Attacking,
        Dodging,
        Blocking
    }

    private PlayerState currentState = PlayerState.Idle;


    // 프로퍼티
    public bool CanCombat => canCombat;
    public float AttackCooldown => attackCooldown;
    public float DodgeDistance => dodgeDistance;
    public float DodgeDuration => dodgeDuration;
    public float BlockDuration => blockDuration;
    public int Damage => damage;
    public bool IsAttackActive => isAttackActive;
    public CharacterStats Stats => stats;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();

        if (hitbox != null)
            hitbox.Initialize(this);
    }

    private void Update()
    {
        if (!canCombat) return;

        // Idle 상태일 때만 입력 처리
        if (currentState == PlayerState.Idle && _inputQueue.Count > 0)
        {
            var nextAction = _inputQueue.Dequeue();
            nextAction?.Invoke();
            return;
        }

        if (currentState != PlayerState.Idle) return;

        HandleInput();
    }

    // 입력
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))                // 우클릭: 공격
            QueueAction(TryAttack);

        if (Input.GetKeyDown(KeyCode.Space))            // 스페이스: 회피
            QueueAction(() => StartCoroutine(Dodge()));

        if (Input.GetKeyDown(KeyCode.LeftShift))        // 좌쉬프트: 방어
            QueueAction(() => StartCoroutine(Block()));
    }

    private void QueueAction(Action action)
    {
        if (currentState == PlayerState.Idle)
        {
            action?.Invoke();
        }
        else if (_inputQueue.Count < 1)
        {
            _inputQueue.Enqueue(action);
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        currentState = PlayerState.Attacking;
        lastAttackTime = Time.time;

        animator.SetTrigger("Attack0");
        Debug.Log("기본 공격");

        StartCoroutine(ResetStateAfter(attackCooldown));
    }

    public void AttackStart()
    {
        isAttackActive = true;
        hitbox?.ResetHitCache();
    }

    public void AttackEnd()
    {
        isAttackActive = false;
    }

    private IEnumerator Dodge()
    {
        currentState = PlayerState.Dodging;
        isDodging = true;

        Vector3 cachedDirection = transform.forward;
        animator.SetTrigger("isDodging");

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

    private IEnumerator Block()
    {
        currentState = PlayerState.Blocking;
        isBlocking = true;

        animator.SetBool("isBlocking", true);
        Debug.Log("방어 시작");

        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        animator.SetBool("isBlocking", false);
        Debug.Log("방어 종료");

        currentState = PlayerState.Idle;
    }

    private IEnumerator ResetStateAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = PlayerState.Idle;
    }

    public bool IsBlocking() => isBlocking;
    public bool IsDodging() => isDodging;
}

