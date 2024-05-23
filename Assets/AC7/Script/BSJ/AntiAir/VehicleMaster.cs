using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ Ŭ����. ���� ������Ʈ�� ������ �� ���. �����ϸ� ���⼭ ���� ������Ʈ�� �������� �� ��
//���� ����� �켱 ���⿡ �ٿ�����.
public class VehicleMaster : MonoBehaviour
{
    VehicleCombat vehicleCombat;
    Rigidbody rigidbody;

    public bool IsDead { get { return vehicleCombat.IsDead(); } }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        vehicleCombat = GetComponent<VehicleCombat>();
    }

    public void Dead()
    {
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSeconds(2.5f);
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);
        Destroy(this.gameObject);
    }
}
