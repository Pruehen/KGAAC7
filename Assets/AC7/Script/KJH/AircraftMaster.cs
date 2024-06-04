using Mirror;
using System.Collections;
using System.Collections.Generic;
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

    private float _syncedSpeed = 0f;
    private float SyncedSpeed
    {
        get
        {
            CommandGetSpeed();
            return _syncedSpeed;
        }
    }

    [Command (requiresAuthority = false)]
    private void CommandGetSpeed()
    {
        RpcSetSpeed(rigidbody.velocity.magnitude * 3.6f);
    }
    [ClientRpc]
    private void RpcSetSpeed(float speed)
    {
        _syncedSpeed = speed;
    }

    Rigidbody rigidbody;

    /// <summary>
    /// ���� �װ����� �ӵ�(km/h)�� ��ȯ�ϴ� �޼��� 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        if(isClientOnly)
        {
            return SyncedSpeed;
        }
        else
        {
            return rigidbody.velocity.magnitude * 3.6f;
        }
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
        if (isLocalPlayer)
        {
            Camera.main.transform.GetComponent<CamRotate>().enabled = false;
            if (isLocalPlayer)
            {
                Cursor.lockState = CursorLockMode.None;
            }

        }
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSeconds(2.5f);
        if(isLocalPlayer)
        {
            kjh.GameManager.Instance.GameEnd(false, .3f);
        }
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);
        
        if (!_isPlayer)
            Destroy(this.gameObject);
        else
            gameObject.SetActive(false);
    }
}
