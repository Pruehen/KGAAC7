using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : MonoBehaviour, IFightable
{    
    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    void IFightable.TakeDamage(float damage)
    {
        combat.TakeDamage(damage);
    }

    Combat combat = new Combat();

    private void Awake()
    {
        combat.Init(this.transform, 100);        
        combat.OnDead += Dead;
    }

    void Dead()
    {
        onDead.Invoke();
        Debug.Log("Æã");
    }

    public UnityEvent onDead;
}
