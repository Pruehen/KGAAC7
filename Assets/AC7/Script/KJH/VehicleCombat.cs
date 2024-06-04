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
        //������ ��� ����Ʈ�� ������ �������� ������ ����
        if (this.isServer)
        {
            CommandTakeDamage(damage);//������ �� Ŀ�ǵ� �޼��� ����
        }
    }
    [Command(requiresAuthority = false)]
    void CommandTakeDamage(float damage)
    {
        combat.TakeDamage(damage);//�������� ������ ���
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
