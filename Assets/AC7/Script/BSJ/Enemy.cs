using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IFightable
{
    Combat _combat = new Combat();

    private void OnEnable()
    {
        _combat.Init(transform, 100f);
    }

    public void DealDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        _combat.TakeDamage(damage);
    }
}
