using kjh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{    
    [SerializeField] VehicleCombat lockOnTarget;
    [SerializeField] float radarMaxAngle;
    public float RadarMaxAngle() { return radarMaxAngle; }
    [SerializeField] bool isEnemy;
    WeaponSystem weaponSystem;

    [SerializeField] GameObject _lockOnSfxPrefab;
    AudioSource _lockOnSfx;

    public float toTargetAngle { get; private set; }
    public float toTargetDistance { get; private set; }
    bool onNextLockOn = false;

    public bool isMissileLock { get; private set; }
    public bool isRadarLock { get; private set; }

    /// <summary>
    /// 현재 레이더가 락온중인 트랜스폼을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public VehicleCombat GetTarget()
    {
        return lockOnTarget;
    }

    public void Init()
    {
        weaponSystem = GetComponent<AircraftMaster>().AircraftSelecter().weaponSystem;

        if (_lockOnSfxPrefab != null)
        {
            GameObject item = Instantiate(_lockOnSfxPrefab);
            _lockOnSfx = item.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (lockOnTarget != null)
        {
            if (lockOnTarget.IsDead() && !onNextLockOn)
            {
                //lockOnTarget = null;
                onNextLockOn = true;
                Debug.Log("적 사망에 따른 락온 발생");
                StartCoroutine(NextTargetLock());
            }

            SetTargetData();

            if (true)//this.isLocalPlayer)
            {
                if (_lockOnSfx != null && !_lockOnSfx.isPlaying && isMissileLock)
                {
                    _lockOnSfx?.Play();
                    //Debug.Log("소리");
                }
                else if (!isMissileLock)
                {
                    _lockOnSfx?.Pause();
                }
            }
        }
        else// if(this.isLocalPlayer)
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
            VehicleCombat thisVehicleCombat = this.gameObject.GetComponent<VehicleCombat>();

            for (int i = 0; i < targetList.Count; i++)
            {
                VehicleCombat item = targetList[i];

                if (item != null && thisVehicleCombat != item)
                {
                    float itemAngle = Vector3.Angle(Camera.main.transform.forward, item.transform.position - this.transform.position);
                    float itemDistance = Vector3.Distance(this.transform.position, item.transform.position);
                    if (itemAngle < 10 && !inRangeTargetList.Contains(item))
                    {
                        inRangeTargetList.Add(item);
                    }
                    else if (itemAngle >= 10 && inRangeTargetList.Contains(item))
                    {
                        inRangeTargetList.Remove(item);
                    }
                    if (itemDistance < distanceTemp && !item.IsDead())
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
                    Vector3.Angle(Camera.main.transform.forward, item.transform.position - this.transform.position) >= 10 || 
                    item.IsDead())
                {
                    inRangeTargetList.Remove(item);
                    i--;
                }
            }

            if (lockOnTarget != null)
            {
                isMissileLock = false;
                isRadarLock = false;
            }

            if (inRangeTargetList.Count == 0)
            {
                lockOnTarget = targetTemp;
            }
            else
            {
                if(iterator >= inRangeTargetList.Count)
                {
                    iterator = 0;
                }

                lockOnTarget = inRangeTargetList[iterator];
                iterator++;
            }

            if(lockOnTarget != null)
            {
                //lockOnTarget.isTargeted = true;
                SetTargetData();
            }                        
        }
        else
        {
            lockOnTarget = kjh.GameManager.Instance.player.GetComponent<VehicleCombat>();
            //lockOnTarget.isTargeted = true;
            toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
            toTargetDistance = Vector3.Distance(this.transform.position, lockOnTarget.transform.position);            
        }
        onNextLockOn = false;
    }

    void SetTargetData()
    {
        toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
        toTargetDistance = Vector3.Distance(this.transform.position, lockOnTarget.transform.position);

        if (!isEnemy)
        {
            WeaponData weaponData = weaponSystem.UseWeaponData();

            if (toTargetAngle <= weaponData.MaxSeekerAngle() && toTargetDistance <= weaponData.LockOnRange())
            {
                isMissileLock = true;
            }
            else
            {
                isMissileLock = false;
            }
            if (toTargetAngle <= radarMaxAngle)
            {
                isRadarLock = true;
            }
            else
            {
                isRadarLock = false;
            }
        }
    }
}
