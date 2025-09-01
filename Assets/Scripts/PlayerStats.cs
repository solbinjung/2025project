using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _maxMp = 100;
    
    [SerializeField] private float _invincibleDuration = 2f;

    private int _currentHp;
    private int _currentMp;
    private bool _isInvincible = false;

    public int MaxHp => _maxHp;
    public int MaxMp => _maxMp;
    public int CurrentHp => _currentHp;
    public int CurrentMp => _currentMp;

    private Animator _animator;

    void Start()
    {
        _currentHp = _maxHp;
        _currentMp = _maxMp;

        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) // 적에 의해 데미지를 입을 경우
    {
        if (_isInvincible) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0); // HP가 마이너스 값이 되지 않도록 하기 위해
        _animator.SetTrigger("GetHit");
        Debug.Log($"Player took {damage} damage. Current HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibleCoroutine());
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;

        Renderer renderer = GetComponentInChildren<Renderer>(); // 플레이어 MeshRenderer 가져오기
        float elapsed = 0f;

        while (elapsed < _invincibleDuration)
        {
            if (renderer != null)
            {
                renderer.enabled = !renderer.enabled; // 깜빡이기
            }
            yield return new WaitForSeconds(0.2f);
            elapsed += 0.2f;
        }

        // 무적 해제
        if (renderer != null)
            renderer.enabled = true; // 보이게 고정
        _isInvincible = false;
    }

    public void Heal(int amount) // HP 충전
    {
        _currentHp += amount;
        _currentHp = Mathf.Min(_currentHp, _maxHp); // HP가 최대 HP 값을 초과하지 않도록 하기 위해
    }

    private void Die() // 플레이어 사망
    {
        _animator.SetBool("isDead", true);
        Debug.Log($"{gameObject.name} died!");
        GetComponent<PlayerController>().enabled = false; // 이동 막기
        GetComponent<Collider>().enabled = false;
    }
}
