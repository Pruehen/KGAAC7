using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌시 사망하는 클래스
/// </summary>
public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        IFightable target;
        if(col.transform.TryGetComponent<IFightable>(out target))
        {
            target.TakeDamage(9999999f);
        }
    }
}
