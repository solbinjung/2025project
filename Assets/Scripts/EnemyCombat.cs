using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 공격 실행 (거리/충돌 체크는 HitBox에서 처리)
    public void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            //animator.SetTrigger("doAttack");
            lastAttackTime = Time.time;
        }
    }

    // 애니메이션 이벤트에서 호출
    public void DealDamage(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
    }
}
