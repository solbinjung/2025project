using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _destroyDelay = 3f;

    private int _currentHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    private Animator _animator;
    private Collider _collider;
    private NavMeshAgent _agent;
    private EnemyAI _ai;

    void Awake()
    {
        _currentHp = _maxHp;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _ai = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);

        _animator.SetTrigger("GetHit");

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
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
