using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _maxMp = 100;
    
    [SerializeField] private float _invincibleDuration = 2f;
    [SerializeField] private float _knockbackForce = 5f;

    private int _currentHp;
    private int _currentMp;
    private bool _isInvincible = false;

    public int MaxHp => _maxHp;
    public int MaxMp => _maxMp;
    public int CurrentHp => _currentHp;
    public int CurrentMp => _currentMp;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerController _playerController;
    private Collider _collider;

    void Start()
    {
        _currentHp = _maxHp;
        _currentMp = _maxMp;

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
        _collider = GetComponent<Collider>();
    }

    public void TakeDamage(int damage, Vector3 hitDirection) // 적에 의해 데미지를 입을 경우
    {
        if (_isInvincible) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0); // HP가 마이너스 값이 되지 않도록 하기 위해
        _animator.SetTrigger("GetHit");

        // 넉백 적용
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(-hitDirection.normalized * _knockbackForce, ForceMode.Impulse);
        }

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

        if (_playerController != null)
            _playerController.CanControl = false; // 무적 동안 이동 막기

        Renderer renderer = GetComponentInChildren<Renderer>();
        float elapsed = 0f;

        while (elapsed < _invincibleDuration)
        {
            if (renderer != null)
                renderer.enabled = !renderer.enabled;

            yield return new WaitForSeconds(0.2f);
            elapsed += 0.2f;
        }

        if (renderer != null)
            renderer.enabled = true;

        _isInvincible = false;

        if (_playerController != null)
            _playerController.CanControl = true; // 제어 복구
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

        if (_playerController != null)
            _playerController.CanControl = false;

        if (_collider != null)
            _collider.enabled = false;

        if (_rigidbody != null)
            _rigidbody.isKinematic = true; // 죽으면 물리 영향 막기
    }
}
