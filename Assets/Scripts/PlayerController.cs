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
        // 왼쪽 마우스 버튼 클릭시
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;       
            
            
            // ray와 닿은 물체가 있는지 검사
            if(Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                print("Ground clicked" + hit.transform.name);

                // hit.point는 마우스 클릭한 곳의 월드 좌표
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
            // 현재 위치와 목표 위치의 방향 벡터
            dir = destPos - transform.position;
            // 바라 보아야 할 곳의 Quaternion
            lookTarget = Quaternion.LookRotation(dir);
            // 플레이어가 이동할 방향으로 Time.deltaTime * speed의 속도로 움직임
            transform.position += dir.normalized * Time.deltaTime * speed;
            // 현재 방향에서 움직여야 할 방향으로 부드럽게 회전
            transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, 0.25f);
            
            // 일정 거리 이내면 목표 도달로 간주
            if(dir.magnitude <= 0.05f)
            {
                move = false;
            }

            // isRunning 애니메이션
            m_Animator.SetBool("isRunning", move);
        }
    }
}
