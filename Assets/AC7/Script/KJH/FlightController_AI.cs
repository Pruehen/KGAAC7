using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 조종하는 항공기 조종면에 조종 데이터를 전달하는 클래스
public class FlightController_AI : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    AircraftControl aircraftControl;

    Rigidbody rigidbody;
    Vector3 waypoint;

    bool isDead = false;  
    [SerializeField][Range(3f, 10f)] float aiLevel;//난이도 설정. 3부터 10까지의 값을 가짐. 값이 클수록 기동을 더 빡세게 함

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();

        isDead = false;

        CreateNewWayPoint_Forward();        
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(waypoint, 100);
    }*/

    // Update is called once per frame
    void Update()
    {
        aircraftControl = aircraftSelecter.aircraftControl;
        if(isDead)//사망 시 조종 기능을 잠금
        {
            aircraftControl.SetAxisValue(0, 1, 0, 0);
            return;
        }

        if (WayPointOnCheck())//웨이포인트에 도달했을 경우
        {
            CreateNewWayPoint_Cruise();
        }

        ToWayPointMove();        
    }

    bool WayPointOnCheck()//웨이포인트와의 거리가 200 미만일 경우 true 반환
    {
        return (Vector3.Distance(waypoint, this.transform.position) < 200);        
    }

    /// <summary>
    /// 순항 비행하는 웨이포인트를 생성
    /// </summary>
    public void CreateNewWayPoint_Cruise()
    {
        waypoint = this.transform.position + new Vector3(this.transform.forward.x, 0, this.transform.forward.z).normalized * 5000;
    }

    /// <summary>
    /// 직진 비행하는 웨이포인트를 생성
    /// </summary>
    public void CreateNewWayPoint_Forward()
    {
        waypoint = this.transform.position + this.transform.forward * 1000;
    }

    /// <summary>
    /// 특정 범위 내에서 랜덤 비행하는 웨이포인트를 생성
    /// </summary>
    public void CreateNewWayPoint_Random(Vector3 center, float range)
    {
        waypoint = center + new Vector3(Random.Range(-range, range), Random.Range(1000, 6000), Random.Range(-range, range));
    }

    /// <summary>
    /// 특정 지점으로 이동하는 웨이포인트를 생성
    /// </summary>
    /// <param name="position"></param>
    public void CreateNewWayPoint_Position(Vector3 position)
    {
        waypoint = position;
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

        if((this.transform.position + velocity * (16 - aiLevel)).y < 0)//6~13초 후의 예상 고도가 음수일 경우
        {
            rollOrder = RollKeepLevel();
            pitchOrder = 1;
            //Debug.Log("지상 회피");
        }

        pitchOrder = Mathf.Clamp(pitchOrder, -maxMobility, maxMobility);
        rollOrder = Mathf.Clamp(rollOrder, -1, 1);
        yawOrder = Mathf.Clamp(yawOrder, -1, 1);

        aircraftControl.SetAxisValue(pitchOrder, rollOrder, yawOrder, throttleOrder);
    }

    float RollKeepLevel()
    {
        if(this.transform.up.y < 0)
        {
            return Mathf.Clamp(this.transform.up.y * 20, -1, 1);
        }
        else
        {
            return Mathf.Clamp(this.transform.right.y * 2, -1, 1);
        }
    }

    /// <summary>
    /// 사망시 호출하는 메서드
    /// </summary>
    public void Dead()
    {
        isDead = true;
    }
}
