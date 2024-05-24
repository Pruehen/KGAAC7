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

    public void TakeDamage(float damage)
    {
        combat.TakeDamage(damage);
        CustomAI customAI;
        if(TryGetComponent<CustomAI>(out customAI))
        {
            customAI.EngageOrder();
        }
    }

    public void Die()
    {
        combat.Die();
    }

    Combat combat = new Combat();
    public Combat Combat() { return combat; }
    protected virtual void Awake()
    {
        combat.Init(this.transform, startHp);        
        combat.OnDead += Dead;
        isTargeted = false;
        isRaderLock = false;
        isMissileLock = false;

        if(isPlayer)
        {
            StartCoroutine(Heal());
        }
    }
    IEnumerator Heal()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            if (!IsDead())
            {
                combat.Heal(5);
            }
            else
            {
                yield break;
            }
        }
    }

    public bool IsDead()
    {
        return combat.IsDead();
    }

    void Dead()
    {
        if (!isPlayer)
        {
            kjh.GameManager.Instance.RemoveActiveTarget(this);
            SubtitleManager.Instance.ShowSubtitle("Kill1");
            SphereCollider sphereCollider;
            if (TryGetComponent<SphereCollider>(out sphereCollider))
            {
                sphereCollider.enabled = false;
            }
        }
        else
        {
            BGM_Player.Instance.Stop();
        }
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

    private void OnDestroy()
    {
        kjh.GameManager.Instance.RemoveActiveTarget(this);
    }

    public UnityEvent onDead;
    public System.Action onFlare;
}
