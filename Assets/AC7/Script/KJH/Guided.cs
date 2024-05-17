using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guided : MonoBehaviour
{
    [SerializeField] int EIRCM_Count;
    VehicleCombat target;

    /// <summary>
    /// 유도 미사일의 타겟을 지정해주는 메서드
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(VehicleCombat target)
    {
        if (target != null)
        {
            this.target = target;
            target.onFlare += EIRCM;
        }
    }

    /// <summary>
    /// 유도 미사일의 타겟을 해제해주는 메서드 (타겟을 잃었거나, 미사일이 충돌했거나)
    /// </summary>
    public void RemoveTarget()
    {
        if(target != null)
        {
            target.onFlare -= EIRCM;
        }
        this.target = null;        
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

    float pGain = 3;
    float dGain = 200;

    // Start is called before the first frame update
    void Start()
    {
        rocket = GetComponent<Rocket>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null)
        {
            targetVec = target.transform.position;//타겟 벡터 지정

            Vector3 toTargetVec = (targetVec - this.transform.position).normalized;//방향 벡터 산출

            Vector3 angleError_diff = toTargetVec - angleError_temp;//방향 벡터의 변화량 (시선각 변화량)
            angleError_temp = toTargetVec;


            Vector3 side1 = toTargetVec;
            Vector3 side2 = toTargetVec + angleError_diff;
            Vector3 orderAxis = Vector3.Cross(side1, side2);

            Vector3 orderAxis_Diff = orderAxis - orderAxis_Temp;
            orderAxis_Temp = orderAxis;

            float velocity = rigidbody.velocity.magnitude;

            float availableTorqueRatio = (isTVC && rocket.isCombustion) ? 1 : Mathf.Clamp(velocity * 0.0015f, 0, 1);

            if (rocket.SideForce().magnitude < maxSideForce)
            {
                float p = (600) * pGain;
                float d = (600) * dGain;

                rigidbody.AddTorque(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate), ForceMode.Acceleration);
                //this.transform.Rotate(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate) * Time.fixedDeltaTime);
            }

            if (Vector3.Angle(this.transform.forward, targetVec - this.transform.position) > traceAngleLimit)
            {
                RemoveTarget();
            }
        }
    }

    /// <summary>
    /// 자신이 목표하고 있는 타겟이 플레어를 살포하였을 때 실행하는 메서드
    /// </summary>
    public void EIRCM()
    {
        EIRCM_Count--;

        if(EIRCM_Count <= 0) 
        {
            RemoveTarget();
        }
    }
}
