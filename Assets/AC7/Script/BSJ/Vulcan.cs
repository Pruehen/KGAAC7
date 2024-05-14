using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulcan : MonoBehaviour
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private float _fireInterval = 0.01f;
    private float _lifeTime = 0f;

    [SerializeField] private GameObject _bulletProjectile;
    public float bulletSpeed;


    private void Awake()
    {
        bulletSpeed = _bulletProjectile.GetComponent<bsj.Bullet>()._bulletSpeed;
    }
    private void Update()
    {
        if(_lifeTime >= _fireInterval)
        {
            _lifeTime = 0f;
            Fire();
        }
    }

    public void Fire()
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(_bulletProjectile);
        bsj.Bullet bullet = item.GetComponent<bsj.Bullet>();
        bullet.transform.position = _firePos.position;
        bullet.transform.rotation = _firePos.rotation;
        bullet.Init();
    }
}
