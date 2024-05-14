using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponController : MonoBehaviour
    {
        AircraftSelecter aircraftSelecter;
        [SerializeField] WeaponSystem weaponSystem;
        Radar radar;
        Rigidbody rigidbody;
        
        void Awake()
        {
            aircraftSelecter = GetComponent<AircraftSelecter>();
            rigidbody = GetComponent<Rigidbody>();
            radar = GetComponent<Radar>();
        }

        // Update is called once per frame
        void Update()
        {
            weaponSystem = aircraftSelecter.weaponSystem;
        }

        /// <summary>
        /// ���� �ý��ۿ� ���� �߻縦 ��û�ϴ� �޼���
        /// </summary>
        public void Fire()
        {
            weaponSystem.Fire(rigidbody.velocity, radar.GetTarget());
        }

        /// <summary>
        /// ���� �ý����� ���⸦ ��ü�ϴ� �޼���
        /// </summary>
        public void ChangeWeapon()
        {
            weaponSystem.ChangeWeaponIndex();
        }

        /// <summary>
        /// ���� �ý����� ����� Ʈ���Ÿ� �����ϴ� �޼���
        /// </summary>
        /// <param name="value"></param>
        public void SetTrigger(bool value)
        {
            weaponSystem.SetTrigger(value);
        }
    }
}
