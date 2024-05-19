using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : MonoBehaviour, IFightable
{
    public bool mainTarget = false;
    public string name;
    public string nickname;
    public int targetedLevel;

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
        targetedLevel = 0;
    }

    public bool IsDead()
    {
        return combat.IsDead();
    }

    void Dead()
    {
        kjh.GameManager.Instance.RemoveActiveTarget(this);
        onDead.Invoke();
        //Debug.Log("��");
    }

    /// <summary>
    /// �÷�� �����Ǿ��� �� ����Ǵ� �޼���
    /// </summary>
    public void FlareDeploy()
    {
        onFlare?.Invoke();
    }

    public UnityEvent onDead;
    public System.Action onFlare;
}
