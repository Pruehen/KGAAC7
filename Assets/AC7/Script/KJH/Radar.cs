using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] Transform targetGroupTrf;
    [SerializeField] Transform lockOnTarget;
    [SerializeField] float radarMaxAngle;
    int targetListCount;
    public List<Transform> targetList = new List<Transform>();

    /// <summary>
    /// 현재 레이더가 락온중인 트랜스폼을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public Transform GetTarget()
    {
        return lockOnTarget;
    }    
    void TargetListUpdate()
    {
        targetList.Clear();
        for (int i = 0; i < targetGroupTrf.childCount; i++)
        {
            targetList.Add(targetGroupTrf.GetChild(i));
        }
        targetListCount = targetGroupTrf.childCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetGroupTrf = GameObject.Find("Enemy_Transform").transform;        
        TargetListUpdate();
    }

    private void Update()
    {
        if (targetListCount != targetGroupTrf.childCount)
        {
            TargetListUpdate();
        }

        if (lockOnTarget != null)
        {
            /*if(Vector3.Angle(this.transform.forward, lockOnTarget.position - this.transform.position) > radarMaxAngle)
            {
                lockOnTarget = null;
            }*/
            if(lockOnTarget.GetComponent<VehicleCombat>().IsDead())
            {
                lockOnTarget = null;
            }
        }        
    }

    /*private void OnDrawGizmos()
    {
        if (lockOnTarget != null)
        {
            Gizmos.DrawSphere(lockOnTarget.position, 20);
        }
    }*/

    /// <summary>
    /// 레이더에 락온 명령을 내리는 메서드
    /// </summary>
    public void LockOn()
    {
        float angleTemp = radarMaxAngle;
        Transform targetTemp = null;
        for (int i = 0; i < targetListCount; i++)
        {
            Transform itemTrf = targetGroupTrf.GetChild(i);
            if (itemTrf.gameObject.activeSelf)
            {                
                float itemAngle = Vector3.Angle(this.transform.forward, itemTrf.position - this.transform.position);
                if (itemAngle < angleTemp)
                {
                    targetTemp = itemTrf;
                    angleTemp = itemAngle;
                }
            }
        }

        lockOnTarget = targetTemp;
    }
}
