using kjh;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : NetworkBehaviour
{
    VehicleCombat _lockonTarget;
    VehicleCombat lockonTarget
    {
        get
        {
            return _lockonTarget;
        }
        set
        {
            _lockonTarget = value;
            if(isServer)
            {
                if(value != null)
                {
                    RpcSetLockonTarget(value.netId);
                }
                else
                {
                    RpcSetLockonTargetNull();
                }
            }
        }
    }

    [ClientRpc]
    private void RpcSetLockonTarget(uint netId)
    {
        NetworkIdentity target = NetworkClient.spawned[netId];
        _lockonTarget = target.GetComponent<VehicleCombat>();
        if (_lockonTarget == null)
        {
            _lockonTarget = target.GetComponentInChildren<VehicleCombat>();
        }
    }
    [ClientRpc]
    private void RpcSetLockonTargetNull()
    {
        _lockonTarget = null;
    }

    [SerializeField] float radarMaxAngle;
    public float RadarMaxAngle() { return radarMaxAngle; }
    [SerializeField] bool isEnemy;
    WeaponSystem weaponSystem;

    [SerializeField] GameObject _lockOnSfxPrefab;
    AudioSource _lockOnSfx;

    private bool _isRadarLock = false;
    private bool _isMissileLock = false;

    public float toTargetAngle { get; private set; }
    public float toTargetDistance { get; private set; }
    public bool IsMissileLock 
    { get => _isMissileLock;
        set
        {
            _isMissileLock = value;
            if(isServer)
            {
                RpcSetIsMissileLock(value);
            }
        }
    }

    [ClientRpc]
    void RpcSetIsRadarLock(bool isRadarLock)
    {
        _isRadarLock = isRadarLock;
    }

    public bool IsRadarLock 
    { get => _isRadarLock; 
        set
        {
            _isRadarLock = value;
            if(isServer)
            {
                RpcSetIsRadarLock(value);
            }
        }
    }

    [ClientRpc]
    void RpcSetIsMissileLock(bool isMissileLock)
    {
        _isMissileLock = isMissileLock;
    }

    bool onNextLockOn = false;
    /// <summary>
    /// 현재 레이더가 락온중인 트랜스폼을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public VehicleCombat GetTarget()
    {
        return lockonTarget;
    }

    private void Start()
    {
        weaponSystem = GetComponent<AircraftMaster>()?.AircraftSelecter().weaponSystem;

        if (_lockOnSfxPrefab != null)
        {
            GameObject item = Instantiate(_lockOnSfxPrefab);
            _lockOnSfx = item.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (lockonTarget != null)
        {
            toTargetAngle = Vector3.Angle(this.transform.forward, lockonTarget.transform.position - this.transform.position);
            toTargetDistance = Vector3.Distance(this.transform.position, lockonTarget.transform.position);

            if (!isEnemy)
            {
                WeaponData weaponData = weaponSystem.UseWeaponData();

                if (lockonTarget.IsDead() && !onNextLockOn)
                {
                    //lockOnTarget = null;
                    onNextLockOn = true;
                    Debug.Log("적 사망에 따른 락온 발생");
                    StartCoroutine(NextTargetLock());
                }

                if (toTargetAngle <= weaponData.MaxSeekerAngle() && toTargetDistance <= weaponData.LockOnRange())
                {
                    lockonTarget.isMissileLock = true;
                    IsMissileLock = true;
                }
                else
                {
                    lockonTarget.isMissileLock = false;
                    IsMissileLock = false;
                }
                if (toTargetAngle <= radarMaxAngle)
                {
                    lockonTarget.isRaderLock = true;
                    IsRadarLock = true;
                }
                else
                {
                    lockonTarget.isRaderLock = false;
                    IsRadarLock = false;
                }

                if (_lockOnSfx != null && !_lockOnSfx.isPlaying && lockonTarget.isMissileLock)
                {
                    _lockOnSfx?.Play();
                    //Debug.Log("소리");
                }
                else if (!lockonTarget.isMissileLock)
                {
                    _lockOnSfx?.Pause();
                }
            }
        }
        else
        {
            _lockOnSfx?.Pause();
        }
    }

    IEnumerator NextTargetLock()
    {
        yield return new WaitForSeconds(2);
        if (onNextLockOn)
        {
            Debug.Log("적 사망에 따른 락온 발생");
            LockOn();
        }
    }

    /*private void OnDrawGizmos()
    {
        if (lockOnTarget != null)
        {
            Gizmos.DrawSphere(lockOnTarget.position, 20);
        }
    }*/
    List<VehicleCombat> inRangeTargetList = new List<VehicleCombat>();
    int iterator = 0;
    /// <summary>
    /// 레이더에 락온 명령을 내리는 메서드
    /// </summary>
    public void LockOn()
    {
        if (!isEnemy)
        {
            float distanceTemp = float.MaxValue;
            VehicleCombat targetTemp = null;

            List<VehicleCombat> targetList = kjh.GameManager.Instance.activeTargetList;            

            for (int i = 0; i < targetList.Count; i++)
            {
                VehicleCombat item = targetList[i];

                if (item != null)
                {
                    float itemAngle = Vector3.Angle(transform.forward, item.transform.position - this.transform.position);
                    float itemDistance = Vector3.Distance(this.transform.position, item.transform.position);
                    if (itemAngle < 10 && !inRangeTargetList.Contains(item) && item.transform != transform)
                    {
                        inRangeTargetList.Add(item);
                    }
                    else if (itemAngle >= 10 && inRangeTargetList.Contains(item))
                    {
                        inRangeTargetList.Remove(item);
                    }
                    if (itemDistance < distanceTemp && !item.IsDead() && item.transform != transform)
                    {
                        targetTemp = item;
                        distanceTemp = itemDistance;
                    }
                }
            }
            for (int i = 0; i < inRangeTargetList.Count; i++)
            {
                VehicleCombat item = inRangeTargetList[i];
                
                if (item == null || 
                    Vector3.Angle(this.transform.forward, item.transform.position - this.transform.position) >= 10 || 
                    item.IsDead())
                {
                    inRangeTargetList.Remove(item);
                    i--;
                }
            }

            if (lockonTarget != null)
            {
                lockonTarget.isTargeted = false;
                lockonTarget.isMissileLock = false;
                IsMissileLock = false;
                lockonTarget.isRaderLock = false;
                IsRadarLock = false;
            }

            if (inRangeTargetList.Count == 0)
            {
                lockonTarget = targetTemp;
            }
            else
            {
                if(iterator >= inRangeTargetList.Count)
                {
                    iterator = 0;
                }

                lockonTarget = inRangeTargetList[iterator];
                iterator++;
            }

            if(lockonTarget != null)
            {
                lockonTarget.isTargeted = true;
                toTargetAngle = Vector3.Angle(this.transform.forward, lockonTarget.transform.position - this.transform.position);
                toTargetDistance = Vector3.Distance(this.transform.position, lockonTarget.transform.position);               
            }                        
        }
        else
        {
            lockonTarget = kjh.GameManager.Instance.player.GetComponent<VehicleCombat>();
            lockonTarget.isTargeted = true;
            toTargetAngle = Vector3.Angle(this.transform.forward, lockonTarget.transform.position - this.transform.position);
            toTargetDistance = Vector3.Distance(this.transform.position, lockonTarget.transform.position);            
        }
        onNextLockOn = false;
    }
}
