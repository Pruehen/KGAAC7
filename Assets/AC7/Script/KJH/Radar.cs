using kjh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{    
    VehicleCombat lockOnTarget;
    [SerializeField] float radarMaxAngle;
    [SerializeField] bool isEnemy;
    WeaponSystem weaponSystem;

    [SerializeField] GameObject _lockOnSfxPrefab;
    AudioSource _lockOnSfx;

    public float toTargetAngle { get; private set; }
    public float toTargetDistance { get; private set; }

    /// <summary>
    /// 현재 레이더가 락온중인 트랜스폼을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public VehicleCombat GetTarget()
    {
        return lockOnTarget;
    }

    private void Start()
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
            toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
            toTargetDistance = Vector3.Distance(this.transform.position, lockOnTarget.transform.position);

            if (!isEnemy)
            {
                WeaponData weaponData = weaponSystem.UseWeaponData();

                if (toTargetAngle <= weaponData.MaxSeekerAngle() && toTargetDistance <= weaponData.LockOnRange())
                {
                    lockOnTarget.isMissileLock = true;
                }
                else
                {
                    lockOnTarget.isMissileLock = false;
                }
                if (toTargetAngle <= radarMaxAngle)
                {
                    lockOnTarget.isRaderLock = true;
                }
                else
                {
                    lockOnTarget.isRaderLock = false;
                }

                if (lockOnTarget.GetComponent<VehicleCombat>().IsDead())
                {
                    //lockOnTarget = null;
                    StartCoroutine(NextTargetLock());
                }

                if (_lockOnSfx != null && !_lockOnSfx.isPlaying && lockOnTarget.isMissileLock)
                {
                    _lockOnSfx?.Play();
                    Debug.Log("소리");
                }
                else if (!lockOnTarget.isMissileLock)
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
        LockOn();
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
            float angleTemp = 200;
            VehicleCombat targetTemp = null;

            List<VehicleCombat> targetList = kjh.GameManager.Instance.activeTargetList;            

            for (int i = 0; i < targetList.Count; i++)
            {
                VehicleCombat item = targetList[i];

                float itemAngle = Vector3.Angle(this.transform.forward, item.transform.position - this.transform.position);
                if(itemAngle < 30 && !inRangeTargetList.Contains(item))
                {
                    inRangeTargetList.Add(item);
                }
                else if(itemAngle >= 30 && inRangeTargetList.Contains(item))
                {
                    inRangeTargetList.Remove(item);
                }
                if (itemAngle < angleTemp)
                {
                    targetTemp = item;
                    angleTemp = itemAngle;                    
                }
            }
            for (int i = 0; i < inRangeTargetList.Count; i++)
            {
                VehicleCombat item = inRangeTargetList[i];
                if (item == null)
                {
                    inRangeTargetList.Remove(item);
                    continue;
                }

                float itemAngle = Vector3.Angle(this.transform.forward, item.transform.position - this.transform.position);
                if (itemAngle >= 30)
                {
                    inRangeTargetList.Remove(item);
                }
            }

            if (lockOnTarget != null)
            {
                lockOnTarget.isTargeted = false;
                lockOnTarget.isMissileLock = false;
                lockOnTarget.isRaderLock = false;
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
                lockOnTarget.isTargeted = true;
                toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
                toTargetDistance = Vector3.Distance(this.transform.position, lockOnTarget.transform.position);
            }
        }
        else
        {
            lockOnTarget = kjh.GameManager.Instance.player.GetComponent<VehicleCombat>();
            lockOnTarget.isTargeted = true;
            toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
            toTargetDistance = Vector3.Distance(this.transform.position, lockOnTarget.transform.position);
        }
    }
}
