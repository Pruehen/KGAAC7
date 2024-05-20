using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponController : MonoBehaviour
    {
        AircraftSelecter aircraftSelecter;
        kjh.WeaponSystem weaponSystem;
        Radar radar;
        Rigidbody rigidbody;

        void Awake()
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
        /// ���� �ý��ۿ� ���� �߻縦 ��û�ϴ� �޼���
        /// </summary>
        public void Fire()
        {
            weaponSystem.Fire(rigidbody.velocity, radar.GetTarget(), radar.toTargetAngle, radar.toTargetDistance);
        }

        /// <summary>
        /// ���� �ý����� ���⸦ ��ü�ϴ� �޼���
        /// </summary>
        public int ChangeWeapon()
        {
            return weaponSystem.ChangeWeaponIndex();
        }
        public void ChangeWeaponVoid()
        {
            weaponSystem.ChangeWeaponIndex();
        }

        /// <summary>
        /// ���� �ý����� ����� Ʈ���Ÿ� �����ϴ� �޼���
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
