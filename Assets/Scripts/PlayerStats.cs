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
    private bool _isDead = false;
    
    public int MaxHp => _maxHp;
    public int MaxMp => _maxMp;
    public int CurrentHp => _currentHp;
    public int CurrentMp => _currentMp;
    public bool IsDead => _isDead;
    public bool IsInvincible => _isInvincible;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private PlayerController _playerController;
    private PlayerCombat _playerCombat;

    void Start()
    {
        _currentHp = _maxHp;
        _currentMp = _maxMp;

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _playerController = GetComponent<PlayerController>();
        _playerCombat = GetComponent<PlayerCombat>();

    }

    public void TakeDamage(int damage, Vector3 hitDirection) // 적에 의해 데미지를 입을 경우
    {
        if (_isInvincible || _isDead ) return;

        _playerCombat.OnTakeHit();
        _playerController.StopMovement(); // 이동 중지

        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0); // HP가 마이너스 값이 되지 않도록 하기 위해

        // 넉백 처리
        if (_rigidbody != null)
        {
            Vector3 dir = hitDirection;
            dir.y = 0.2f;
            if (dir.sqrMagnitude > 0.001f) // 0 벡터 방지
            {
                _rigidbody.velocity = Vector3.zero; // 기존 힘 초기화
                _rigidbody.AddForce(dir.normalized * _knockbackForce, ForceMode.Impulse);
            }
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

        _playerCombat.State = PlayerCombat.PlayerState.Idle;
    }

    private IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;
        _playerController.CanControl = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;

        // 원래 색상 저장
        Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
        foreach (var r in renderers)
        {
            originalColors[r] = r.material.color;
        }

        while (elapsed < _invincibleDuration)
        {
            foreach (var r in renderers)
            {
                // 빨강 or 원래색 번갈아가면서 적용
                if (Mathf.FloorToInt(elapsed * 5f) % 2 == 0) 
                    r.material.color = Color.red;
                else
                    r.material.color = originalColors[r];
            }

            yield return new WaitForSeconds(0.15f); // 0.15초 간격
            elapsed += 0.2f;
        }

        // 끝난 뒤 원래 색상 복원
        foreach (var r in renderers)
        {
            r.material.color = originalColors[r];
        }

        _isInvincible = false;
    }
    
    public void SetInvincible(bool value)
    {
        _isInvincible = value;
    }

    public void CostMp(int mpCost) // MP 소모
    {
        _currentMp -= mpCost;
        _currentMp = Mathf.Max(_currentMp, 0);
    }

    public void Heal(int amount) // HP 충전
    {
        _currentHp += amount;
        _currentHp = Mathf.Min(_currentHp, _maxHp);
    }

    public void RestoreMp(int amount) // MP 충전
    {
        _currentMp += amount;
        _currentMp = Mathf.Min(_currentMp, _maxMp);
    }
    private void Die()
    {
        _isDead = true;
        _animator.SetBool("isDead", true);
        Debug.Log("Player died!");
        _playerController.StopMovement();

        if (_playerController != null)
            _playerController.CanControl = false;

        if (_rigidbody != null)
            _rigidbody.isKinematic = true;

        //if (_collider != null)
        //    _collider.enabled = false;
    }
}
