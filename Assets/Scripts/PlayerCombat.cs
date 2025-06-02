using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    public bool canCombat = true;
    public float attackCooldown = 0.5f;
    public float dodgeDistance = 2f;
    public float dodgeDuration = 0.3f;
    public float blockDuration = 1.0f;

    private bool isDodging = false;
    private bool isBlocking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private Vector3 dodgeDirection;

    private PlayerController m_playerController;
    private Animator m_animator;

    void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canCombat || isDodging || isBlocking) return;

        HandleInput();
    }
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) // ��Ŭ��: ����
        {
            TryAttack();
        }
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽�: ȸ��
        {
            StartCoroutine(Dodge());
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) // ���� ����Ʈ: ���
        {
            StartCoroutine(Block());
        }
    }
    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack0");
        Debug.Log("�⺻ ����");
    }
    IEnumerator Dodge()
    {
        isDodging = true;
        dodgeDirection = transform.forward;

        m_animator.SetTrigger("isDodging");
        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            transform.position += dodgeDirection.normalized * (dodgeDistance / dodgeDuration) * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }
    IEnumerator Block()
    {
        isBlocking = true;
        m_animator.SetBool("isBlocking", true);
        Debug.Log("��� ����");

        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        m_animator.SetBool("isBlocking", false);
        Debug.Log("��� ����");
    }
    public bool IsBlocking() => isBlocking;
    public bool IsDodging() => isDodging;
}