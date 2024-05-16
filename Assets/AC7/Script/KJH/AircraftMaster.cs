using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
//전투 기능을 우선 여기에 붙여봤음.
public class AircraftMaster : MonoBehaviour
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
    }

    public void Dead()
    {
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSeconds(5);
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);
        Destroy(this.gameObject);
    }
}
