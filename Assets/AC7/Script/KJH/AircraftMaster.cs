using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
//전투 기능을 우선 여기에 붙여봤음.
public class AircraftMaster : MonoBehaviour, IFightable
{
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
    Rigidbody rigidbody;
    /// <summary>
    /// 현재 항공기의 속도(km/h)를 반환하는 메서드 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude * 3.6f;
    }

    public AircraftSelecter AircraftSelecter() { return aircraftSelecter; }
    //public AircraftControl aircraftControl;

    Combat combat = new Combat();

    public void DealDamage(IFightable target, float damage)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
        rigidbody = GetComponent<Rigidbody>();

        if (aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
        }

        combat.Init(this.transform, 100);
    }
}
