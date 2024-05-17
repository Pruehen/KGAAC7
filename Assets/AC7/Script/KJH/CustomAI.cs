using System;
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
    [SerializeField] List<Vector3> wayPointList;//�̸� ������ ���� ���
    [SerializeField] bool wayPointLoop;//������ ��ο� �������� �� ó�� ��η� ���ư�����
    [SerializeField] float targetSpeed;//��ǥ �̵� �ӵ�

    [Header("��� ����")]
    [SerializeField] Transform flightLeader;//�������� Ʈ������. null�� ��� ������̰ų� �ܵ� ��ü
    [SerializeField] float spreadValue = 1;
    Vector3 formationLocalPos;//����� ���� �ڽ��� ���� ��ǥ

    Transform target;
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

        flightStratagesList.Add(new MoveToWaypoints(wayPointList, this.transform, wayPointLoop, targetSpeed));
        flightStratagesList.Add(new Formation(this.transform, flightLeader, formationLocalPos));
        flightStratagesList.Add(new Engage());

        if(flightLeader == null)//������̰ų� �ܵ� ��ü�� ���
        {
            currentflightStratage = flightStratagesList[0];
        }
        else//������ ���
        {
            currentflightStratage = flightStratagesList[1];
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
}

class MoveToWaypoints : IFlightStratage//��� ���� ����
{
    List<Vector3> wayPointList;
    Transform myTrf;
    int naxtVisitIndex;
    bool isLoop;
    float targetSpeed;
    public MoveToWaypoints(List<Vector3> wayPointList, Transform myTrf, bool isLoop, float targetSpeed)
    {
        this.wayPointList = wayPointList;
        this.myTrf = myTrf;
        this.isLoop = isLoop;
        naxtVisitIndex = 0;
        this.targetSpeed = targetSpeed;
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
    Transform flTrf;
    Transform myTrf;
    Vector3 formationLocalPos;
    Vector3 targetWorldPos;

    float distanceTemp;
    float p = 20;
    float d = 1500;

    public Formation(Transform myTrf, Transform flTrf, Vector3 formationLocalPos)
    {
        this.myTrf = myTrf;
        this.flTrf = flTrf;
        this.formationLocalPos = formationLocalPos;
    }

    public Vector3 ReturnNewOrder()
    {
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

class Engage : IFlightStratage
{
    public Vector3 ReturnNewOrder()
    {
        throw new System.NotImplementedException();
    }
    public float ReturnNewSpeed()
    {
        throw new System.NotImplementedException();
    }
}