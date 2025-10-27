using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    private PlayerCombat _owner;
    private HashSet<Collider> _hitThisSwing = new HashSet<Collider>();

    public void Initialize(PlayerCombat owner)
    {
        _owner = owner;
    }

    public void ResetHitCache()
    {
        _hitThisSwing.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_owner == null || !_owner.IsAttackActive) return;
        if (_hitThisSwing.Contains(other)) return;

        EnemyStats targetStats = other.GetComponentInParent<EnemyStats>();
        if (targetStats != null && targetStats != _owner.Stats)
        {
            Vector3 hitDirection = (targetStats.transform.position - transform.position).normalized;
            targetStats.TakeDamage(_owner.Damage, hitDirection);
            _hitThisSwing.Add(other);
        }
    }
}

