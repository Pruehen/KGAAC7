using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : MonoBehaviour, IFightable
{
    public float startHp;
    public bool isPlayer;
    public bool mainTarget = false;
    public string name;
    public string nickname;
    public bool isTargeted;
    public bool isRaderLock;
    public bool isMissileLock;

    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    void IFightable.TakeDamage(float damage)
    {
        if (isPlayer)
            damage *= 0.3f;

        combat.TakeDamage(damage);
        CustomAI customAI;
        if(TryGetComponent<CustomAI>(out customAI))
        {
            customAI.EngageOrder();
        }
    }

    Combat combat = new Combat();

    private void Awake()
    {
        combat.Init(this.transform, startHp);        
        combat.OnDead += Dead;
        isTargeted = false;
        isRaderLock = false;
        isMissileLock = false;
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
