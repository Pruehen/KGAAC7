using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ���� ���� - ���� ���� ���¸� �Ǵ� �� ��������Ʈ�� �������� �������� ���� �Ǵ��Ѵ�.
//1. ��������Ʈ ����Ʈ�� �����Ͽ� �ش� ��������Ʈ���� ������� �湮
//2. Ư�� Ʈ�������� ��� ��ġ�� ��� ����
//3. Ư�� Ʈ�������� ���� ���� ����(���� ����)

public interface IFlightStratage
{
    public Vector3 ReturnNewOrder();//���ο� ��������Ʈ ��ǥ�� ��ȯ
    public float ReturnNewSpeed();//���ο� Ÿ�� �ӵ��� ��ȯ
}

public class CustomAI : MonoBehaviour
{
    FlightController_AI flightController_AI;
    [Header("���� ��� ����")]
    public List<Vector3> wayPointList;//�̸� ������ ���� ���
    public bool wayPointLoop;//������ ��ο� �������� �� ó�� ��η� ���ư�����
    public float targetSpeed;//��ǥ �̵� �ӵ�

    [Header("��� ����")]
    public Transform flightLeader;//�������� Ʈ������. null�� ��� ������̰ų� �ܵ� ��ü
    public float spreadValue = 1;
    public Vector3 formationLocalPos;//����� ���� �ڽ��� ���� ��ǥ

    public Transform target;
    public System.Action engage;
    bool isEngage;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    List<IFlightStratage> flightStratagesList = new List<IFlightStratage>();
    IFlightStratage currentflightStratage;
    Vector3 wayPointTemp = Vector3.zero;
    float targetSpeedTemp = 0;

    private void OnDrawGizmos()
    {
        if (wayPointList != null)
        {
            Gizmos.color = Color.white;
            Vector3 posTemp = this.transform.position;
            foreach (Vector3 pos in wayPointList)
            {
                Gizmos.DrawLine(posTemp, pos);
                posTemp = pos;
            }
            if (wayPointLoop)
            {
                Gizmos.DrawLine(posTemp, wayPointList[0]);
            }
        }

        if (flightLeader != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, flightLeader.position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(this.transform.position, wayPointTemp);
    }

    void Awake()
    {
        if (flightLeader != null)
        {
            formationLocalPos = flightLeader.InverseTransformPoint(this.transform.position) * spreadValue + new Vector3(0, 0, 1000);
        }

        flightController_AI = GetComponent<FlightController_AI>();

        flightStratagesList.Add(new MoveToWaypoints(this));
        flightStratagesList.Add(new Formation(this));
        flightStratagesList.Add(new CircleFlight(this));

        isEngage = false;

        if (flightLeader == null)//������̰ų� �ܵ� ��ü�� ���
        {
            currentflightStratage = flightStratagesList[0];
            engage += Engage;
        }
        else//������ ���
        {
            currentflightStratage = flightStratagesList[1];
            flightLeader.GetComponent<CustomAI>().engage += this.Engage;
        }
    }

    void Engage()//�ڽ��� ���¸� ���� ���·� ����
    {
        if (!isEngage)
        {
            Debug.Log("���� ���� ����");
        }                    
    }

    /// <summary>
    /// �ڱ� ��ü�� �ǰݴ����� �� ȣ��Ǵ� �޼���. ���� ���¸� ���� ���·� �����ϴ� ����.
    /// </summary>
    public void TakeDamage()
    {
        if (flightLeader == null)//������̰ų� �ܵ� ��ü�� ���
        {
            engage?.Invoke();//����� �������� Engage �޼��带 �����Ŵ.
        }
        else
        {
            flightLeader.GetComponent<CustomAI>().engage?.Invoke();//����忡�� ���� ��� ��û
        }    
    }

    private void Update()
    {
        Vector3 newWayPoint = currentflightStratage.ReturnNewOrder();
        float targetSpeed = currentflightStratage.ReturnNewSpeed();

        if(newWayPoint != wayPointTemp)//���� ����� ��ο� �ٸ� ��� ai�� ��������Ʈ�� ������
        {
            wayPointTemp = newWayPoint;
            flightController_AI.CreateNewWayPoint_Position(newWayPoint);            
        }
        if(targetSpeed != targetSpeedTemp)//�ӵ� ����
        {
            targetSpeedTemp = targetSpeed;
            flightController_AI.SetTargetSpeed(targetSpeed);
        }
    }

    public void ChangeStratage(int index)
    {
        currentflightStratage = flightStratagesList[index];
    }
}

class MoveToWaypoints : IFlightStratage//��� ���� ����
{
    CustomAI customAI;
    List<Vector3> wayPointList;
    Transform myTrf;
    int naxtVisitIndex;
    bool isLoop;
    float targetSpeed;
    public MoveToWaypoints(CustomAI customAI)
    {
        this.customAI = customAI;
        this.wayPointList = customAI.wayPointList;
        this.myTrf = customAI.transform;
        this.isLoop = customAI.wayPointLoop;        
        this.targetSpeed = customAI.targetSpeed;

        naxtVisitIndex = 0;
    }

    public Vector3 ReturnNewOrder()
    {
        if (Vector3.Distance(myTrf.position, wayPointList[naxtVisitIndex]) < 100)//��ǥ ���� ��
        {
            naxtVisitIndex++;
            if (isLoop && wayPointList.Count <= naxtVisitIndex)
            {
                naxtVisitIndex = 0;
            }
        }

        if (wayPointList == null || wayPointList.Count <= naxtVisitIndex)//��������Ʈ ����Ʈ�� ���ų�, ������ �湮�� ��尡 ���� ���
        {
            Vector3 outRangePos = myTrf.position + myTrf.forward * 100000;
            outRangePos.y = Mathf.Clamp(outRangePos.y, 2000, 10000);
            return outRangePos;
        }        
        else
        {
            return wayPointList[naxtVisitIndex];
        }
        
    }

    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}

class Formation : IFlightStratage//��� ���� ����
{
    CustomAI customAI;
    Transform flTrf;
    Transform myTrf;
    Vector3 formationLocalPos;
    Vector3 targetWorldPos;

    float distanceTemp;
    float p = 40;
    float d = 3000;

    public Formation(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;
        this.flTrf = customAI.flightLeader;
        this.formationLocalPos = customAI.formationLocalPos;
    }

    public Vector3 ReturnNewOrder()
    {
        if(flTrf == null)
        {
            customAI.ChangeStratage(2);
            return Vector3.zero;
        }

        targetWorldPos = flTrf.position + (flTrf.rotation * (formationLocalPos));
        return targetWorldPos;
    }
    public float ReturnNewSpeed()
    {
        float distance = (targetWorldPos - myTrf.position).magnitude - 1000;
        float distanceDiff = distance - distanceTemp;
        distanceTemp = distance;
        return distance * p + distanceDiff * d;
    }
}

class CircleFlight : IFlightStratage //��ȸ ���� ����
{
    CustomAI customAI;
    Transform myTrf;
    float targetSpeed;
    public CircleFlight(CustomAI customAI)
    {
        this.customAI = customAI;
        myTrf = customAI.transform;
        targetSpeed = customAI.targetSpeed;
    }

    public Vector3 ReturnNewOrder()
    {
        Vector3 targetPos = myTrf.position + myTrf.forward * 3000;
        targetPos.y = myTrf.position.y;
        targetPos = Quaternion.Euler(0, 30, 0) * targetPos;
        return targetPos;
    }
    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}