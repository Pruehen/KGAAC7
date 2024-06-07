using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace kjh
{
    public class WeaponController : NetworkBehaviour
    {
        AircraftSelecter aircraftSelecter;
        kjh.WeaponSystem weaponSystem;
        Radar radar;
        Rigidbody rigidbody;

        void Start()
        {
            aircraftSelecter = GetComponent<AircraftSelecter>();
            rigidbody = GetComponent<Rigidbody>();
            radar = GetComponent<Radar>();
            weaponSystem = aircraftSelecter.weaponSystem;
        }

        public WeaponData GetUseWeaponData()
        {
            if(weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;

            return weaponSystem.UseWeaponData();
        }

        /// <summary>
        /// 웨폰 시스템에 무장 발사를 요청하는 메서드
        /// </summary>
        public void Fire()
        {
            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;
            weaponSystem.Fire(rigidbody.velocity, radar);
        }

        /// <summary>
        /// 웨폰 시스템의 무기를 교체하는 메서드
        /// </summary>
        public int ChangeWeapon()
        {
            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;
            return weaponSystem.ChangeWeaponIndex();
        }
        public void ChangeWeaponVoid()
        {
            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;
            weaponSystem.ChangeWeaponIndex();
        }

        /// <summary>
        /// 웨폰 시스템의 기관포 트리거를 설정하는 메서드
        /// </summary>
        /// <param name="value"></param>
        public void SetGunTrigger(bool value)
        {
            if(isServerOnly)
            {
                ExeSetGunTrigger(value);
                RpcSetGunTrigger(value);
            }
            else if(isServer)
            {
                RpcSetGunTrigger(value);
            }
        }

        [ClientRpc]
        private void RpcSetGunTrigger(bool value)
        {
            ExeSetGunTrigger(value);
        }

        private void ExeSetGunTrigger(bool value)
        {

            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;
            weaponSystem.SetGunTrigger(value);
        }

        public void SetFlareTrigger(bool value)
        {
            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;
            weaponSystem.SetFlareTrigger(value);
        }
    }
}
