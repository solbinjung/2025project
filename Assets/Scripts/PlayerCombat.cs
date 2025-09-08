using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private readonly Queue<Action> _inputQueue = new Queue<Action>();

    [Header("Settings")]
    [SerializeField] private bool _canCombat = true;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private MeleeHitBox _hitbox;

    [SerializeField] private float _dodgeDistance = 5f;
    [SerializeField] private float _dodgeDuration = 0.3f;
    [SerializeField] private float dodgeCooldown = 1f;

    [SerializeField] private float _blockDuration = 1.0f;

    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform effectPoint;

    private bool _isAttackActive = false;
    private bool _isDodging = false;
    private bool _isBlocking = false;
    private float _lastAttackTime = -Mathf.Infinity;
    private Vector3 _dodgeDirection;

    private PlayerController _playerController;
    private PlayerStats _playerStats;
    private Animator _animator;
    private EnemyStats _stats;

    public enum PlayerState
    {
        Idle,
        Attacking,
        Dodging,
        Blocking,
        GettingHit
    }

    public PlayerState State { get; set; } = PlayerState.Idle; // 외부에서도 변경 가능
    // 내부용 필드 필요 없음, 시스템 그대로 유지

    // 프로퍼티
    public bool CanCombat => _canCombat;
    public float AttackCooldown => _attackCooldown;
    public float DodgeDistance => _dodgeDistance;
    public float BlockDuration => _blockDuration;
    public int Damage => _damage;
    public bool IsAttackActive => _isAttackActive;
    public EnemyStats Stats => _stats;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStats = GetComponent<PlayerStats>();
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

        if (State == PlayerState.Idle && _inputQueue.Count > 0)
        {
            var nextAction = _inputQueue.Dequeue();
            nextAction?.Invoke();
            return;
        }

        if (State != PlayerState.Idle) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
            QueueAction(TryAttack);

        if (Input.GetKeyDown(KeyCode.Space))
            QueueAction(() => StartCoroutine(Dodge()));

        if (Input.GetKeyDown(KeyCode.LeftShift))
            QueueAction(() => StartCoroutine(Block()));
    }

    private void QueueAction(Action action)
    {
        if (State == PlayerState.Idle)
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

        State = PlayerState.Attacking;
        _lastAttackTime = Time.time;

        _animator.SetTrigger("Attack");

        StartCoroutine(PerformAttack());
        StartCoroutine(ResetStateAfter(_attackCooldown));
    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(0.1f);
        AttackStart();
        yield return new WaitForSeconds(0.4f);
        PlaySlashEffect();
        AttackEnd();
    }

    public void AttackStart()
    {
        _isAttackActive = true;
        _hitbox?.ResetHitCache();
    }

    public void AttackEnd()
    {
        _isAttackActive = false;
    }

    private void PlaySlashEffect()
    {
        if (slashEffectPrefab != null && effectPoint != null)
        {
            GameObject effect = Instantiate(slashEffectPrefab, effectPoint.position, effectPoint.rotation);
            Destroy(effect, 1f);
        }
    }

    private IEnumerator Dodge()
    {
        State = PlayerState.Dodging;
        _isDodging = true;

        // 무적 상태
        _playerStats.SetInvincible(true);

        // 애니메이션 트리거 실행
        _animator.SetTrigger("Dodge");
      
        _dodgeDirection = transform.forward; // 현재 바라보는 방향으로 회피
        Quaternion fixedRotation = transform.rotation; 

        float elapsed = 0f;
        while (elapsed < _dodgeDuration)
        {
            float speed = DodgeDistance / _dodgeDuration;
            transform.position += _dodgeDirection * speed * Time.deltaTime;

            transform.rotation = fixedRotation;

            elapsed += Time.deltaTime;
            yield return null; 
        }
        // 무적 해제
        _playerStats.SetInvincible(false);

        _isDodging = false;
        State = PlayerState.Idle;

        yield return new WaitForSeconds(dodgeCooldown);
    }

    private IEnumerator Block()
    {
        State = PlayerState.Blocking;
        _isBlocking = true;

        _animator.SetBool("isBlocking", true);

        yield return new WaitForSeconds(_blockDuration);

        _isBlocking = false;
        _animator.SetBool("isBlocking", false);

        State = PlayerState.Idle;
    }

    private IEnumerator ResetStateAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (State == PlayerState.Attacking)
            State = PlayerState.Idle;
    }

    // 피격 처리: State 변경 + Animator 트리거
    public void OnTakeHit()
    {
        State = PlayerState.GettingHit;
        _animator.SetTrigger("GetHit");
        _playerController.StopMovement(); // 이동 강제 중지
    }

    public bool IsBlocking() => _isBlocking;
    public bool IsDodging() => _isDodging;
}
