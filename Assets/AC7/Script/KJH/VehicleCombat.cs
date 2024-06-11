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


        if (!isLocalPlayer)
        {
            combat.Init(this.transform, startHp);
            isTargeted = false;
            isRaderLock = false;
            isMissileLock = false;
            CommandSyncTargetInfo();
        }
        else
        {
            PlayerData data = FindAnyObjectByType<PlayerData>();
            if (data != null)
                CommandSetPlayerInfo(data.PlayerAircraft, data.PlayerId);
            combat.Init(this.transform, startHp);
            isTargeted = false;
            isRaderLock = false;
            isMissileLock = false;
        }
        if (isServerOnly)
        {
            combat.OnDead += RpcDie;
        }
        combat.OnDead += OnAnyDead;


        if (isPlayer)
        {
            StartCoroutine(Heal());
        }
        kjh.GameManager.Instance.AddActiveTarget(this);
    }
    private void OnLocalPlayerSpawn()
    {

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

    public void TakeDamage(float damage, VehicleCombat owner)
    {
        if(isServer)
        {
            combat.TakeDamage(damage);
            if (IsDead())
            {
                if (owner == kjh.GameManager.Instance.player.vehicleCombat)
                {
                    SubtitleManager.Instance.ShowSubtitle("Kill1");
                }
            }
            RpcTakeDamage(damage, owner.netId);
        }
        CustomAI customAI;
        if(TryGetComponent<CustomAI>(out customAI))
        {
            customAI.EngageOrder();
        }
    }

    [ClientRpc]
    private void RpcTakeDamage(float damage, uint uid)
    {
        if(isClientOnly)
        {
            combat.TakeDamage(damage);


            if (IsDead())
            {
                VehicleCombat owner = NetworkClient.spawned[uid].GetComponentInChildren<VehicleCombat>();
                if (owner == kjh.GameManager.Instance.player.vehicleCombat)
                {
                    SubtitleManager.Instance.ShowSubtitle("Kill1");
                }
            }
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
    public void CommandDie()
    {
        RpcDie();
    }
    [ClientRpc]
    void RpcDie()
    {
        Die();
    }

    void OnAnyDead()
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
        kjh.GameManager.Instance.RemoveActiveTarget(this);
        onDead.Invoke();
        //Debug.Log("펑");
    }
    void OnLocalDead()
    {
        onLocalDead.Invoke();
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
    public UnityEvent onLocalDead;
    public UnityEvent onDestroyed;
    public System.Action onFlare;
}
