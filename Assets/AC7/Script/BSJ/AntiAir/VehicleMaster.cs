using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
//전투 기능을 우선 여기에 붙여봤음.
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
        OnCombatDestroy.Invoke();
        Destroy(this.gameObject);
    }

    public UnityEvent OnCombatDestroy;
}
