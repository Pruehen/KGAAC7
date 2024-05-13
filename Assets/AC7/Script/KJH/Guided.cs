using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guided : MonoBehaviour
{
    [SerializeField] Transform targetTrf;

    /// <summary>
    /// 유도 미사일의 타겟을 지정해주는 메서드
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        targetTrf = target;
    }

    [SerializeField] float maxTurnRate;//최대 토크
    [SerializeField] float maxSideForce;//최대 기동력
    [SerializeField] bool isTVC;//추력 편향 노즐 여부
    [SerializeField] float traceAngleLimit;//추적 한계각. 이 각도를 넘어가면 추적을 중단함

    Rocket rocket;
    Rigidbody rigidbody;

    Vector3 targetVec;
    Vector3 angleError_temp;
    Vector3 orderAxis_Temp;

    float p = 2000;
    float d = 30000;

    // Start is called before the first frame update
    void Start()
    {
        rocket = GetComponent<Rocket>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(targetTrf != null)
        {
            targetVec = targetTrf.position;//타겟 벡터 지정

            Vector3 toTargetVec = (targetVec - this.transform.position).normalized;//방향 벡터 산출

            Vector3 angleError_diff = toTargetVec - angleError_temp;//방향 벡터의 변화량 (시선각 변화량)
            angleError_temp = toTargetVec;


            Vector3 side1 = toTargetVec;
            Vector3 side2 = toTargetVec + angleError_diff;
            Vector3 orderAxis = Vector3.Cross(side1, side2);

            Vector3 orderAxis_Diff = orderAxis - orderAxis_Temp;
            orderAxis_Temp = orderAxis;

            float availableTorqueRatio = (isTVC && rocket.isCombustion) ? 1 : Mathf.Clamp(rigidbody.velocity.magnitude * 0.001f, 0, 1);

            if (rocket.SideForce().magnitude < maxSideForce)
            {
                rigidbody.AddTorque(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate), ForceMode.Acceleration);
                //this.transform.Rotate(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate) * Time.fixedDeltaTime);
            }

            if (Vector3.Angle(this.transform.forward, targetVec - this.transform.position) > traceAngleLimit)
            {
                targetTrf = null;                
            }
        }
    }
}
