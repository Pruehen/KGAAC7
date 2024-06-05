using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{            
    public static PlayerSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }

    public List<Transform> Positions_SpawnPositions;
    public AircraftName UseAircraftNameEnum { get; private set; }
    public string UserNickName { get; set; }
    public string SelectAircraftName = "F-16C";

    public void SetAircraft_F14()
    {
        UseAircraftNameEnum = AircraftName.F14;
        SelectAircraftName = "F-14A";
    }
    public void SetAircraft_F15()
    {
        UseAircraftNameEnum = AircraftName.F15;
        SelectAircraftName = "F-15C";
    }
    public void SetAircraft_F16()
    {
        UseAircraftNameEnum = AircraftName.F16;
        SelectAircraftName = "F-16C";
    }
    public void SetAircraft_M29()
    {
        UseAircraftNameEnum = AircraftName.M29;
        SelectAircraftName = "MiG-29A";
    }
    public void SetAircraft_S37()
    {
        UseAircraftNameEnum = AircraftName.S37;
        SelectAircraftName = "Su-37";
    }
}
