using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public LayerMask groundMask;

    private Animator m_animator;
    private Rigidbody m_rigidbody;
    private Vector3 destPos;
    private Quaternion lookTarget;

    private bool move = false;
    public bool canControl = true;

    private PlayerSkillManager skillManager;

    void Start()
    {
        skillManager = GetComponent<PlayerSkillManager>();

        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        HandleMovement();
        HandleSkillInput();
    }
    void HandleSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) skillManager.UseSkill(KeyCode.Q);
        if (Input.GetKeyDown(KeyCode.W)) skillManager.UseSkill(KeyCode.W);
        if (Input.GetKeyDown(KeyCode.E)) skillManager.UseSkill(KeyCode.E);
        if (Input.GetKeyDown(KeyCode.R)) skillManager.UseSkill(KeyCode.R);
        if (Input.GetKeyDown(KeyCode.T)) skillManager.UseSkill(KeyCode.T);
    }
    void HandleMovement()
    {
        if (!canControl) return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                destPos = hit.point;
                move = true;
            }
        }
        m_animator.SetBool("isRunning", move);
    }

    void FixedUpdate()
    {
        if (!move) return;

        Vector3 dir = destPos - transform.position;
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);

        // ȸ��: �ٶ󺸴� �������� �ε巴�� ȸ��
        if (flatDir.sqrMagnitude > 0.001f)  // ������ 0�� �ƴ� ���� ȸ��
        {
            lookTarget = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, Time.deltaTime * 10f); // ȸ�� �ӵ� ����
        }

        // �̵�
        transform.position += flatDir.normalized * speed * Time.deltaTime;

        // ������ ���� �� ����
        if (flatDir.magnitude <= 0.05f)
        {
            move = false;
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((groundMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            // �ٴ��̹Ƿ� �浹 ����
            return;
        }
        // � ��ü�� �ε����� ����
        move = false;
        m_animator.SetBool("isRunning", false);
        print("�浹");
    }
    public void StopMovement()
    {
        move = false;
        m_animator.SetBool("isRunning", false);
    }
}
