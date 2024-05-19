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

    bool _firing;
    FadableAudio _gunSound;

    private void Start()
    {
        _fireInterval = _bulletProjectile.GetComponent<WeaponData>().ReloadTime();
        _gunSound = GetComponent<FadableAudio>();
    }

    private void Update()
    {
        _lifeTime += Time.deltaTime;

        if(_firing)
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
        }
    }


    public void Fire(bool trigger)
    {
        if (trigger)
        {
            _firing = true;
            if(!_gunSound.IsPlaying())
            {
                _gunSound.Play();
            }
        }
        else
        {
            _firing = false;
            _gunSound.Stop();
        }
    }

}
