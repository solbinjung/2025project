using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public CharacterStats Stats { get; private set; }
    public int damage = 10;

    private void Awake()
    {
        Stats = GetComponent<CharacterStats>();
    }

    public void Attack(CharacterStats target)
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
