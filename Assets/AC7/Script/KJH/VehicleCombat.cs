using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : NetworkBehaviour, IFightable
{
    public float startHp;
    public bool isPlayer;
    public bool mainTarget = false;
    public string name;
    public string nickname;
    public bool isTargeted;
    public bool isRaderLock;
    public bool isMissileLock;

    private void Start()
    {
        bsj.GameManager.Instance.AfterAnyPlayerSpawned += OnAnyPlayerSpawn;
    }

    private void OnAnyPlayerSpawn()
    {
        combat.Init(this.transform, startHp);
        if (isServerOnly)
        {
            combat.OnDead += RpcDead;
            combat.OnDead += Dead;
        }
        if (isClient && isServer)
        {
            combat.OnDead += RpcDead;
        }
        isTargeted = false;
        isRaderLock = false;
        isMissileLock = false;

        if (isPlayer)
        {
            StartCoroutine(Heal());
        }
        kjh.GameManager.Instance.AddActiveTarget(this);
    }

    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        if(isServer)
        {
            combat.TakeDamage(damage);
            RpcTakeDamage(damage);
        }
        CustomAI customAI;
        if(TryGetComponent<CustomAI>(out customAI))
        {
            customAI.EngageOrder();
        }
    }

    [ClientRpc]
    private void RpcTakeDamage(float damage)
    {
        if(isClientOnly)
        {
            combat.TakeDamage(damage);
        }
    }

    public void Die()
    {
        combat.Die();
    }

    Combat combat = new Combat();
    public Combat Combat() { return combat; }


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

    [ClientRpc]
    void RpcDead()
    {
        Die();
        Dead();
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
        else if(isLocalPlayer)
        {
            BGM_Player.Instance.Stop();
        }
        onDead.Invoke();
        //Debug.Log("펑");
    }

    public void ResetDead()
    {
        SphereCollider sphereCollider;
        if (TryGetComponent<SphereCollider>(out sphereCollider))
        {
            sphereCollider.enabled = true;
        }
        combat.Reset();
    }

    /// <summary>
    /// 플레어가 살포되었을 때 실행되는 메서드
    /// </summary>
    public void FlareDeploy()
    {
        onFlare?.Invoke();
    }

    public UnityEvent onDead;
    public System.Action onFlare;
}
