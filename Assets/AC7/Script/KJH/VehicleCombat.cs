using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class VehicleCombat : NetworkBehaviour, IFightable
{
    public float startHp;
    public bool isPlayer;
    public bool mainTarget = false;
    public string name;
    public string nickname;

    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        //로컬일 경우 이펙트만 나오고 데미지는 입히지 않음
        if (this.isServer)
        {
            CommandTakeDamage(damage);//서버일 시 커맨드 메서드 실행
        }
    }
    [Command(requiresAuthority = false)]
    void CommandTakeDamage(float damage)
    {
        combat.TakeDamage(damage);//서버에서 데미지 계산
        CustomAI customAI;
        if (TryGetComponent<CustomAI>(out customAI))
        {
            customAI.EngageOrder();
        }

        RpcTakeDamage(damage);
    }

    [ClientRpc]
    void RpcTakeDamage(float dmg)
    {
        if (this.isServer)
            return;

        combat.TakeDamage(dmg);
        CustomAI customAI;
        if (TryGetComponent<CustomAI>(out customAI))
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
        //isTargeted = false;
        //isRaderLock = false;
        //isMissileLock = false;

        if(isPlayer)
        {
            //StartCoroutine(Heal());
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

        kjh.GameManager.Instance.RemoveActiveTarget(this);
        //SubtitleManager.Instance.ShowSubtitle("Kill1");

        if (TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        if (this.isLocalPlayer)
        {
            BGM_Player.Instance.Stop();
        }
        onDead.Invoke();
        //Debug.Log("펑");
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
