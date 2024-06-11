using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public struct AirInfo
{
    public AircraftName AirName;
    public string UserNickName;
}

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
//전투 기능을 우선 여기에 붙여봤음.
public class AircraftMaster : MonoBehaviour
{
    [SerializeField] bool _isPlayer = false;
    public bool IsPlayer() { return _isPlayer; }
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
    public AircraftSelecter AircraftSelecter() { return aircraftSelecter; }
    public AircraftControl aircraftControl;
    public VehicleCombat vehicleCombat;
    Radar radar;

    AircraftFM aircraftFM;

    AircraftName _aircraftName;
    string _userName;
    bool _isInit = false;    

    public UnityEvent OnAircraftMasterInit;
    IEnumerator OnAircraftMasterInitInvoke(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        OnAircraftMasterInit.Invoke();
        _isInit = true;
    }

    /// <summary>
    /// 현재 항공기의 속도(km/h)를 반환하는 메서드 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return (aircraftFM != null) ? aircraftFM.Velocity : 0;
    }
    public float GetGForce()
    {
        return (aircraftFM != null) ? aircraftFM.G_Force : 0;
    }
    public float GetAoa()
    {
        return (aircraftFM != null) ? aircraftFM.AoA: 0;
    }    

    /*public override void OnStartLocalPlayer()
    {
        CommandInit(PlayerSpawner.Instance.UseAircraftNameEnum, PlayerSpawner.Instance.UserNickName);
    }*/

    private void Start()
    {
        //StartCoroutine(TryCommandSync());
        Init(PlayerSpawner.Instance.UseAircraftNameEnum, PlayerSpawner.Instance.UserNickName);
    }
    //IEnumerator TryCommandSync()
    //{
    //    yield return new WaitForSecondsRealtime(1);
    //    if (!this.isLocalPlayer)
    //    {
    //        CommandSync();
    //    }
    //}

    public void Init(AircraftName aircraftName, string userName)
    {
        _aircraftName = aircraftName;
        _userName = userName;

        //rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();
        aircraftSelecter.SetControlAircraft(_aircraftName);

        aircraftControl = aircraftSelecter.aircraftControl;
        vehicleCombat = GetComponent<VehicleCombat>();
        vehicleCombat.Init();
        vehicleCombat.SetNames(_userName);

        radar = GetComponent<Radar>();
        radar.Init();

        aircraftFM = GetComponent<AircraftFM>();


        if (aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
            GetComponent<WeaponController_AI>().enabled = false;
            GetComponent<CustomAI>().enabled = false;
        }

        if (_isPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        //if (this.isLocalPlayer)
        //{
        //    SetPositionPlayer();
        //    //vehicleCombat.Init();
        //}
        SetPositionPlayer();
        kjh.GameManager.Instance.AddActiveTarget(vehicleCombat);
        //StartCoroutine(OnAircraftMasterInitInvoke(0.2f));
        OnAircraftMasterInit.Invoke();
        _isInit = true;
    }
    //[Command(requiresAuthority = false)]
    //void CommandInit(AircraftName aircraftName, string userName)
    //{
    //    //Init(aircraftName, userName);
    //    RpcInit(aircraftName, userName);
    //}
    //[Command(requiresAuthority = false)]
    //void CommandSync()
    //{
    //    RpcSync(this._aircraftName, this._userName);
    //}

    //[ClientRpc]
    //void RpcInit(AircraftName aircraftName, string userName)
    //{
    //    Init(aircraftName, userName);
    //}
    //[ClientRpc]
    //void RpcSync(AircraftName aircraftName, string userName)
    //{
    //    if(!_isInit)
    //    {
    //        Init(aircraftName, userName);
    //    }
    //}


    public void Dead()
    {
        vehicleCombat.onDead.Invoke();
    }
    public void OnDead()
    {
        if (_isPlayer)
        {
            //Camera.main.transform.SetParent(null);
            //Camera.main.transform.GetComponent<CamRotate>().enabled = false;
            if (_isPlayer)
            {
                //Cursor.lockState = CursorLockMode.None;
            }

        }
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);

        yield return new WaitForSecondsRealtime(1);
        ReSpawnPlayer();
    }

    public void ReSpawnPlayer()
    {
        //if (this.isLocalPlayer)
        //{
        //    SetPositionPlayer();            
        //}
        SetPositionPlayer();
        kjh.GameManager.Instance.AddActiveTarget(vehicleCombat);
        vehicleCombat.Init();
    }

    void SetPositionPlayer()
    {
        int randomNum = Random.Range(0, PlayerSpawner.Instance.Positions_SpawnPositions.Count);
        this.transform.position = PlayerSpawner.Instance.Positions_SpawnPositions[randomNum].position;
        this.transform.rotation = PlayerSpawner.Instance.Positions_SpawnPositions[randomNum].rotation;
        //rigidbody.velocity = this.transform.forward * 200;
        aircraftFM.InitSpeed();
    }
}
