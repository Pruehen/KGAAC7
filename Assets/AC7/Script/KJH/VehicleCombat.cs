using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCombat : MonoBehaviour, IFightable
{
    public float startHp;
    public bool isPlayer;
    public bool mainTarget = false;
    public string name;
    public string nickname;
    bool _isInit = false;
    float lifeTime = 0;

    //[ClientCallback]
    public void SetNames(string nickName)
    {
        name = GetComponent<AircraftSelecter>().aircraftControl.name;
        this.nickname = nickName;
    }

    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        //������ ��� ����Ʈ�� ������ �������� ������ ����
        //if (this.isServer && lifeTime > 2)
        //{
        //    ServerOnlyTakeDamage(damage);//������ �� Ŀ�ǵ� �޼��� ����
        //}
        combat.TakeDamage(damage);
    }
    //public void TakeDamageExecuteCommand(float damage)
    //{
    //    CommandTakeDamage(damage);
    //}

    //[Server]
    //void ServerOnlyTakeDamage(float damage)
    //{
    //    combat.TakeDamage(damage);//�������� ������ ���
    //    //CustomAI customAI;
    //    //if (TryGetComponent<CustomAI>(out customAI))
    //    //{
    //    //    customAI.EngageOrder();
    //    //}

    //    RpcTakeDamage(damage);
    //}
    //[Command(requiresAuthority = false)]
    //void CommandTakeDamage(float damage)
    //{
    //    combat.TakeDamage(damage);
    //    RpcTakeDamage(damage);
    //}

    //[ClientRpc]
    //void RpcTakeDamage(float dmg)
    //{
    //    if (this.isServer)
    //        return;

    //    combat.TakeDamage(dmg);
    //    //CustomAI customAI;
    //    //if (TryGetComponent<CustomAI>(out customAI))
    //    //{
    //    //    customAI.EngageOrder();
    //    //}
    //}


    public void Die()
    {
        combat.Die();
    }

    Combat combat = new Combat();
    public Combat Combat() { return combat; }
    protected virtual void Awake()
    {        
        combat.OnDead += Dead;
        //combat.Init(this.transform, startHp);
    }
    private void Update()
    {
        lifeTime += Time.deltaTime;
    }
    public void Init()
    {        
        combat.Init(this.transform, startHp);
        onInit.Invoke();
        _isInit = true;
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

        //if (this.isLocalPlayer)
        //{
        //    //BGM_Player.Instance.Stop();
        //}
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
    public UnityEvent onInit;
    public System.Action onFlare;
}
