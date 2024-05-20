using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �浹�� ����ϴ� Ŭ����
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
