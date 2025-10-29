using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    [Header("Boss Phase Settings")]
    [SerializeField] private float _phase2HealthThreshold = 0.7f;   // 체력 70% 이하
    [SerializeField] private float _phase3HealthThreshold = 0.3f;   // 체력 30% 이하
    [SerializeField] private float _phaseTransitionDuration = 5.0f; // 페이즈 전환 시 5초 무적

    [Header("Basic Attack")]
    // 기본 근접 공격
    [SerializeField] private int _basicAttackDamage = 400; 

    [Header("Special Attack A")]
    // 점프 범위 공격
    [SerializeField] private int _specialAttackADamage = 600;
    //[SerializeField] private GameObject _specialAttackAWarningPrefab;     // 경고 프리팹
    //[SerializeField] private float _specialAttackAWarningDuration = 2.0f; // 경고 프리팹 지속 시간
    [SerializeField] private float _specialAttackARadius = 5f;            // 공격 범위
    [SerializeField] private float _specialAttackACooldown = 5f;          // A 스킬 쿨타임

    [Header("Special Attack B")]
    // 광역 공격
    [SerializeField] private int _specialAttackBDamage = 800;
    //[SerializeField] private GameObject _specialAttackBConeWarningPrefab; // 경고 프리팹
    //[SerializeField] private float _specialAttackBWarningDuration = 3.0f; // 경고 프리팹 지속 시간
    [SerializeField] private GameObject _specialAttackBFirePrefab;        // 불 프리팹(공격 이펙트)
    [SerializeField] private float _specialAttackBCooldown = 8f;          // B 스킬 쿨타임
    
    private int _currentPhase = 1;
    private bool _isTransitioning = false; // 페이즈 변경 연출 중인지 여부

    // 각 스킬의 쿨타임 추적용
    private float _specialACooldownTimer = 0f;
    private float _specialBCooldownTimer = 0f;

    protected override void Awake()
    {
        // 1. 부모 클래스의 Awake()를 실행 (_agent, _animator, _player 초기화)
        base.Awake();

        // 2. BossAI는 EnemyStats 컴포넌트가 필수

        if (_stats == null)
        {
            Debug.LogError(gameObject.name + " requires an EnemyStats component!");
        }
    }

    // --- 2. Update: 부모(EnemyAI)의 Update 로직에 페이즈 체크 추가 ---
    protected override void Update()
    {
        // 부모의 기본 체크 (사망, 플레이어 없음)
        if (_isDead || _player == null) return;

        // 플레이어 사망 시 부모 로직 재사용
        PlayerStats playerStats = _player.GetComponent<PlayerStats>();
        if (playerStats != null && playerStats.IsDead)
        {
            SetMovement(Vector3.zero, 0f, true);
            return;
        }

        // 1. (Boss 전용) 페이즈 전환 체크
        CheckPhaseTransition();

        // 2. (Boss 전용) 페이즈 전환 중에는 모든 행동 중지
        if (_isTransitioning)
        {
            SetMovement(Vector3.zero, 0f, true); // 멈춤
            return;
        }

        // 3. (Boss 전용) 스킬 쿨타임 감소
        if (_specialACooldownTimer > 0) _specialACooldownTimer -= Time.deltaTime;
        if (_specialBCooldownTimer > 0) _specialBCooldownTimer -= Time.deltaTime;

        // 4. (부모 로직 재사용) 페이즈 전환 중이 아니라면, 부모의 Update 로직 실행
        // (Patrol -> Chase -> TryAttack)
        base.Update();
    }

    // 페이즈 관리
    private void CheckPhaseTransition()
    {
        // 전환 중이거나, stats가 없거나, 이미 마지막 페이즈라면 체크 중지
        if (_isTransitioning || _stats == null || _currentPhase == 3) return;

        float currentHealthPct = _stats.HealthPercentage;

        // HP 70% 도달 시
        if (_currentPhase == 1 && currentHealthPct <= _phase2HealthThreshold)
        {
            StartCoroutine(PhaseTransitionRoutine(2));
        }
        // HP 30% 도달 시
        else if (_currentPhase == 2 && currentHealthPct <= _phase3HealthThreshold)
        {
            StartCoroutine(PhaseTransitionRoutine(3));
        }
    }

    // 페이즈 전환 로직
    private IEnumerator PhaseTransitionRoutine(int newPhase)
    {
        _isTransitioning = true;
        _currentPhase = newPhase;
        _canAttack = false; // 모든 공격 중지

        Debug.Log($"BOSS: ENTERING PHASE {newPhase}!");

        // 1. 행동 중지
        SetMovement(Vector3.zero, 0f, true);

        // 2. 무적 : 3초 간
        _stats.IsInvincible = true;

        // 3. 포효 : Scream 애니메이션 재생
        _animator.SetTrigger("Scream");

        // 4. 3초 대기
        yield return new WaitForSeconds(_phaseTransitionDuration);

        // 5. 전환 완료 : 무적이 풀리고, 다음 페이즈 행동 시작
        _stats.IsInvincible = false;

        _isTransitioning = false;
        _canAttack = true; // 다시 공격 가능
        _specialACooldownTimer = 0; // 스킬 쿨타임 초기화
        _specialBCooldownTimer = 0;
    }

    // 보스는 순찰(Patrol)하지 않고, 플레이어를 바라보며 대기
    protected override void PatrolRandom()
    {
        if (_player == null) return;

        SetMovement(Vector3.zero, 0f, true); // 멈춤

        // 플레이어 바라보기 (Y축 고정)
        var lookPos = _player.position - transform.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

    // 공격 시도 (TryAttack) 로직을 재정의 -> 페이즈별로 다른 공격 선택
    protected override void TryAttack()
    {
        if (!_canAttack || _isTransitioning) return;

        // 공격 전 플레이어 즉시 바라보기
        transform.LookAt(_player.position);

        // 페이즈별로 공격 패턴 결정
        switch (_currentPhase)
        {
            case 1:
                // 페이즈 1: 기본 근접 공격 (100%)
                StartCoroutine(AttackRoutine());
                break;

            case 2:
                // 페이즈 2: 기본 공격 (60%) 또는 특수 공격 A (40%)
                if (Random.value < 0.6f)
                {
                    StartCoroutine(AttackRoutine()); // 60%
                }
                else if (_specialACooldownTimer <= 0) // 40% + 쿨타임 체크
                {
                    StartCoroutine(SpecialAttackA());
                }
                else
                {
                    StartCoroutine(AttackRoutine()); // 스킬 쿨타임이면 기본 공격
                }
                break;

            case 3:
                // 페이즈 3: 기본(30%), A(40%), B(30%)
                float rand = Random.value;
                if (rand < 0.3f)
                {
                    StartCoroutine(AttackRoutine()); // 30%
                }
                else if (rand < 0.7f && _specialACooldownTimer <= 0) // 40% + 쿨타임
                {
                    StartCoroutine(SpecialAttackA());
                }
                else if (_specialBCooldownTimer <= 0) // 30% + 쿨타임
                {
                    StartCoroutine(SpecialAttackB());
                }
                else
                {
                    StartCoroutine(AttackRoutine()); // 쓸 스킬 없으면 기본 공격
                }
                break;
        }
    }

    // 기본 근접 공격 (재정의)
    protected override IEnumerator AttackRoutine()
    {
        _canAttack = false;
        _animator.SetTrigger("BasicAttack"); // 기본 공격 애니메이션

        yield return new WaitForSeconds(0.5f); // 선딜레이

        // 데미지 판정
        if (_player != null && Vector3.Distance(transform.position, _player.position) <= _attackRange + 0.5f)
        {
            PlayerStats playerStats = _player.GetComponent<PlayerStats>();
            if (playerStats != null && !playerStats.IsDead)
            {
                Vector3 hitDirection = (playerStats.transform.position - transform.position).normalized;
                playerStats.TakeDamage(_basicAttackDamage, hitDirection);
            }
        }
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    // 특수 공격 A (점프 공격)
    protected virtual IEnumerator SpecialAttackA()
    {
        _canAttack = false;
        _specialACooldownTimer = _specialAttackACooldown; // 쿨타임 시작

        // 1. 잠깐 멈춤 (예: 0.5초) + 애니메이션 준비
        SetMovement(Vector3.zero, 0f, true); // 이동 멈춤
        _animator.SetTrigger("ClawAttack"); // 점프 준비 또는 공격 애니메이션 트리거
        yield return new WaitForSeconds(0.5f); // 공격 전 잠시 멈춤

        // 2. 점프 목표 지점 설정 (현재 플레이어 위치)
        Vector3 startPosition = transform.position; // 보스 현재 위치
        Vector3 targetPosition = _player.position; // 플레이어 현재 위치
        // (선택) targetPosition.y = transform.position.y; // 바닥으로 점프하려면 y값 고정

        // 3. 점프 시작 전 NavMeshAgent 비활성화 (물리적 이동을 위해)
        if (_agent.enabled)
        {
            _agent.enabled = false;
        }

        // --- 4. 점프 이동 (Lerp 사용) ---
        float jumpDuration = 0.8f; // 점프에 걸리는 시간 (애니메이션 길이에 맞추세요)
        float jumpHeight = 3.0f;   // 점프 높이
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration; // 진행률 (0 -> 1)

            // 수평 이동 (Lerp)
            Vector3 currentHorizontalPos = Vector3.Lerp(startPosition, targetPosition, t);

            // 수직 이동 (포물선)
            float currentHeight = jumpHeight * 4 * (t - t * t); // 간단한 포물선 공식

            // 최종 위치 설정
            transform.position = new Vector3(currentHorizontalPos.x, startPosition.y + currentHeight, currentHorizontalPos.z);

            // 다음 프레임까지 대기
            yield return null;
        }
        // --- 점프 끝 ---

        // 5. 착지 위치 보정
        transform.position = targetPosition; // 목표 지점에 정확히 착지

        // 6. 착지 지점에 데미지 판정 (OverlapSphere 사용)
        Collider[] hits = Physics.OverlapSphere(transform.position, _specialAttackARadius); // 현재 위치 기준
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null && !playerStats.IsDead)
                {
                    playerStats.TakeDamage(_specialAttackADamage, Vector3.zero); // 넉백 없음
                }
            }
        }

        // 7. 화면 진동 효과
        Debug.Log("화면 진동! (Special Attack A)");
        // CameraShaker.Instance.Shake(0.5f, 0.2f); // 실제 진동 코드

        // 8. NavMeshAgent 다시 활성화 (잠시 후)
        yield return new WaitForSeconds(0.1f); // 착지 후 잠시 대기
        if (!_agent.enabled)
        {
            _agent.enabled = true;
            // (선택) 에이전트 위치를 현재 위치로 동기화 (필요시)
            // if (_agent.isOnNavMesh) _agent.Warp(transform.position); 
        }

        // 9. 공격 후딜레이 및 상태 복귀
        yield return new WaitForSeconds(1.0f); // 후딜레이
        _canAttack = true;
    }
    // 특수 공격 B
    protected virtual IEnumerator SpecialAttackB()
    {
        _canAttack = false;
        _specialBCooldownTimer = _specialAttackBCooldown;

        //// 2. 보스 앞에 부채꼴 경고 프리팹 3초간
        //// (프리팹이 보스를 따라다니도록 자식으로 붙임)
        //GameObject warningMarker = Instantiate(_specialAttackBConeWarningPrefab, transform.position, transform.rotation, transform);

        //// 3. 경고 프리팹 3초간
        //yield return new WaitForSeconds(_specialAttackBWarningDuration);

        //// 4. 경고 마커 제거
        //Destroy(warningMarker);

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("FlameAttack");
        // 5. 불 프리팹 생성
        if (_specialAttackBFirePrefab != null)
        {
            Instantiate(_specialAttackBFirePrefab, transform.position, transform.rotation, transform);
        }

        Collider[] hits = Physics.OverlapBox(transform.position + transform.forward * 5f, new Vector3(3f, 2f, 5f), transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null && !playerStats.IsDead)
                {
                    // 데미지 800
                    playerStats.TakeDamage(_specialAttackBDamage, Vector3.zero);
                }
            }
        }

        Debug.Log("불 내뿜기! (Special Attack B)");

        yield return new WaitForSeconds(2f);

        _canAttack = true;
    }
}