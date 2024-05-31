using Mirror;
using UnityEngine;

public class Guided : NetworkBehaviour
{
    [SerializeField] protected int EIRCM_Count;
    [SerializeField] protected VehicleCombat target;

    public System.Action OnRemove;
    public bool Target()
    {
        return target != null;
    }

    /// <summary>
    /// 유도 미사일의 타겟을 지정해주는 메서드
    /// </summary>
    /// <param name="target"></param>
    public virtual void SetTarget(Radar radar)
    {
        WeaponData weaponData = GetComponent<WeaponData>();
        if (radar.toTargetAngle <= weaponData.MaxSeekerAngle() && radar.toTargetDistance <= weaponData.LockOnRange())
        {
            this.target = radar.GetTarget();
            if (target != null)
            {
                target.onFlare += EIRCM;
                kjh.GameManager.Instance.AddMissile(transform);
            }
        }
    }

    MWR mwr;

    protected void AddMwr()
    {
        if (mwr == null && target.TryGetComponent<MWR>(out mwr))
        {
            //Debug.Log("미사일 추가");
            mwr.AddMissile(this);
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
            if (mwr != null)
            {
                mwr.RemoveMissile(this);
            }
        }
        this.target = null;
        OnRemove?.Invoke();
    }

    [SerializeField] protected float maxTurnRate;//최대 토크
    [SerializeField] protected float maxSideForce;//최대 기동력
    [SerializeField] protected bool isTVC;//추력 편향 노즐 여부
    [SerializeField] protected float traceAngleLimit;//추적 한계각. 이 각도를 넘어가면 추적을 중단함

    protected Rocket rocket;
    protected Rigidbody rigidbody;

    protected Vector3 targetVec;
    protected Vector3 angleError_temp;
    protected Vector3 orderAxis_Temp;

    protected float pGain = 3;
    protected float dGain = 200;

    // Start is called before the first frame update
    void Start()
    {
        rocket = GetComponent<Rocket>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Homing();
    }

    protected virtual void Homing()
    {
        if (target != null)
        {
            targetVec = target.transform.position;//타겟 벡터 지정

            Vector3 toTargetVec = targetVec - this.transform.position;
            if(toTargetVec.magnitude < 3000)
            {
                AddMwr();
            }

            Vector3 toTargetDir = toTargetVec.normalized;//방향 벡터 산출

            Vector3 angleError_diff = toTargetDir - angleError_temp;//방향 벡터의 변화량 (시선각 변화량)
            angleError_temp = toTargetDir;

            

            Vector3 side1 = toTargetDir;
            Vector3 side2 = toTargetDir + angleError_diff;
            Vector3 orderAxis = Vector3.Cross(side1, side2);

            Vector3 orderAxis_Diff = orderAxis - orderAxis_Temp;
            orderAxis_Temp = orderAxis;

            float velocity = rigidbody.velocity.magnitude;

            float availableTorqueRatio = (isTVC && rocket.isCombustion) ? 1 : Mathf.Clamp(velocity * 0.0015f, 0, 1);

            if (rocket.SideForce().magnitude < maxSideForce * maneuverability)
            {
                float p = (600) * pGain;
                float d = (600) * dGain;

                rigidbody.AddTorque(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate), ForceMode.Acceleration);
                //this.transform.Rotate(Vector3.ClampMagnitude((orderAxis * p + orderAxis_Diff * d) * availableTorqueRatio, maxTurnRate) * Time.fixedDeltaTime);
            }

            if (Vector3.Angle(this.transform.forward, toTargetDir) > traceAngleLimit)
            {
                RemoveTarget();
            }
        }
    }


    float maneuverability = 1;
    /// <summary>
    /// 자신이 목표하고 있는 타겟이 플레어를 살포하였을 때 실행하는 메서드
    /// </summary>
    public void EIRCM()
    {
        EIRCM_Count--;
        maneuverability *= 0.97f;

        if (EIRCM_Count <= 0) 
        {
            RemoveTarget();
        }
    }
}
