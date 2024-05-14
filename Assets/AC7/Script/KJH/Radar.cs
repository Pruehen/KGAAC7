using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
