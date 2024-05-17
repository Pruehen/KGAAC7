using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//비행 전략 공통 - 현재 비행 상태를 판단 후 웨이포인트를 갱신할지 전략별로 각각 판단한다.
//1. 웨이포인트 리스트를 지정하여 해당 웨이포인트들을 순서대로 방문
//2. 특정 트랜스폼의 상대 위치로 편대 비행
//3. 특정 트랜스폼을 향해 직접 비행(자유 교전)

public interface IFlightStratage
{
    public Vector3 ReturnNewOrder();//새로운 웨이포인트 좌표를 반환
    public float ReturnNewSpeed();//새로운 타겟 속도를 반환
}

public class CustomAI : MonoBehaviour
{
    FlightController_AI flightController_AI;
    [Header("비행 경로 지정")]
    [SerializeField] List<Vector3> wayPointList;//미리 지정된 비행 경로
    [SerializeField] bool wayPointLoop;//마지막 경로에 도착했을 때 처음 경로로 돌아갈건지
    [SerializeField] float targetSpeed;//목표 이동 속도

    [Header("편대 비행")]
    [SerializeField] Transform flightLeader;//편대장기의 트랜스폼. null일 경우 편대장이거나 단독 개체
    [SerializeField] float spreadValue = 1;
    Vector3 formationLocalPos;//편대장 기준 자신의 로컬 좌표

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

        if(flightLeader == null)//편대장이거나 단독 개체일 경우
        {
            currentflightStratage = flightStratagesList[0];
        }
        else//편대원일 경우
        {
            currentflightStratage = flightStratagesList[1];
        }
    }

    private void Update()
    {
        Vector3 newWayPoint = currentflightStratage.ReturnNewOrder();
        float targetSpeed = currentflightStratage.ReturnNewSpeed();

        if(newWayPoint != wayPointTemp)//기존 저장된 경로와 다를 경우 ai의 웨이포인트를 갱신함
        {
            wayPointTemp = newWayPoint;
            flightController_AI.CreateNewWayPoint_Position(newWayPoint);            
        }
        if(targetSpeed != targetSpeedTemp)//속도 갱신
        {
            targetSpeedTemp = targetSpeed;
            flightController_AI.SetTargetSpeed(targetSpeed);
        }
    }
}

class MoveToWaypoints : IFlightStratage//경로 비행 전략
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
        if (Vector3.Distance(myTrf.position, wayPointList[naxtVisitIndex]) < 100)//목표 도착 시
        {
            naxtVisitIndex++;
            if (isLoop && wayPointList.Count <= naxtVisitIndex)
            {
                naxtVisitIndex = 0;
            }
        }

        if (wayPointList == null || wayPointList.Count <= naxtVisitIndex)//웨이포인트 리스트가 없거나, 다음에 방문할 노드가 없을 경우
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

class Formation : IFlightStratage//편대 비행 전략
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