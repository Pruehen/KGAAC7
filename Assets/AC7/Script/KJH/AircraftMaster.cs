using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ Ŭ����. ���� ������Ʈ�� ������ �� ���. �����ϸ� ���⼭ ���� ������Ʈ�� �������� �� ��
//���� ����� �켱 ���⿡ �ٿ�����.
public class AircraftMaster : MonoBehaviour
{
    [SerializeField] bool aiControl;
    AircraftSelecter aircraftSelecter;
    Rigidbody rigidbody;
    /// <summary>
    /// ���� �װ����� �ӵ�(km/h)�� ��ȯ�ϴ� �޼��� 
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return rigidbody.velocity.magnitude * 3.6f;
    }

    public AircraftSelecter AircraftSelecter() { return aircraftSelecter; }
    //public AircraftControl aircraftControl;

    private void Awake()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
        rigidbody = GetComponent<Rigidbody>();

        if (aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
        }       
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
