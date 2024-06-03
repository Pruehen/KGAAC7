using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌시 사망하는 클래스
/// </summary>
public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        IFightable target;
        if (collision.transform.TryGetComponent<IFightable>(out target))
        {
            target.CommandTakeDamage(9999999f);
        }
    }
}
