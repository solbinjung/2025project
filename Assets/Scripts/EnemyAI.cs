using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float contactDamageCooldown = 3f;

    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float detectionRange = 15f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;   // 랜덤 순찰 반경
    [SerializeField] private float patrolWaitTime = 3f;  // 순찰 지점에서 대기 시간
    private bool waitingAtPoint = false;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canDealContactDamage = true;
    private bool canAttack = true;
    private bool isDead = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (isDead) return;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            // 플레이어가 탐지 범위 밖 → 랜덤 순찰
            PatrolRandom();
        }
        else if (distance > attackRange)
        {
            // 플레이어 탐지됨, 공격 범위 밖 → 추적
            SetMovement(player.position, 5f, false); // Run
        }
        else
        {
            // 플레이어 공격 범위 안 → 공격
            SetMovement(Vector3.zero, 0f, true); // Idle
            TryAttack();
        }
    }
    // 랜덤 순찰 로직
    private void PatrolRandom()
    {
        if (!waitingAtPoint && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance))
        {
            // 랜덤한 목적지 찾기
            Vector3 randomPos = GetRandomNavMeshPosition(patrolRadius);
            SetMovement(randomPos, 2f, false); // Walk

            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        waitingAtPoint = true;
        yield return new WaitForSeconds(patrolWaitTime);
        waitingAtPoint = false;
    }

    // NavMesh 위의 랜덤 지점 반환
    private Vector3 GetRandomNavMeshPosition(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // 실패 시 현재 위치 반환
    }

    // 이동 + 애니메이션 제어
    private void SetMovement(Vector3 targetPos, float speed, bool stop)
    {
        if (stop)
        {
            agent.isStopped = true;
            animator.SetFloat("MoveSpeed", 0f);
        }
        else
        {
            agent.isStopped = false;
            agent.speed = speed;
            agent.SetDestination(targetPos);
            animator.SetFloat("MoveSpeed", speed > 3f ? 1f : 0.5f);
            // 1f = 달리기, 0.5f = 걷기
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canDealContactDamage && collision.gameObject.CompareTag("Player"))
        {
            PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(contactDamage);
                StartCoroutine(ContactDamageCooldown());
            }
        }
    }

    private IEnumerator ContactDamageCooldown()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }

    private void TryAttack()
    {
        if (canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    public void OnDeath()
    {
        isDead = true;
        if (agent != null) agent.enabled = false;
    }
}

