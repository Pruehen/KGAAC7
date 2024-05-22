using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �����ϴ� �װ��� �����鿡 ���� �����͸� �����ϴ� Ŭ����
public class FlightController_AI : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    AircraftControl aircraftControl;

    Rigidbody rigidbody;
    Vector3 waypoint;

    bool isDead = false;  
    [SerializeField][Range(3f, 10f)] float aiLevel;//���̵� ����. 3���� 10������ ���� ����. ���� Ŭ���� �⵿�� �� ���ϰ� ��

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
        if(isDead)//��� �� ���� ����� ���
        {
            aircraftControl.SetAxisValue(0, 2, 0, 0);
            return;
        }

        ToWayPointMove();        
    }

    /// <summary>
    /// Ư�� �������� �̵��ϴ� ��������Ʈ�� ����
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
            if (toTargetVecZRange > 0 || toTargetVec.magnitude > 3000)//Ÿ���� �ڽ� �տ� ������ �Ÿ��� �� ���, Ʈ��ŷ�ϸ� ���� ����
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

        if((this.transform.position + velocity * (16 - aiLevel)).y < 0)//6~13�� ���� ���� ���� ������ ���
        {
            rollOrder = RollKeepLevel();
            pitchOrder = 1;
            //Debug.Log("���� ȸ��");
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
    /// ����� ȣ���ϴ� �޼���
    /// </summary>
    public void Dead()
    {
        isDead = true;
    }
}
