using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulcan : MonoBehaviour
{
    [SerializeField] private Transform _firePos;
    private float _fireInterval;
    private float _lifeTime = 0f;

    [SerializeField] private GameObject _bulletProjectile;
    public float bulletSpeed;


    private void Start()
    {
        _fireInterval = _bulletProjectile.GetComponent<WeaponData>().ReloadTime();
    }

    private void Update()
    {
        _lifeTime += Time.deltaTime;
    }


    public void Fire()
    {
        if (_lifeTime >= _fireInterval)
        {
            _lifeTime = 0f;
        }
        else
        {
            return;
        }
        GameObject item = ObjectPoolManager.Instance.DequeueObject(_bulletProjectile);
        Rigidbody bullet = item.GetComponent<Rigidbody>();
        bullet.transform.position = _firePos.position;
        bullet.transform.rotation = _firePos.rotation;
        bullet.velocity = bullet.transform.forward * bulletSpeed;
        ObjectPoolManager.Instance.EnqueueObject(item, 30f);
    }

}
