using UnityEngine;

public enum AircraftName
{
    F14,
    F15,
    F16,
    M29,
    S37
}
//어떤 항공기를 사용하는지 알려주는 클래스
public class AircraftSelecter : MonoBehaviour
{
    [SerializeField] AircraftName controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }
    public AircraftControl aircraftControl { get; private set; }

    public string name { get; private set; }

    private void Awake()
    {
        //SetControlAircraft(PlayerSpawner.Instance.UseAircraftNameEnum);
    }

    public void SetControlAircraft(AircraftName aircraftName)
    {
        GameObject aircraftInChild;

        switch (aircraftName)
        {
            case AircraftName.F14:
                aircraftInChild = this.transform.Find("F-14A").gameObject;
                break;
            case AircraftName.F15:
                aircraftInChild = this.transform.Find("F-15C").gameObject;
                break;
            case AircraftName.F16:
                aircraftInChild = this.transform.Find("F-16C").gameObject;
                break;
            case AircraftName.M29:
                aircraftInChild = this.transform.Find("MiG-29A").gameObject;
                break;
            case AircraftName.S37:
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
