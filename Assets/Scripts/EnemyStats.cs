using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _destroyDelay = 3f;
    [SerializeField] private float _knockbackForce = 5f;

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform hitPoint;

    private int _currentHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    private Animator _animator;
    private Collider _collider;
    private NavMeshAgent _agent;
    private EnemyAI _ai;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _currentHp = _maxHp;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _ai = GetComponent<EnemyAI>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);

        _animator.SetTrigger("GetHit");
        HitEffect();

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {_currentHp}");

        // ≥ÀπÈ √≥∏Æ
        if (_rigidbody != null)
        {
            Vector3 dir = hitDirection;
            dir.y = 0.2f;
            if (dir.sqrMagnitude > 0.001f) // 0 ∫§≈Õ πÊ¡ˆ
            {
                _rigidbody.velocity = Vector3.zero; // ±‚¡∏ »˚ √ ±‚»≠
                _rigidbody.AddForce(dir.normalized * _knockbackForce, ForceMode.Impulse);
            }
        }

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void HitEffect()
    {
        if (effectPrefab != null && hitPoint != null)
        {
            GameObject effect = Instantiate(effectPrefab, hitPoint.position, Quaternion.identity);
            effect.transform.forward = transform.forward;
            Destroy(effect, 1f);
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        _animator.SetBool("isDead", true);

        if (_collider != null) _collider.enabled = false;
        if (_agent != null) _agent.enabled = false;
        if (_ai != null) _ai.OnDeath();

        StartCoroutine(RemoveAfterDelay());
    }

    IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }
}
