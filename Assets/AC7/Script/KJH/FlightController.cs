using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �����ϴ� �װ��� �����鿡 ���� �����͸� �����ϴ� Ŭ����
public class FlightController : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] public AircraftControl aircraftControl;
    [SerializeField] PlayerInputCustom PlayerInputCustom_input;

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
            aircraftControl.SetAxisValue(PlayerInputCustom_input.pitchAxis, PlayerInputCustom_input.rollAxis,
                PlayerInputCustom_input.yawAxis, PlayerInputCustom_input.throttleAxis);//�׽�Ʈ �ڵ�
        }
        else
        {
            aircraftControl.SetAxisValue(0, 2, 0, -2);//�׽�Ʈ �ڵ�
        }
    }

    bool dead = false;
    public void Dead()
    {
        dead = true;
    }
}