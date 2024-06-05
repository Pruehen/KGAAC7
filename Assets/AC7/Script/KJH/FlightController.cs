using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 조종하는 항공기 조종면에 조종 데이터를 전달하는 클래스
public class FlightController : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] public AircraftControl aircraftControl;
    [SerializeField] private PlayerInputCustom PlayerInput;

    // Start is called before the first frame update
    void Start()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
    }

    // Update is called once per frame
    void Update()
    {
        aircraftControl = aircraftSelecter.aircraftControl;
        if (!dead)
        {            
            aircraftControl.SetAxisValue(PlayerInput.pitchAxis, PlayerInput.rollAxis, PlayerInput.yawAxis, PlayerInput.throttleAxis);//테스트 코드
        }
        else
        {
            aircraftControl.SetAxisValue(0, 2, 0, -2);//테스트 코드
        }
    }

    bool dead = false;
    public void Dead()
    {
        dead = true;
    }
    public void ResetDead()
    {
        dead = false;
    }
}