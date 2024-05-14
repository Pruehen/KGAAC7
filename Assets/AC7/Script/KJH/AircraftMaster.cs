using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
public class AircraftMaster : NetworkBehaviour
{
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
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

    
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        aircraftSelecter = GetComponent<AircraftSelecter>();
        if (aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
            bsj.GameManager.Instance.TriggerPlayerSpawn(transform);
            Camera.main.transform.parent.SetParent(transform);
        }

    }
}
