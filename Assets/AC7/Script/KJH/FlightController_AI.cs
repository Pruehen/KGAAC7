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
    [SerializeField][Range(3f, 10f)] float aiLevel;//난이도 설정. 3부터 10까지의 값을 가짐. 값이 클수록 기동을 더 강하게 함

    float targetSpeed;
    public void SetTargetSpeed(float value)
    {
        targetSpeed = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();

        isDead = false;

        //CreateNewWayPoint_Forward();
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
            aircraftControl.SetAxisValue(0, 2, 0, 0);
            return;
        }

        ToWayPointMove();        
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
        float throttleOrder = targetSpeed - velocity.magnitude;

        if (localOrder_PD.y > -0.2f)
        {            
            if(toTargetVecZRange < -100)
            {
                pitchOrder = 1;
            }
            else
            {
                pitchOrder = localOrder_PD.y;
            }
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
            if(yawOrder > 0)
            {
                rollOrder = 1;
            }
            else
            {
                rollOrder = -1;
            }
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
        throttleOrder = Mathf.Clamp(throttleOrder, -1, 1);

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
