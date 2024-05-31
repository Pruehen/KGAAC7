using Mirror;
using UnityEngine;

namespace kjh
{
    public class WeaponController : NetworkBehaviour
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
            if (weaponSystem == null)
                weaponSystem = aircraftSelecter.weaponSystem;

            weaponSystem.Fire(rigidbody.velocity, radar);
        }

        /// <summary>
        /// ���� �ý����� ���⸦ ��ü�ϴ� �޼���
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
        /// ���� �ý����� ����� Ʈ���Ÿ� �����ϴ� �޼���
        /// </summary>
        /// <param name="value"></param>
        public void SetGunTrigger(bool value)
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
