using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AircraftCombat : MonoBehaviour, IFightable
{
    Rigidbody rigidbody;
    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    void IFightable.TakeDamage(float damage)
    {
        if (combat.IsDead())
            return;

        combat.TakeDamage(damage);
        if(combat.IsDead())
        {
            Dead();
        }
    }

    Combat combat = new Combat();

    private void Awake()
    {
        combat.Init(this.transform, 100);
        rigidbody = GetComponent<Rigidbody>();
    }

    void Dead()
    {
        onDead.Invoke();
        Debug.Log("Æã");
    }

    public UnityEvent onDead;
}
