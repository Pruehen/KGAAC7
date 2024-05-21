using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guided_SARH : Guided
{
    Radar radar;

    // Start is called before the first frame update
    /*void Start()
    {
        rocket = GetComponent<Rocket>();
        rigidbody = GetComponent<Rigidbody>();
    }*/

    /// <summary>
    /// 유도 미사일의 타겟을 지정해주는 메서드
    /// </summary>
    /// <param name="target"></param>
    public override void SetTarget(Radar radar)
    {
        WeaponData weaponData = GetComponent<WeaponData>();
        this.radar = radar;
        if (radar.toTargetAngle <= weaponData.MaxSeekerAngle() && radar.toTargetDistance <= weaponData.LockOnRange())
        {
            this.target = radar.GetTarget();
            if (target != null)
            {
                target.onFlare += EIRCM;

                MWR mwr;
                if (target.TryGetComponent<MWR>(out mwr))
                {
                    mwr.AddMissile(this);
                }
            }
        }
    }

    protected override void Homing()
    {
        if (target != null)
        {
            target = radar.GetTarget();
            if (radar.toTargetAngle > radar.RadarMaxAngle())
                return;

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
        }
    }
}

