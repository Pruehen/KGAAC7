using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �����ϴ� �װ��� �����鿡 ���� �����͸� �����ϴ� Ŭ����
public class FlightController : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] public AircraftControl aircraftControl;

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
            aircraftControl.SetAxisValue(PlayerInputCustom.Instance.pitchAxis, PlayerInputCustom.Instance.rollAxis, PlayerInputCustom.Instance.yawAxis, PlayerInputCustom.Instance.throttleAxis);//�׽�Ʈ �ڵ�
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