using Mirror;
using System.Collections;
using UnityEngine;

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

    Rigidbody rigidbody;

    /// <summary>
    /// ���� �װ����� �ӵ�(km/h)�� ��ȯ�ϴ� �޼��� 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude * 3.6f;
    }
    
    //public AircraftControl aircraftControl;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        aircraftSelecter = GetComponent<AircraftSelecter>();
        aircraftControl = aircraftSelecter.aircraftControl;
        vehicleCombat = GetComponent<VehicleCombat>();

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

        if(_isPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
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

        kjh.GameManager.Instance.AddActiveTarget(vehicleCombat);
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
