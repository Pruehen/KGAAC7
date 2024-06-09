using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        bsj.GameManager.Instance.AfterPlayerSpawned += OnLocalPlayerSpawn;
    }
    
    private void OnLocalPlayerSpawn()
    {
        PlayerData data = FindAnyObjectByType<PlayerData>();
        if(data != null )
            CommandSetPlayerInfo(data.PlayerAircraft, data.PlayerId);
    }

    [Command]
    private void CommandSetPlayerInfo(string aircraftName, string nickname)
    {
        RpcSetPlayerInfo(aircraftName, nickname);
    }
    [ClientRpc]
    private void RpcSetPlayerInfo(string aircraftName, string nickname)
    {
        name = aircraftName;
        this.nickname = nickname;
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

        CommandSyncTargetInfo();

        if (isPlayer)
        {
            StartCoroutine(Heal());
        }
        kjh.GameManager.Instance.AddActiveTarget(this);
    }

    [Command]
    private void CommandSyncTargetInfo()
    {
        //서버로 신호 보내고 클라로 싱크 시켜주기
        RpcSyncTargetInfo(name, nickname);
    }
    [ClientRpc]
    private void RpcSyncTargetInfo(string aircraftName, string nickname)
    {
        name = aircraftName;
        this.nickname = nickname;
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
    [Command]
    public void CommandDead()
    {
        RpcDead();
    }
    [ClientRpc]
    void RpcDead()
    {
        Die();
        Dead();
    }

    void Dead()
    {
        Debug.Log("OnVehicleCombat Dead Called");
        if (!isPlayer)
        {
            SphereCollider sphereCollider;
            if (TryGetComponent<SphereCollider>(out sphereCollider))
            {
                sphereCollider.enabled = false;
            }
        }
        else if(isLocalPlayer)
        {
        }
        else
        {
            SubtitleManager.Instance.ShowSubtitle("Kill1");
        }
        kjh.GameManager.Instance.RemoveActiveTarget(this);
        onDead.Invoke();
        //Debug.Log("펑");
    }

    public void ResetDead()
    {
        gameObject.SetActive(true);
        SphereCollider sphereCollider;
        if (TryGetComponent<SphereCollider>(out sphereCollider))
        {
            sphereCollider.enabled = true;
        }
        combat.Reset();
        kjh.GameManager.Instance.AddActiveTarget(this);
    }

    private void OnDestroy()
    {
        Debug.Log("On VehicleCombat destroy");
        kjh.GameManager.Instance.RemoveActiveTarget(this);
        bsj.GameManager.Instance.AfterAnyPlayerSpawned -= OnAnyPlayerSpawn;
        bsj.GameManager.Instance.AfterPlayerSpawned -= OnLocalPlayerSpawn;
        onDestroyed?.Invoke();
    }


    [Command]
    public void CommandResetDead()
    {
        if (isServer)
        {
            RpcResetDead();
        }
    }
    [ClientRpc]
    private void RpcResetDead()
    {
        ResetDead();
    }


    /// <summary>
    /// 플레어가 살포되었을 때 실행되는 메서드
    /// </summary>
    public void FlareDeploy()
    {
        onFlare?.Invoke();
    }

    public UnityEvent onDead;
    public UnityEvent onDestroyed;
    public System.Action onFlare;
}
