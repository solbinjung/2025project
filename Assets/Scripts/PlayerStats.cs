using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _maxMp = 100;

    private int _currentHp;
    private int _currentMp;

    public int MaxHp => _maxHp;
    public int MaxMp => _maxMp;
    public int CurrentHp => _currentHp;
    public int CurrentMp => _currentMp;

    private Animator animator;

    void Start()
    {
        _currentHp = _maxHp;
        _currentMp = _maxMp;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) // 적에 의해 데미지를 입을 경우
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0); // HP가 마이너스 값이 되지 않도록 하기 위해
        //animator.SetTrigger("GetHit");
        Debug.Log($"Player took {damage} damage. Current HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount) // HP 충전
    {
        _currentHp += amount;
        _currentHp = Mathf.Min(_currentHp, _maxHp); // HP가 최대 HP 값을 초과하지 않도록 하기 위해
    }

    private void Die() // 플레이어 사망
    {
        //animator.SetTrigger("Die");
        Debug.Log($"{gameObject.name} died!");
        GetComponent<PlayerController>().enabled = false; // 이동 막기
        GetComponent<Collider>().enabled = false;
    }
}
