using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : MonoBehaviour, IFightable
{
    public bool mainTarget = false;
    public string name;
    public string nickname;
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

    public bool IsDead()
    {
        return combat.IsDead();
    }

    void Dead()
    {
        kjh.GameManager.Instance.RemoveActiveTarget(this);
        onDead.Invoke();
        onDeadWithSelf.Invoke(this);
        //Debug.Log("Æã");
    }

    public UnityEvent onDead;
    public UnityEvent<VehicleCombat> onDeadWithSelf;
}
