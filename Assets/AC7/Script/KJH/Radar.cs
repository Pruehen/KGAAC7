using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{    
    [SerializeField] Transform lockOnTarget;
    [SerializeField] float radarMaxAngle;    
    [SerializeField] GameObject _lockOnSfxPrefab;    
    AudioSource _lockOnSfx;    

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
        if(_lockOnSfxPrefab != null )
        {
            GameObject item = Instantiate(_lockOnSfxPrefab);
            _lockOnSfx = item.GetComponent<AudioSource>();
        }

    }

    private void Update()
    {
        if (lockOnTarget != null)
        {
            /*if(Vector3.Angle(this.transform.forward, lockOnTarget.position - this.transform.position) > radarMaxAngle)
            {
                lockOnTarget = null;
            }*/
            if (lockOnTarget.GetComponent<VehicleCombat>().IsDead())
            {
                //lockOnTarget = null;
                StartCoroutine(NextTargetLock());
            }
            if (!_lockOnSfx.isPlaying)
            {
                _lockOnSfx?.Play();
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

    /// <summary>
    /// 레이더에 락온 명령을 내리는 메서드
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
