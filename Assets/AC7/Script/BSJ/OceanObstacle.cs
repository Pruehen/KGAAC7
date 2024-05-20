using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌시 사망하는 바다 클래스
/// </summary>
public class OceanObstacle : MonoBehaviour
{
    [SerializeField] GameObject _spashVfxPrefab;
    private void OnTriggerEnter(Collider col)
    {
        IFightable target;
        if (col.transform.TryGetComponent<IFightable>(out target))
        {
            GameObject pooledItem = ObjectPoolManager.Instance.DequeueObject(_spashVfxPrefab);
            pooledItem.transform.position = col.transform.position;
            ObjectPoolManager.Instance.EnqueueObject(pooledItem, 10f);
            target.TakeDamage(9999999f);
        }
    }
}