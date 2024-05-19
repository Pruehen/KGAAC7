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

    /// <summary>
    /// ���� ���̴��� �������� Ʈ�������� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <returns></returns>
    public VehicleCombat GetTarget()
    {
        return lockOnTarget;
    }

    private void Start()
    {
        weaponSystem = GetComponent<AircraftMaster>().AircraftSelecter().weaponSystem;
    }

    private void Update()
    {
        if (lockOnTarget != null)
        {
            float toTargetAngle = Vector3.Angle(this.transform.forward, lockOnTarget.transform.position - this.transform.position);
            if (toTargetAngle <= weaponSystem.UseMissileSeekerAngle())
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

    /// <summary>
    /// ���̴��� ���� ����� ������ �޼���
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
                VehicleCombat itemTrf = targetList[i];

                float itemAngle = Vector3.Angle(this.transform.forward, itemTrf.transform.position - this.transform.position);
                if (itemAngle < angleTemp)
                {
                    targetTemp = itemTrf;
                    angleTemp = itemAngle;
                }

            }
            if (lockOnTarget != null)
            {
                lockOnTarget.isTargeted = false;
                lockOnTarget.isMissileLock = false;
                lockOnTarget.isRaderLock = false;
            }

            lockOnTarget = targetTemp;

            if(lockOnTarget != null)
            {
                lockOnTarget.isTargeted = true;
            }
        }
        else
        {
            lockOnTarget = kjh.GameManager.Instance.player.GetComponent<VehicleCombat>();
            lockOnTarget.isTargeted = true;
        }
    }
}
