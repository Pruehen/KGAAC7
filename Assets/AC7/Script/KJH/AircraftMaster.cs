using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ Ŭ����. ���� ������Ʈ�� ������ �� ���. �����ϸ� ���⼭ ���� ������Ʈ�� �������� �� ��
//���� ����� �켱 ���⿡ �ٿ�����.
public class AircraftMaster : MonoBehaviour
{
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
    public AircraftSelecter AircraftSelecter() { return aircraftSelecter; }
    public AircraftControl aircraftControl;
    public VehicleCombat vehicleCombat;

    Rigidbody rigidbody;

    string name;

    /// <summary>
    /// ���� �װ����� �ӵ�(km/h)�� ��ȯ�ϴ� �޼��� 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude * 3.6f;
    }
    public string Name()
    {
        return name;
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
        }

        name = aircraftControl.gameObject.name;
    }

    public void Dead()
    {
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSeconds(5);
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);
        Destroy(this.gameObject);
    }
}
