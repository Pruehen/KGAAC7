using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guided : MonoBehaviour
{
    [SerializeField] Transform targetTrf;

    [SerializeField] float maxTurnRate;//최대 토크
    [SerializeField] float maxSideForce;//최대 기동력
    [SerializeField] bool isTVC;//추력 편향 노즐 여부
    [SerializeField] float traceAngleLimit;//추적 한계각. 이 각도를 넘어가면 추적을 중단함

    Rocket rocket;
    Rigidbody rigidbody;

    Vector3 targetVec;
    Vector3 angleError_diff;
    Vector3 angleError_temp;

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

            angleError_diff = toTargetVec - angleError_temp;//방향 벡터의 변화량 (시선각 변화량)
            angleError_temp = toTargetVec;


            Vector3 diffedAE = angleError_diff;//시선각 변화량

            //Vector3 dieedAE_diff = diffedAE - orderTemp;
            //orderTemp = diffedAE;
            //Vector3 pnOrderVec = diffedAE * Kp + dieedAE_diff * Kd;//비례항법식


            Vector3 side1 = toTargetVec;
            Vector3 side2 = diffedAE;

            Vector3 orderAxis = Vector3.Cross(side1, side2);

            float availableTorqueRatio = (isTVC && rocket.isCombustion) ? 1 : Mathf.Clamp(rigidbody.velocity.magnitude * 0.002f, 0, 1);

            if (rocket.SideForce().magnitude < maxSideForce)
            {
                //rigidbody.AddTorque(Vector3.ClampMagnitude(orderAxis * availableTorqueRatio * 100, maxTorque), ForceMode.Acceleration);
                this.transform.Rotate(Vector3.ClampMagnitude(orderAxis * 1000 * availableTorqueRatio, maxTurnRate * Time.fixedDeltaTime));
            }

            if (Vector3.Angle(this.transform.forward, targetVec - this.transform.position) > traceAngleLimit)
            {
                targetTrf = null;                
            }
        }
    }
}
