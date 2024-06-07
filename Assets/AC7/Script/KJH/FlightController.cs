using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 조종하는 항공기 조종면에 조종 데이터를 전달하는 클래스
public class FlightController : NetworkBehaviour
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
        if(aircraftSelecter.aircraftControl == null)
        {
            return;
        }
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


    [Command]
    public void CommandResetDead()
    {
        if (isServer)
        {
            RpcResetDead();
        }
    }
    [ClientRpc]
    private void RpcResetDead()
    {
        ResetDead();
    }

    public void ResetDead()
    {
        dead = false;
        transform.position = Vector3.up * 2000f;
    }
}