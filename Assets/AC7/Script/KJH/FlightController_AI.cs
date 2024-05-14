using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiState
{
    cruise,
    random,
    tracking
}

//현재 조종하는 항공기 조종면에 조종 데이터를 전달하는 클래스
public class FlightController_AI : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] AircraftControl aircraftControl;

    Rigidbody rigidbody;
    [SerializeField] Vector3 waypoint;

    AiState state;
    [SerializeField] Transform trakingTarget = null;
    [SerializeField][Range(5f, 10f)] float aiLevel;//난이도 설정. 5부터 10까지의 값을 가짐. 값이 클수록 기동을 더 빡세게 함
    /// <summary>
    /// 이 AI가 추적할 타겟(트랜스폼)을 지정하는 메서드
    /// </summary>
    /// <param name="target"></param>
    public void SetTrakingTarget(Transform target)
    {
        trakingTarget = target;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();

        state = AiState.random;

        CreateNewWayPoint_Normal();
        StartCoroutine(CreateWayPoint_Traking_Repeat());
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(waypoint, 100);
    }*/

    // Update is called once per frame
    void Update()
    {
        aircraftControl = aircraftSelecter.aircraftControl;

        if(trakingTarget != null && state != AiState.tracking)//추적할 트랜스폼이 존재할 경우
        {
            state = AiState.tracking;
        }

        if (state != AiState.tracking && WayPointOnCheck())//추적할 트랜스폼이 없으며, 웨이포인트에 도달했을 경우
        {
            CreateNewWayPoint_Normal();
        }

        ToWayPointMove();
        //aircraftControl.SetAxisValue(PlayerInputCustom.Instance.pitchAxis, PlayerInputCustom.Instance.rollAxis, PlayerInputCustom.Instance.yawAxis, PlayerInputCustom.Instance.throttleAxis);//테스트 코드
    }

    bool WayPointOnCheck()//웨이포인트와의 거리가 100 미만일 경우 true 반환
    {
        return (Vector3.Distance(waypoint, this.transform.position) < 100);        
    }

    void CreateNewWayPoint_Traking()
    {
        if (state == AiState.tracking)//추적 비행 모드
        {
            waypoint = trakingTarget.position;
        }
    }

    void CreateNewWayPoint_Normal()
    {
        if (state == AiState.cruise)//순항 비행 모드
        {
            waypoint = new Vector3(this.transform.forward.x, 0, this.transform.forward.z).normalized * 5000 + this.transform.position;
        }
        else//랜덤 비행 모드
        {
            waypoint = new Vector3(Random.Range(-10000, 10000), Random.Range(1000, 6000), Random.Range(-10000, 10000));
        }
    }

    IEnumerator CreateWayPoint_Traking_Repeat()
    {
        while(true)
        {
            yield return new WaitForSeconds(11 - aiLevel);
            CreateNewWayPoint_Traking();
        }
    }

    Vector3 localOrderTemp = Vector3.zero;
    float p = 0.7f;
    float d = 10;
    void ToWayPointMove()
    {
        Vector3 velocity = rigidbody.velocity;
        Vector3 toTargetVec = (waypoint - this.transform.position);

        float toTargetVecZRange = this.transform.InverseTransformDirection(toTargetVec).z;

        Vector3 velocityDiff = toTargetVec - velocity.normalized;
        Vector3 localOrder = this.transform.InverseTransformDirection(velocityDiff);
        Vector3 localOrderDiff = localOrder - localOrderTemp;
        localOrderTemp = localOrder;

        Vector3 localOrder_PD = Vector3.ClampMagnitude(localOrder * p + localOrderDiff * d, 1);

        float pitchOrder = 0;
        float rollOrder = 0;
        float yawOrder = 0;
        float throttleOrder = 1;

        if (localOrder_PD.y > -0.2f)
        {
            pitchOrder = localOrder_PD.y;
            yawOrder = localOrder_PD.x;
            rollOrder = localOrder_PD.x * 5;
            if (toTargetVecZRange > 0 || toTargetVec.magnitude > 3000)//타겟이 자신 앞에 있으며 거리가 멀 경우, 트래킹하며 수평 유지
            {
                if (Mathf.Abs(pitchOrder) < 0.1f && Mathf.Abs(yawOrder) < 0.1f)
                {
                    rollOrder = RollKeepLevel();
                }
            }
        }
        else
        {
            rollOrder = 1;
        }

        float maxMobility = aiLevel * 0.1f;
        pitchOrder = Mathf.Clamp(pitchOrder, -maxMobility, maxMobility);
        rollOrder = Mathf.Clamp(rollOrder, -1, 1);
        yawOrder = Mathf.Clamp(yawOrder, -1, 1);

        aircraftControl.SetAxisValue(pitchOrder, rollOrder, yawOrder, throttleOrder);
    }

    float RollKeepLevel()
    {
        if(this.transform.up.y < 0)
        {
            return Mathf.Clamp(this.transform.up.y * 30, -1, 1);
        }
        else
        {
            return Mathf.Clamp(this.transform.right.y * 2, -1, 1);
        }
    }
}
