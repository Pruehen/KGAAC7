using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AntiAirMissileLauncher : MonoBehaviour, IAntiAirWeapon
{
    [SerializeField] private Transform _firePos; 
    [SerializeField] private float _launchForce = 10f; 
    private float _fireInterval;
    private float _lifeTime = 0f;

    public float bulletSpeed;
    bool _firing;

    kjh.WeaponSystem _weaponSystem;

    Rigidbody _playerRb;
    Radar _radar;

    private void Start()
    {
        _weaponSystem = GetComponent<kjh.WeaponSystem>();
        //_fireInterval = _projectile.GetComponent<WeaponData>().ReloadTime();
        _fireInterval = 10f;
        _playerRb = kjh.GameManager.Instance.player.GetComponent<Rigidbody>();
        _radar = GetComponent<Radar>();

        _weaponSystem.Init();
    }

    private void Update()
    {
        _lifeTime += Time.deltaTime;

        if (_firing)
        {
            if (_lifeTime >= _fireInterval)
            {
                _radar.LockOn();
                _lifeTime = _fireInterval - Random.Range(0f, 3f);
                _weaponSystem.Fire(_firePos.forward * _launchForce, _radar);
            }
            else
            {
                return;
            }
        }
    }

    public void Fire(bool trigger)
    {
        if (trigger && !_firing)
        {
            _firing = true;
            _lifeTime = _fireInterval - Random.Range(0f, 3f);
        }
        else
        {
            _firing = false;
        }
    }

}

