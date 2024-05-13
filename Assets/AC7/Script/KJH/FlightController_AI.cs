using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 조종하는 항공기 조종면에 조종 데이터를 전달하는 클래스
public class FlightController_AI : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] AircraftControl aircraftControl;

    // Start is called before the first frame update
    void Start()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
    }

    // Update is called once per frame
    void Update()
    {
        aircraftControl = aircraftSelecter.aircraftControl;

        //aircraftControl.SetAxisValue(PlayerInputCustom.Instance.pitchAxis, PlayerInputCustom.Instance.rollAxis, PlayerInputCustom.Instance.yawAxis, PlayerInputCustom.Instance.throttleAxis);//테스트 코드
    }
}
