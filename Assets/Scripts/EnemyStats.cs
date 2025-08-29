using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float destroyDelay = 3f;

    private int _currentHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    private Animator animator;
    private Collider col;
    private NavMeshAgent agent;
    private EnemyAI ai;

    void Awake()
    {
        _currentHp = _maxHp;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);

        animator.SetTrigger("GetHit");

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        animator.SetBool("isDead", true);

        if (col != null) col.enabled = false;
        if (agent != null) agent.enabled = false;
        if (ai != null) ai.OnDeath();

        StartCoroutine(RemoveAfterDelay());
    }

    IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
