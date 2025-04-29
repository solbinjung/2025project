using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 destPos;
    Vector3 dir;
    Quaternion lookTarget;
    public float speed = 1.0f;
    public LayerMask groundMask;

    bool move = false;

    Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }
    void Update()
    {
        // ���� ���콺 ��ư Ŭ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;       
            
            
            // ray�� ���� ��ü�� �ִ��� �˻�
            if(Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                print("Ground clicked" + hit.transform.name);

                // hit.point�� ���콺 Ŭ���� ���� ���� ��ǥ
                destPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                
                move = true;
            }
        }
        Move();
    }
    void Move()
    {
        if (move)
        {
            // ���� ��ġ�� ��ǥ ��ġ�� ���� ����
            dir = destPos - transform.position;
            // �ٶ� ���ƾ� �� ���� Quaternion
            lookTarget = Quaternion.LookRotation(dir);
            // �÷��̾ �̵��� �������� Time.deltaTime * speed�� �ӵ��� ������
            transform.position += dir.normalized * Time.deltaTime * speed;
            // ���� ���⿡�� �������� �� �������� �ε巴�� ȸ��
            transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, 0.25f);
            
            // ���� �Ÿ� �̳��� ��ǥ ���޷� ����
            if(dir.magnitude <= 0.05f)
            {
                move = false;
            }

            // isRunning �ִϸ��̼�
            m_Animator.SetBool("isRunning", move);
        }
    }
}
