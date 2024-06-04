using UnityEngine;

//어떤 항공기를 사용하는지 알려주는 클래스
public class AircraftSelecter : MonoBehaviour
{
    enum AircraftName
    {
        F14A,
        F15C,
        F16C,
        MiG29A,
        SU37
    }


    [SerializeField] AircraftName controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }
    public AircraftControl aircraftControl { get; private set; }

    public string name { get; private set; }

    private void Awake()
    {
        SetControlAircraft(controlAircraft);
    }

    void SetControlAircraft(AircraftName aircraftName)
    {
        GameObject aircraftInChild;

        switch (aircraftName)
        {
            case AircraftName.F14A:
                aircraftInChild = this.transform.Find("F-14A").gameObject;
                break;
            case AircraftName.F15C:
                aircraftInChild = this.transform.Find("F-15C").gameObject;
                break;
            case AircraftName.F16C:
                aircraftInChild = this.transform.Find("F-16C").gameObject;
                break;
            case AircraftName.MiG29A:
                aircraftInChild = this.transform.Find("MiG-29A").gameObject;
                break;
            case AircraftName.SU37:
                aircraftInChild = this.transform.Find("Su-37").gameObject;
                break;
            default:
                aircraftInChild = this.transform.Find("F-16C").gameObject;
                break;
        }
        SetControlAircraft(aircraftInChild);
    }

    /// <summary>
    /// aircraft가 사용할 항공기를 정해주는 메서드
    /// </summary>
    /// <param name="controlAircraft"></param>
    void SetControlAircraft(GameObject controlAircraft)
    {        
        controlAircraft.SetActive(true);
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        this.gameObject.GetComponent<AircraftFM>().Init(controlAircraft);
        weaponSystem = controlAircraft.GetComponent<kjh.WeaponSystem>();
        weaponSystem?.Init();
        aircraftControl = controlAircraft.GetComponent<AircraftControl>();
    }
}
