using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int _contactDamage = 10;
    [SerializeField] protected float _contactDamageCooldown = 3f;

    [SerializeField] protected int _attackDamage = 20;
    [SerializeField] protected float _attackRange = 7f;
    [SerializeField] protected float _attackCooldown = 2f;
    [SerializeField] protected float _detectionRange = 15f;

    [Header("Patrol Settings")]
    [SerializeField] protected float _patrolRadius = 10f;   // 랜덤 순찰 반경
    [SerializeField] protected float _patrolWaitTime = 3f;  // 순찰 지점에서 대기 시간
    protected bool _waitingAtPoint = false;

    protected Transform _player;
    protected PlayerStats _playerStats;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected EnemyStats _stats;

    protected bool _canDealContactDamage = true;
    protected bool _canAttack = true;
    protected bool _isDead = false;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _stats = GetComponent<EnemyStats>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }
    }

    protected virtual void Update()
    {
        if (_isDead) return;
        if (_player == null) return;

        PlayerStats playerStats = _player.GetComponent<PlayerStats>();
        if (playerStats != null && playerStats.IsDead)
        {
            // 플레이어 죽으면 그냥 대기 상태 유지
            SetMovement(Vector3.zero, 0f, true);
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance > _detectionRange)
        {
            // 플레이어가 탐지 범위 밖 -> 랜덤 순찰
            PatrolRandom();
        }
        else if (distance > _attackRange)
        {
            // 플레이어 탐지됨, 공격 범위 밖 -> 추적
            SetMovement(_player.position, 5f, false); // Run
        }
        else
        {
            // 플레이어 공격 범위 안 -> 공격
            SetMovement(Vector3.zero, 0f, true); // Idle
            TryAttack();
        }
    }
    // 랜덤 순찰 로직
    protected virtual void PatrolRandom()
    {
        if (!_waitingAtPoint && (!_agent.hasPath || _agent.remainingDistance <= _agent.stoppingDistance))
        {
            // 랜덤한 목적지 찾기
            Vector3 randomPos = GetRandomNavMeshPosition(_patrolRadius);
            SetMovement(randomPos, 2f, false); // Walk

            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    protected virtual IEnumerator WaitAtPatrolPoint()
    {
        _waitingAtPoint = true;
        yield return new WaitForSeconds(_patrolWaitTime);
        _waitingAtPoint = false;
    }

    // NavMesh 위의 랜덤 지점 반환
    protected Vector3 GetRandomNavMeshPosition(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // 실패 시 현재 위치 반환
    }

    // 이동 + 애니메이션 제어
    protected void SetMovement(Vector3 targetPos, float speed, bool stop)
    {
        if (_agent.enabled == false || _isDead) return;

        if (stop)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _animator.SetFloat("MoveSpeed", 0f);
        }
        else
        {
            _agent.isStopped = false;
            _agent.speed = speed;
            _agent.SetDestination(targetPos);
            _animator.SetFloat("MoveSpeed", _agent.velocity.magnitude / _agent.speed);
            // 1f = 달리기, 0.5f = 걷기
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (_canDealContactDamage && collision.gameObject.CompareTag("Player"))
        {
            if (_playerStats != null && !_playerStats.IsDead)
            {
                Vector3 hitDirection = (_playerStats.transform.position - transform.position).normalized;
                _playerStats.TakeDamage(_contactDamage, hitDirection);

                StartCoroutine(ContactDamageCooldown());
            }
        }
    }

    protected virtual IEnumerator ContactDamageCooldown()
    {
        _canDealContactDamage = false;
        yield return new WaitForSeconds(_contactDamageCooldown);
        _canDealContactDamage = true;
    }

    protected virtual void TryAttack()
    {
        if (_canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    protected virtual IEnumerator AttackRoutine()
    {
        _canAttack = false;
        transform.LookAt(_player);
        _animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, _player.position) <= _attackRange + 0.5f)
        {
            if (_playerStats != null && !_playerStats.IsDead)
            {
                Vector3 hitDirection = (_playerStats.transform.position - transform.position).normalized;
                _playerStats.TakeDamage(_attackDamage, hitDirection);
            }
        }

        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
    public virtual void OnDeath()
    {
        _isDead = true;
        StopAllCoroutines(); // 모든 행동(공격, 순찰) 중지
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }
    }
}

