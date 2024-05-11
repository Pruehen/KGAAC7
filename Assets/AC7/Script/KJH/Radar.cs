using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Radar : MonoBehaviour
{
    [SerializeField] Transform targetGroupTrf;
    [SerializeField] Transform lockOnTarget;
    [SerializeField] float radarMaxAngle;

    /// <summary>
    /// 현재 레이더가 락온중인 트랜스폼을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public Transform GetTarget()
    {
        return lockOnTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetGroupTrf = GameObject.Find("Enemy_Transform").transform;
    }

    private void Update()
    {
        if(lockOnTarget != null && Vector3.Angle(this.transform.forward, lockOnTarget.position - this.transform.position) > radarMaxAngle)
        {
            lockOnTarget = null;
        }
    }

    public void LockOn()
    {
        float angleTemp = radarMaxAngle;
        Transform targetTemp = null;
        for (int i = 0; i < targetGroupTrf.childCount; i++)
        {
            Transform item = targetGroupTrf.GetChild(i).transform;
            float itemAngle = Vector3.Angle(this.transform.forward, item.position - this.transform.position);
            if (itemAngle < angleTemp)
            {
                targetTemp = item;
                angleTemp = itemAngle;
            }
        }

        lockOnTarget = targetTemp;
    }
}
