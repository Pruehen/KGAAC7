using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌시 사망하는 바다 클래스
/// </summary>
public class OceanObstacle : MonoBehaviour
{
    [SerializeField] GameObject _splashVfxPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        VehicleCombat target;
        if (collision.transform.TryGetComponent<VehicleCombat>(out target))
        {
            if (target.IsDead() == true)
                return;
            GameObject pooledItem = ObjectPoolManager.Instance.DequeueObject(_splashVfxPrefab);
            pooledItem.transform.position = collision.transform.position;
            ObjectPoolManager.Instance.EnqueueObject(pooledItem, 10f);            
            //target.TakeDamage(9999999f);
            //collision.transform.position += new Vector3(0, -20, 0);
        }
    }
}