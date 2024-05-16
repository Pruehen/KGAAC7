using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{    
    [SerializeField] Transform lockOnTarget;
    [SerializeField] float radarMaxAngle;    

    /// <summary>
    /// ���� ���̴��� �������� Ʈ�������� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <returns></returns>
    public Transform GetTarget()
    {
        return lockOnTarget;
    }    

    // Start is called before the first frame update
    void Start()
    {        

    }

    private void Update()
    {
        if (lockOnTarget != null)
        {
            /*if(Vector3.Angle(this.transform.forward, lockOnTarget.position - this.transform.position) > radarMaxAngle)
            {
                lockOnTarget = null;
            }*/
            if(lockOnTarget.GetComponent<VehicleCombat>().IsDead())
            {
                //lockOnTarget = null;
                StartCoroutine(NextTargetLock());
            }
        }        
    }

    IEnumerator NextTargetLock()
    {
        yield return new WaitForSeconds(1);
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
        float angleTemp = radarMaxAngle;
        Transform targetTemp = null;

        List<VehicleCombat> targetList = kjh.GameManager.Instance.activeTargetList;
        for (int i = 0; i < targetList.Count; i++)
        {
            VehicleCombat itemTrf = targetList[i];

            float itemAngle = Vector3.Angle(this.transform.forward, itemTrf.transform.position - this.transform.position);
            if (itemAngle < angleTemp)
            {
                targetTemp = itemTrf.transform;
                angleTemp = itemAngle;
            }

        }

        lockOnTarget = targetTemp;
    }
}
