using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // 필드
    private readonly Queue<Action> _inputQueue = new Queue<Action>();

    [Header("Settings")]
    [SerializeField] private bool _canCombat = true;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private MeleeHitBox _hitbox;

    [SerializeField] private float _dodgeDistance = 2f;
    [SerializeField] private float _dodgeDuration = 0.3f;
    [SerializeField] private float _blockDuration = 1.0f;

    [SerializeField] private GameObject slashEffectPrefab;  
    [SerializeField] private Transform effectPoint;

    private bool _isAttackActive = false;
    private bool _isDodging = false;
    private bool _isBlocking = false;
    private float _lastAttackTime = -Mathf.Infinity;
    private Vector3 _dodgeDirection;

    private PlayerController _playerController;
    private Animator _animator;
    private EnemyStats _stats;
    
    public enum PlayerState
    {
        Idle,
        Attacking,
        Dodging,
        Blocking
    }

    private PlayerState CurrentState = PlayerState.Idle;

    // 프로퍼티
    public bool CanCombat => _canCombat;
    public float AttackCooldown => _attackCooldown;
    public float DodgeDistance => _dodgeDistance;
    public float DodgeDuration => _dodgeDuration;
    public float BlockDuration => _blockDuration;
    public int Damage => _damage;
    public bool IsAttackActive => _isAttackActive;
    public EnemyStats Stats => _stats;
    public PlayerState State => CurrentState;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _stats = GetComponent<EnemyStats>();

        if (_hitbox == null)
            _hitbox = GetComponentInChildren<MeleeHitBox>();

        if (_hitbox != null)
            _hitbox.Initialize(this);
        else
            Debug.LogError("Hitbox가 연결되지 않았습니다!");
    }

    private void Update()
    {
        if (!_canCombat) return;

        // Idle 상태일 때만 입력 처리
        if (CurrentState == PlayerState.Idle && _inputQueue.Count > 0)
        {
            var nextAction = _inputQueue.Dequeue();
            nextAction?.Invoke();
            return;
        }

        if (CurrentState != PlayerState.Idle) return;

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
        if (CurrentState == PlayerState.Idle)
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
        if (Time.time - _lastAttackTime < _attackCooldown) return;

        CurrentState = PlayerState.Attacking;
        _lastAttackTime = Time.time;

        _animator.SetTrigger("Attack");
        //Debug.Log("기본 공격");

        StartCoroutine(PerformAttack());

        StartCoroutine(ResetStateAfter(_attackCooldown));
    }
    private IEnumerator PerformAttack()
    {
        // 공격 시작 전 딜레이 (애니메이션 타이밍 맞춤)
        yield return new WaitForSeconds(0.1f); // 필요에 따라 조절 가능

        // 공격 시작
        AttackStart();

        PlaySlashEffect();

        // 공격 활성화 지속 시간
        yield return new WaitForSeconds(1f); // 필요에 따라 조절 가능

        // 공격 종료
        AttackEnd();
    }
    public void AttackStart()
    {
        _isAttackActive = true;
        _hitbox?.ResetHitCache();
        //Debug.Log("공격 시작!");
    }

    public void AttackEnd()
    {
        _isAttackActive = false;
        //Debug.Log("공격 종료!");
    }

    private void PlaySlashEffect()
    {
        if (slashEffectPrefab != null && effectPoint != null)
        {
            GameObject effect = Instantiate(slashEffectPrefab, effectPoint.position, effectPoint.rotation);
            Destroy(effect, 1f); // 2초 후 자동 삭제
        }
    }
    private IEnumerator Dodge()
    {
        CurrentState = PlayerState.Dodging;
        _isDodging = true;

        Vector3 cachedDirection = transform.forward;
        _animator.SetTrigger("Dodge");

        float elapsed = 0f;
        while (elapsed < _dodgeDuration)
        {
            transform.position += _dodgeDirection.normalized * (_dodgeDistance / _dodgeDuration) * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDodging = false;
        CurrentState = PlayerState.Idle;
    }

    private IEnumerator Block()
    {
        CurrentState = PlayerState.Blocking;
        _isBlocking = true;

        _animator.SetBool("isBlocking", true);
        Debug.Log("방어 시작");

        yield return new WaitForSeconds(_blockDuration);

        _isBlocking = false;
        _animator.SetBool("isBlocking", false);
        Debug.Log("방어 종료");

        CurrentState = PlayerState.Idle;
    }

    private IEnumerator ResetStateAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        CurrentState = PlayerState.Idle;
    }

    public bool IsBlocking() => _isBlocking;
    public bool IsDodging() => _isDodging;
}

