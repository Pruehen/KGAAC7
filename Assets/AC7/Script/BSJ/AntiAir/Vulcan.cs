using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulcan : MonoBehaviour, IAntiAirWeapon
{
    [SerializeField] private Transform _firePos;
    private float _fireInterval;
    private float _lifeTime = 0f;

    [SerializeField] private GameObject _bulletProjectile;
    public float bulletSpeed;

    bool _firing;
    FadableAudio _gunSound;
    [SerializeField] private float _bulletSpread = .5f;

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
                GameObject item = ObjectPoolManager.Instance.DequeueObject(_bulletProjectile);
                float randomPitch = Random.Range(-_bulletSpread / 2f, _bulletSpread/ 2f);
                float randomYaw = Random.Range(-_bulletSpread / 2f, _bulletSpread/ 2f);
                Quaternion randomRotation = Quaternion.Euler(randomPitch, randomYaw, 0f);
                Quaternion targetRotation = _firePos.rotation * randomRotation;
                item.GetComponent<kjh.Bullet>().Init(_firePos.position, targetRotation * Vector3.forward * bulletSpeed);
            }
            else
            {
                return;
            }
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
