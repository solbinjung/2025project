using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private bool _isKnockbackImmune = false; 

    [Header("Death")]
    [SerializeField] private float _destroyDelay = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform hitPoint;

    private int _currentHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    // 0.7f = 70%
    public float HealthPercentage => (float)_currentHp / _maxHp;

    public bool IsInvincible { get; set; } = false;

    public bool IsDead { get; private set; } = false;

    private Animator _animator;
    private Collider _collider;
    private NavMeshAgent _agent;
    private EnemyAI _ai;
    private Rigidbody _rigidbody;

    private Coroutine hitFlashRoutine;
    private Material[] _cachedMaterials;

    protected virtual void Awake()
    {
        _currentHp = _maxHp;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _ai = GetComponent<EnemyAI>();
        _rigidbody = GetComponent<Rigidbody>();

        // material 캐싱
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        _cachedMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            _cachedMaterials[i] = renderers[i].material;
        }
    }

    public virtual void TakeDamage(int damage, Vector3 hitDirection)
    {
        if (IsInvincible || IsDead) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);

        _animator.SetTrigger("GetHit");
        HitEffect();

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {_currentHp}");

        // 넉백 처리
        if (_rigidbody != null && !_isKnockbackImmune)
        {
            Vector3 dir = hitDirection;
            dir.y = 0.2f;
            if (dir.sqrMagnitude > 0.001f) // 0 벡터 방지
            {
                _rigidbody.velocity = Vector3.zero; // 기존 힘 초기화
                _rigidbody.AddForce(dir.normalized * _knockbackForce, ForceMode.Impulse);
            }
        }

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void HitEffect()
    {
        if (effectPrefab != null && hitPoint != null)
        {
            GameObject effect = Instantiate(effectPrefab, hitPoint.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        if (hitFlashRoutine != null)
            StopCoroutine(hitFlashRoutine);

        hitFlashRoutine = StartCoroutine(HitFlashCoroutine());
    }

    protected virtual IEnumerator HitFlashCoroutine()
    {
        // 색 변경
        foreach (var m in _cachedMaterials)
            m.color = Color.red;

        // 0.2초 대기
        yield return new WaitForSeconds(0.2f);

        // 원래 색상 복원
        foreach (var m in _cachedMaterials)
            m.color = Color.white;

        hitFlashRoutine = null;
    }

    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log($"{gameObject.name} died!");
        _animator.SetBool("isDead", true);

        if (_collider != null) _collider.enabled = false;
        if (_agent != null) _agent.enabled = false;
        if (_ai != null) _ai.OnDeath();

        StartCoroutine(RemoveAfterDelay());
    }

    protected virtual IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }
}
