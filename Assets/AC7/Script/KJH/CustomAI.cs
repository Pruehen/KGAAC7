using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//비행 전략 공통 - 현재 비행 상태를 판단 후 웨이포인트를 갱신할지 전략별로 각각 판단한다.
//1. 웨이포인트 리스트를 지정하여 해당 웨이포인트들을 순서대로 방문
//2. 특정 트랜스폼의 상대 위치로 편대 비행
//3. 특정 트랜스폼을 향해 직접 비행(자유 교전)

public interface IFlightStratage
{
    public void EnterState();
    public void ExitState();
    public Vector3 ReturnNewOrder();//새로운 웨이포인트 좌표를 반환
    public float ReturnNewSpeed();//새로운 타겟 속도를 반환
}

public enum EngageRule
{
    none,
    passive,
    active
}
public enum StratageType
{
    moveToWaypoints,
    formation,
    circleFlight,
    breakStrtg,
    traking_Pure,
    traking_Orbit,
    missileEvade
}


public class CustomAI : MonoBehaviour
{
    FlightController_AI flightController_AI;
    public FlightController_AI FlightController_AI() { return flightController_AI; }
    WeaponController_AI weaponController_AI;
    public WeaponController_AI WeaponController_AI() { return weaponController_AI; }
    MWR mwr;
    public float EvadeChance() 
    {         
        if(mwr.missileCount > 0)
        {
            //Debug.Log(missileEvadePercantage);
            return missileEvadePercantage;
        }
        else
        {            
            return 0;
        }
    }
    float missileEvadePercantage;

    [Header("비행 경로 지정")]
    public List<Vector3> wayPointList;//미리 지정된 비행 경로
    public bool wayPointLoop;//마지막 경로에 도착했을 때 처음 경로로 돌아갈건지
    public float targetSpeed;//목표 이동 속도

    [Header("편대 비행")]
    public Transform flightLeader;//편대장기의 트랜스폼. null일 경우 편대장이거나 단독 개체
    public float spreadValue = 1;
    public Vector3 formationLocalPos;//편대장 기준 자신의 로컬 좌표

    [Header("교전 규칙")]
    public EngageRule engageRule;
    public bool isWingmanPosition;
    public float positionSwichTime = 30;

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
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wayPointTemp, 30);
    }

    /// <summary>
    /// 자신의 편대 위치를 설정하는 메서드
    /// </summary>
    /// <param name="spreadValue"></param>
    public void SetFormationPos(float spreadValue)
    {
        if (flightLeader != null)
        {
            formationLocalPos = flightLeader.InverseTransformPoint(this.transform.position) * spreadValue + new Vector3(0, 0, 2000);
        }
    }

    void Start()
    {
        SetFormationPos(spreadValue);

        flightController_AI = GetComponent<FlightController_AI>();
        weaponController_AI = GetComponent<WeaponController_AI>();
        mwr = GetComponent<MWR>();
        missileEvadePercantage = (flightController_AI.AiLevel() * 0.1f) * (flightController_AI.AiLevel() * 0.1f);

        flightStratagesList.Add(new MoveToWaypoints(this));
        flightStratagesList.Add(new Formation(this));
        flightStratagesList.Add(new CircleFlight(this));
        flightStratagesList.Add(new Break(this));
        flightStratagesList.Add(new Traking_Pure(this));
        flightStratagesList.Add(new Traking_Orbit(this));
        flightStratagesList.Add(new MissileEvade(this));

        isEngage = false;

        if (flightLeader == null)//편대장이거나 단독 개체일 경우
        {
            ChangeStratage(StratageType.moveToWaypoints);
            engage += Engage;
        }
        else//편대원일 경우
        {
            ChangeStratage(StratageType.formation);
            flightLeader.GetComponent<CustomAI>().engage += this.Engage;
        }

        StartCoroutine(Order());
        StartCoroutine(ChangeIsWingmanPosition());

        if(engageRule == EngageRule.active)
        {
            StartCoroutine(StartEngage());
        }
    }

    void Engage()//자신의 상태를 교전 상태로 수정
    {
        if (!isEngage && engageRule != EngageRule.none)
        {
            isEngage = true;
            weaponController_AI.StartWeaponFireCheck();
            ChangeStratage(3);
        }                    
    }

    /// <summary>
    /// 교전 실행 시 호출되는 메서드. 편대원 상태를 교전 상태로 변경하는 역할.
    /// </summary>
    public void EngageOrder()
    {
        if (flightLeader == null)//편대장이거나 단독 개체일 경우
        {
            engage?.Invoke();//연결된 편대원들의 Engage 메서드를 실행시킴.
        }
        else
        {
            flightLeader.GetComponent<CustomAI>().engage?.Invoke();//편대장에게 교전 명령 신청
        }    
    }
    IEnumerator StartEngage()
    {
        yield return new WaitForSeconds(1);
        EngageOrder();
    }

    IEnumerator Order()
    {        
        while (true)
        {            
            Vector3 newWayPoint = currentflightStratage.ReturnNewOrder();
            float targetSpeed = currentflightStratage.ReturnNewSpeed();

            if (newWayPoint != wayPointTemp)//기존 저장된 경로와 다를 경우 ai의 웨이포인트를 갱신함
            {
                wayPointTemp = newWayPoint;
                flightController_AI.CreateNewWayPoint_Position(newWayPoint);
            }
            if (targetSpeed != targetSpeedTemp)//속도 갱신
            {
                targetSpeedTemp = targetSpeed;
                flightController_AI.SetTargetSpeed(targetSpeed);
            }
            yield return new WaitForSeconds(1);
        }    
    }
    IEnumerator ChangeIsWingmanPosition()
    {
        while(true)
        {
            yield return new WaitForSeconds(positionSwichTime);
            isWingmanPosition = !isWingmanPosition;
        }
    }

    /// <summary>
    /// 자신의 전략을 변경하는 메서드
    /// </summary>
    /// <param name="index"></param>
    void ChangeStratage(int index)
    {
        if (currentflightStratage != null)
        {
            currentflightStratage.ExitState();
        }
        currentflightStratage = flightStratagesList[index];
        currentflightStratage.EnterState();
    }
    public void ChangeStratage(StratageType type)
    {
        ChangeStratage((int)type);
    }

    /// <summary>
    /// 자신의 전략을 time초 후에 변경하는 메서드
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator ChangeStratage(StratageType type, float time)
    {
        yield return new WaitForSeconds(time);
        ChangeStratage((int)type);
    }
}

class MoveToWaypoints : IFlightStratage//0. 경로 비행 전략
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
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} 상태 설정 : 경로 비행"); }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {
        if (wayPointList == null || wayPointList.Count == 0)
        {
            customAI.ChangeStratage(StratageType.circleFlight);
            return myTrf.forward * 5000;
        }

        if (Vector3.Distance(myTrf.position, wayPointList[naxtVisitIndex]) < 300)//목표 도착 시
        {
            naxtVisitIndex++;
            if (isLoop && wayPointList.Count <= naxtVisitIndex)
            {
                naxtVisitIndex = 0;
            }
        }

        if (wayPointList == null || wayPointList.Count <= naxtVisitIndex)//웨이포인트 리스트가 없거나, 다음에 방문할 노드가 없을 경우
        {
            customAI.ChangeStratage(StratageType.circleFlight);
            return myTrf.forward * 5000;
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

class Formation : IFlightStratage//1. 편대 비행 전략
{
    CustomAI customAI;
    Transform flTrf;
    Transform myTrf;
    Vector3 formationLocalPos;
    Vector3 targetWorldPos;

    float distanceTemp;
    float p = 120;
    float d = 3000;

    public Formation(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;
        this.flTrf = customAI.flightLeader;
        this.formationLocalPos = customAI.formationLocalPos;
    }
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} 상태 설정 : 편대 비행"); }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {
        if(flTrf == null)
        {
            customAI.ChangeStratage(StratageType.circleFlight);
            return Vector3.zero;
        }

        targetWorldPos = flTrf.position + (flTrf.rotation * (formationLocalPos));
        return targetWorldPos;
    }
    public float ReturnNewSpeed()
    {
        float distance = (targetWorldPos - myTrf.position).magnitude - 2000;
        float distanceDiff = distance - distanceTemp;
        distanceTemp = distance;
        return distance * p + distanceDiff * d;
    }
}

class CircleFlight : IFlightStratage //2. 선회 비행 전략
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
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} 상태 설정 : 선회 비행"); }
    public void ExitState() { }
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

class Break : IFlightStratage //3. 편대 해체 전략
{
    CustomAI customAI;
    Transform flTrf;
    Transform myTrf;
    Vector3 formationLocalPos;
    Vector3 targetWorldPos;
    float targetSpeed;
    public Break(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;
        this.flTrf = customAI.flightLeader;
        customAI.SetFormationPos(100);
        this.formationLocalPos = customAI.formationLocalPos;
        targetSpeed = customAI.targetSpeed;
    }
    public void EnterState() 
    {
        customAI.StartCoroutine(customAI.ChangeStratage(StratageType.traking_Pure, 3));
        Debug.Log($"{customAI.gameObject.name} 상태 설정 : 편대 해체, 급기동");
    }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {
        if (flTrf == null)
        {
            Vector3 targetPos = myTrf.position + myTrf.forward * 3000;
            targetPos.y = myTrf.position.y * 10000;
            return targetPos;
        }
        else
        {
            targetWorldPos = flTrf.position + (flTrf.rotation * (formationLocalPos));
            return targetWorldPos;
        }
    }
    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}

class Traking_Pure : IFlightStratage //4. 퓨어 추적 전략
{
    CustomAI customAI;
    Transform targetTrf;
    Transform myTrf;
    float targetSpeed;    
    public Traking_Pure(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;
        customAI.SetTarget(kjh.GameManager.Instance.player.transform);
        this.targetTrf = customAI.target;                
        targetSpeed = customAI.targetSpeed + 200;        
    }
    public void EnterState()
    {        
        Debug.Log($"{customAI.gameObject.name} 상태 설정 : 퓨어 추적");
    }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {                
        if(customAI.isWingmanPosition)
        {
            customAI.ChangeStratage(StratageType.traking_Orbit);
        }
        float random = Random.Range(0f, 1f);
        if (customAI.EvadeChance() > random)
        {
            customAI.ChangeStratage(StratageType.missileEvade);
        }

        return targetTrf.position;
    }
    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}

class Traking_Orbit : IFlightStratage //5. 타겟 기준 선회 전략
{
    CustomAI customAI;
    Transform targetTrf;
    Transform myTrf;
    float targetSpeed;    
    Vector3 dir;
    public Traking_Orbit(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;
        customAI.SetTarget(kjh.GameManager.Instance.player.transform);
        this.targetTrf = customAI.target;
        targetSpeed = customAI.targetSpeed + 200;        
        dir = Vector3.forward * 7000;
    }
    public void EnterState()
    {
        Debug.Log($"{customAI.gameObject.name} 상태 설정 : 타겟 선회");
    }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {
        if (!customAI.isWingmanPosition)
        {
            customAI.ChangeStratage(StratageType.traking_Pure);
        }
        float random = Random.Range(0f, 1f);
        if(customAI.EvadeChance() > random)
        {
            customAI.ChangeStratage(StratageType.missileEvade);
        }
        dir = Quaternion.AngleAxis(5, Vector3.up) * dir;
        Vector3 targetPos = targetTrf.position + dir;        
        return targetPos;
    }
    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}

class MissileEvade : IFlightStratage //6. 미사일 회피 전략
{
    CustomAI customAI;
    Transform myTrf;
    float targetSpeed;
    public MissileEvade(CustomAI customAI)
    {
        this.customAI = customAI;
        this.myTrf = customAI.transform;        
        targetSpeed = customAI.targetSpeed + 1000;        
    }
    public void EnterState()
    {
        Debug.Log($"{customAI.gameObject.name} 상태 설정 : 회피 기동");
        customAI.WeaponController_AI().SetFlareTrigger(true);
    }
    public void ExitState() { customAI.WeaponController_AI().SetFlareTrigger(false); }
    public Vector3 ReturnNewOrder()
    {
        if (customAI.EvadeChance() == 0)
        {
            customAI.ChangeStratage(StratageType.traking_Pure);
        }

        Vector3 targetPos = myTrf.position + myTrf.forward * 1000;
        targetPos.y = myTrf.position.y;
        targetPos = myTrf.rotation * Quaternion.Euler(60, 60, 0) * targetPos;
        return targetPos;
    }
    public float ReturnNewSpeed()
    {
        return targetSpeed;
    }
}