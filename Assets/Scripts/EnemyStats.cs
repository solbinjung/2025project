using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float destroyDelay = 3f;

    private int _currentHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    private Animator animator;

    void Start()
    {
        _currentHp = _maxHp;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) // 적에 의해 데미지를 입을 경우
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0); // HP가 마이너스 값이 되지 않도록 하기 위해
        //animator.SetTrigger("GetHit");
        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die() // 플레이어 사망
    {
        Debug.Log($"{gameObject.name} died!");
        //animator.SetTrigger("Die");
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 일정 시간 후 오브젝트 제거
        StartCoroutine(RemoveAfterDelay());
    }

    IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
