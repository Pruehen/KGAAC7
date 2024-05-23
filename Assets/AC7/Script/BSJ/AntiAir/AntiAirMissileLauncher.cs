using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AntiAirMissileLauncher : MonoBehaviour, IAntiAirWeapon
{
    [SerializeField] private Transform _firePos;
    private float _fireInterval;
    private float _lifeTime = 0f;

    [SerializeField] private GameObject _projectile;
    public float bulletSpeed;
    bool _firing;

    kjh.WeaponSystem _weaponSystem;

    Rigidbody _playerRb;
    Radar _radar;

    private void Start()
    {
        _weaponSystem = GetComponent<kjh.WeaponSystem>();
        //_fireInterval = _projectile.GetComponent<WeaponData>().ReloadTime();
        _fireInterval = 3f;
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
                _lifeTime = 0f;
                _weaponSystem.Fire(_playerRb.velocity, _radar);
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
        }
        else
        {
            _firing = false;
        }
    }

}

