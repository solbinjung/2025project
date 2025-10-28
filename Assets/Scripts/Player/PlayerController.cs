using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private LayerMask _groundMask;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector3 _destPos;
    private Quaternion _lookTarget;
    private PlayerCombat _playerCombat;
    private PlayerStats _playerStats;
    private PlayerSkillManager _skillManager;

    private bool _move = false;
    public bool CanControl = true;

    private void Start()
    {
        _skillManager = GetComponent<PlayerSkillManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        HandleMovement();
        HandleSkillInput();
        HandleItemInput();
    }

    private void HandleSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) _skillManager.UseSkill(KeyCode.Q);
        if (Input.GetKeyDown(KeyCode.W)) _skillManager.UseSkill(KeyCode.W);
        if (Input.GetKeyDown(KeyCode.E)) _skillManager.UseSkill(KeyCode.E);
        if (Input.GetKeyDown(KeyCode.R)) _skillManager.UseSkill(KeyCode.R);
        if (Input.GetKeyDown(KeyCode.T)) _skillManager.UseSkill(KeyCode.T);
    }

    private void HandleItemInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) InventoryManager.Instance.UseItem(0);
        if (Input.GetKeyDown(KeyCode.S)) InventoryManager.Instance.UseItem(1);
        if (Input.GetKeyDown(KeyCode.D)) InventoryManager.Instance.UseItem(2);
        if (Input.GetKeyDown(KeyCode.Z)) InventoryManager.Instance.UseItem(3);
        if (Input.GetKeyDown(KeyCode.X)) InventoryManager.Instance.UseItem(4);
    }

    private void HandleMovement()
    {
        if (_playerCombat.State == PlayerCombat.PlayerState.Dodging)
            return;

        if (_playerCombat.State != PlayerCombat.PlayerState.Idle && !_playerStats.IsInvincible)
            return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundMask))
            {
                _destPos = hit.point;
                _move = true;
            }
        }
        _animator.SetBool("isRunning", _move);
    }

    private void FixedUpdate()
    {
        // 넉백 중이면 플레이어 이동 막기
        if (_playerStats.IsInvincible && _playerCombat.State == PlayerCombat.PlayerState.GettingHit)
            return;

        if (!_move) return;
        if (_playerCombat.State != PlayerCombat.PlayerState.Idle && !_playerStats.IsInvincible)
            return;

        Vector3 dir = _destPos - transform.position;
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);

        // 회전: 바라보는 방향으로 부드럽게 회전
        if (flatDir.sqrMagnitude > 0.001f)  // 방향이 0이 아닐 때만 회전
        {
            _lookTarget = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, _lookTarget, Time.deltaTime * 10f); // 회전 속도 조절
        }

        // 이동
        transform.position += flatDir.normalized * _speed * Time.deltaTime;

        // 목적지 도착 시 멈춤
        if (flatDir.magnitude <= 0.05f)
        {
            _move = false;
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((_groundMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            // 바닥이므로 충돌 무시
            return;
        }
        // 어떤 물체와 부딪히든 멈춤
        _move = false;
        _animator.SetBool("isRunning", false);
        //print("충돌");
    }

    public void StopMovement()
    {
        _move = false;
        _animator.SetBool("isRunning", false);
    }
}

