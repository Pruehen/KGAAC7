using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kjh;

public class AircraftSelecter : MonoBehaviour
{
    public GameObject controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }

    private void Awake()
    {
        SetControlAircraft(controlAircraft);
    }

    public void SetControlAircraft(GameObject controlAircraft)
    {
        this.controlAircraft = controlAircraft;
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        weaponSystem = controlAircraft.GetComponent<kjh.WeaponSystem>();
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
