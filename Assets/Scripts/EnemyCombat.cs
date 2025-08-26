using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public CharacterStats Stats { get; private set; }
    [SerializeField] private int damage = 10;

    private CharacterStats playerStats; 

    private void Awake()
    {
        Stats = GetComponent<CharacterStats>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerStats = playerObj.GetComponent<CharacterStats>();
        else
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
    }

    public void Attack()
    {
        if (playerStats != null)
            playerStats.TakeDamage(damage);
    }
}
