using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kjh;

//어떤 항공기를 사용하는지 알려주는 클래스
public class AircraftSelecter : MonoBehaviour
{
    public GameObject controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }
    public AircraftControl aircraftControl { get; private set; }

    private void Awake()
    {
        SetControlAircraft(controlAircraft);
    }

    /// <summary>
    /// aircraft가 사용할 항공기를 정해주는 메서드
    /// </summary>
    /// <param name="controlAircraft"></param>
    public void SetControlAircraft(GameObject controlAircraft)
    {
        if (this.aircraftControl != null)
        {
            this.controlAircraft.SetActive(false);
        }

        if(controlAircraft == null)
        {
            string name = null;
            if (GameObject.Find("_F-16C") != null)
            {
                name = "F-16C";
            }
            else if(GameObject.Find("_MiG-29A") != null)
            {
                name = "MiG-29A";
            }
            else if (GameObject.Find("_F-14A") != null)
            {
                name = "F-14A";
            }
            else if (GameObject.Find("_F-15C") != null)
            {
                name = "F-15C";
            }
            controlAircraft = transform.Find(name).gameObject;
            if (controlAircraft == null)
                return;
        }

        this.controlAircraft = controlAircraft;
        this.controlAircraft.SetActive(true);
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        this.gameObject.GetComponent<AircraftFM>().Init();
        weaponSystem = controlAircraft.GetComponent<kjh.WeaponSystem>();
        weaponSystem.Init();
        aircraftControl = controlAircraft.GetComponent<AircraftControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
