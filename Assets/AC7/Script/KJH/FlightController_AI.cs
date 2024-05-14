using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiState
{
    cruise,
    random,
    tracking
}

//���� �����ϴ� �װ��� �����鿡 ���� �����͸� �����ϴ� Ŭ����
public class FlightController_AI : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] AircraftControl aircraftControl;

    Rigidbody rigidbody;
    [SerializeField] Vector3 waypoint;

    AiState state;
    [SerializeField] Transform trakingTarget = null;
    [SerializeField][Range(5f, 10f)] float aiLevel;//���̵� ����. 5���� 10������ ���� ����. ���� Ŭ���� �⵿�� �� ������ ��
    /// <summary>
    /// �� AI�� ������ Ÿ��(Ʈ������)�� �����ϴ� �޼���
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

        if(trakingTarget != null && state != AiState.tracking)//������ Ʈ�������� ������ ���
        {
            state = AiState.tracking;
        }

        if (state != AiState.tracking && WayPointOnCheck())//������ Ʈ�������� ������, ��������Ʈ�� �������� ���
        {
            CreateNewWayPoint_Normal();
        }

        ToWayPointMove();
        //aircraftControl.SetAxisValue(PlayerInputCustom.Instance.pitchAxis, PlayerInputCustom.Instance.rollAxis, PlayerInputCustom.Instance.yawAxis, PlayerInputCustom.Instance.throttleAxis);//�׽�Ʈ �ڵ�
    }

    bool WayPointOnCheck()//��������Ʈ���� �Ÿ��� 100 �̸��� ��� true ��ȯ
    {
        return (Vector3.Distance(waypoint, this.transform.position) < 100);        
    }

    void CreateNewWayPoint_Traking()
    {
        if (state == AiState.tracking)//���� ���� ���
        {
            waypoint = trakingTarget.position;
        }
    }

    void CreateNewWayPoint_Normal()
    {
        if (state == AiState.cruise)//���� ���� ���
        {
            waypoint = new Vector3(this.transform.forward.x, 0, this.transform.forward.z).normalized * 5000 + this.transform.position;
        }
        else//���� ���� ���
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
