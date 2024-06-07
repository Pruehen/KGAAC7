using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//������ Ŭ����. ���� ������Ʈ�� ������ �� ���. �����ϸ� ���⼭ ���� ������Ʈ�� �������� �� ��
//���� ����� �켱 ���⿡ �ٿ�����.
public class AircraftMaster : NetworkBehaviour
{
    [SerializeField] bool _isPlayer = false;
    public bool IsPlayer() { return _isPlayer; }
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
    public AircraftSelecter AircraftSelecter() { return aircraftSelecter; }
    public AircraftControl aircraftControl;
    public VehicleCombat vehicleCombat;
    Radar radar;

    Rigidbody rigidbody;

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
    /// ���� �װ����� �ӵ�(km/h)�� ��ȯ�ϴ� �޼��� 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return (rigidbody != null) ? rigidbody.velocity.magnitude * 3.6f : 0;
    }
    
    //public AircraftControl aircraftControl;

    private void Start()
    {
        //Init(PlayerSpawner.Instance.UseAircraftNameEnum, PlayerSpawner.Instance.UserNickName);
        if (this.isLocalPlayer)
        {
            //StartCoroutine(CommandInitCoroutine());
            CommandInit(PlayerSpawner.Instance.UseAircraftNameEnum, PlayerSpawner.Instance.UserNickName);
        }
        else if(!this.isLocalPlayer && !this.isServer)
        {
            CommandSyncInit();
        }
    }

    public void Init(AircraftName aircraftName, string userName)
    {
        _aircraftName = aircraftName;
        _userName = userName;

        rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();
        aircraftSelecter.SetControlAircraft(_aircraftName);
        aircraftControl = aircraftSelecter.aircraftControl;
        vehicleCombat = GetComponent<VehicleCombat>();
        vehicleCombat.SetNames(_userName);
        radar = GetComponent<Radar>();
        radar.Init();

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
        if (this.isLocalPlayer)
        {
            SetPositionPlayer();
            //vehicleCombat.Init();
        }
        else
        {
            kjh.GameManager.Instance.AddActiveTarget(vehicleCombat);
        }        
        StartCoroutine(OnAircraftMasterInitInvoke(0.2f));
    }
    [Command(requiresAuthority = false)]
    void CommandInit(AircraftName aircraftName, string userName)
    {
        Init(aircraftName, userName);
        RpcInit(aircraftName, userName);
    }
    [Command(requiresAuthority = false)]
    void CommandSyncInit()
    {
        //Init(PlayerSpawner.Instance.UseAircraftNameEnum, PlayerSpawner.Instance.UserNickName);
        RpcInit(_aircraftName, _userName);
    }

    [ClientRpc]
    void RpcInit(AircraftName aircraftName, string userName)
    {
        if (!this.isServer && !_isInit)
        {
            Init(aircraftName, userName);
        }
    }


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
        if (this.isLocalPlayer)
        {
            SetPositionPlayer();            
        }
        else
        {
            kjh.GameManager.Instance.AddActiveTarget(vehicleCombat);
        }

        vehicleCombat.Init();
    }

    void SetPositionPlayer()
    {
        int randomNum = Random.Range(0, PlayerSpawner.Instance.Positions_SpawnPositions.Count);
        this.transform.position = PlayerSpawner.Instance.Positions_SpawnPositions[randomNum].position;
        this.transform.rotation = PlayerSpawner.Instance.Positions_SpawnPositions[randomNum].rotation;
        rigidbody.velocity = this.transform.forward * 200;
    }
}
