using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ���� ���� - ���� ���� ���¸� �Ǵ� �� ��������Ʈ�� �������� �������� ���� �Ǵ��Ѵ�.
//1. ��������Ʈ ����Ʈ�� �����Ͽ� �ش� ��������Ʈ���� ������� �湮
//2. Ư�� Ʈ�������� ��� ��ġ�� ��� ����
//3. Ư�� Ʈ�������� ���� ���� ����(���� ����)

public interface IFlightStratage
{
    public void EnterState();
    public void ExitState();
    public Vector3 ReturnNewOrder();//���ο� ��������Ʈ ��ǥ�� ��ȯ
    public float ReturnNewSpeed();//���ο� Ÿ�� �ӵ��� ��ȯ
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

    [Header("���� ��� ����")]
    public List<Vector3> wayPointList;//�̸� ������ ���� ���
    public bool wayPointLoop;//������ ��ο� �������� �� ó�� ��η� ���ư�����
    public float targetSpeed;//��ǥ �̵� �ӵ�

    [Header("��� ����")]
    public Transform flightLeader;//�������� Ʈ������. null�� ��� ������̰ų� �ܵ� ��ü
    public float spreadValue = 1;
    public Vector3 formationLocalPos;//����� ���� �ڽ��� ���� ��ǥ

    [Header("���� ��Ģ")]
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
    /// �ڽ��� ��� ��ġ�� �����ϴ� �޼���
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

        if (flightLeader == null)//������̰ų� �ܵ� ��ü�� ���
        {
            ChangeStratage(StratageType.moveToWaypoints);
            engage += Engage;
        }
        else//������ ���
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

    void Engage()//�ڽ��� ���¸� ���� ���·� ����
    {
        if (!isEngage && engageRule != EngageRule.none)
        {
            isEngage = true;
            weaponController_AI.StartWeaponFireCheck();
            ChangeStratage(3);
        }                    
    }

    /// <summary>
    /// ���� ���� �� ȣ��Ǵ� �޼���. ���� ���¸� ���� ���·� �����ϴ� ����.
    /// </summary>
    public void EngageOrder()
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

            if (newWayPoint != wayPointTemp)//���� ����� ��ο� �ٸ� ��� ai�� ��������Ʈ�� ������
            {
                wayPointTemp = newWayPoint;
                flightController_AI.CreateNewWayPoint_Position(newWayPoint);
            }
            if (targetSpeed != targetSpeedTemp)//�ӵ� ����
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
    /// �ڽ��� ������ �����ϴ� �޼���
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
    /// �ڽ��� ������ time�� �Ŀ� �����ϴ� �޼���
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

class MoveToWaypoints : IFlightStratage//0. ��� ���� ����
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
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} ���� ���� : ��� ����"); }
    public void ExitState() { }
    public Vector3 ReturnNewOrder()
    {
        if (wayPointList == null || wayPointList.Count == 0)
        {
            customAI.ChangeStratage(StratageType.circleFlight);
            return myTrf.forward * 5000;
        }

        if (Vector3.Distance(myTrf.position, wayPointList[naxtVisitIndex]) < 300)//��ǥ ���� ��
        {
            naxtVisitIndex++;
            if (isLoop && wayPointList.Count <= naxtVisitIndex)
            {
                naxtVisitIndex = 0;
            }
        }

        if (wayPointList == null || wayPointList.Count <= naxtVisitIndex)//��������Ʈ ����Ʈ�� ���ų�, ������ �湮�� ��尡 ���� ���
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

class Formation : IFlightStratage//1. ��� ���� ����
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
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} ���� ���� : ��� ����"); }
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

class CircleFlight : IFlightStratage //2. ��ȸ ���� ����
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
    public void EnterState() { Debug.Log($"{customAI.gameObject.name} ���� ���� : ��ȸ ����"); }
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

class Break : IFlightStratage //3. ��� ��ü ����
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
        Debug.Log($"{customAI.gameObject.name} ���� ���� : ��� ��ü, �ޱ⵿");
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

class Traking_Pure : IFlightStratage //4. ǻ�� ���� ����
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
        Debug.Log($"{customAI.gameObject.name} ���� ���� : ǻ�� ����");
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

class Traking_Orbit : IFlightStratage //5. Ÿ�� ���� ��ȸ ����
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
        Debug.Log($"{customAI.gameObject.name} ���� ���� : Ÿ�� ��ȸ");
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

class MissileEvade : IFlightStratage //6. �̻��� ȸ�� ����
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
        Debug.Log($"{customAI.gameObject.name} ���� ���� : ȸ�� �⵿");
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