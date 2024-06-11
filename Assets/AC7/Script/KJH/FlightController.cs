using UnityEngine;

//���� �����ϴ� �װ��� �����鿡 ���� �����͸� �����ϴ� Ŭ����
public class FlightController : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] public AircraftControl aircraftControl;
    [SerializeField] PlayerInputCustom PlayerInputCustom_input;


    bool isInit = false;
    private void Start()
    {
        AircraftMaster aircraftMaster = this.GetComponent<AircraftMaster>();
        aircraftMaster.OnAircraftMasterInit.AddListener(Init);
    }
    // Start is called before the first frame update
    public void Init()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
        dead = false;
        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit)
            return;

        if (aircraftControl == null)
        {
            aircraftControl = aircraftSelecter.aircraftControl;
            if (aircraftControl == null) return;
        }

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