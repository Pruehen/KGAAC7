using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponController : MonoBehaviour
    {
        AircraftSelecter aircraftSelecter;
        WeaponSystem weaponSystem;
        Radar radar;
        Rigidbody rigidbody;

        void Awake()
        {
            aircraftSelecter = GetComponent<AircraftSelecter>();
            rigidbody = GetComponent<Rigidbody>();
            radar = GetComponent<Radar>();
            weaponSystem = aircraftSelecter.weaponSystem;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// 웨폰 시스템에 무장 발사를 요청하는 메서드
        /// </summary>
        public void Fire()
        {
            weaponSystem.Fire(rigidbody.velocity, radar.GetTarget());
        }

        /// <summary>
        /// 웨폰 시스템의 무기를 교체하는 메서드
        /// </summary>
        public void ChangeWeapon()
        {
            weaponSystem.ChangeWeaponIndex();
        }

        /// <summary>
        /// 웨폰 시스템의 기관포 트리거를 설정하는 메서드
        /// </summary>
        /// <param name="value"></param>
        public void SetGunTrigger(bool value)
        {
            weaponSystem.SetGunTrigger(value);
        }
        public void SetFlareTrigger(bool value)
        {
            weaponSystem.SetFlareTrigger(value);
        }
    }
}
