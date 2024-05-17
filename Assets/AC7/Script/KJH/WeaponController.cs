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


        [Header("���� ����")]
        [SerializeField] AudioSource _gunSound;

        void Awake()
        {
            aircraftSelecter = GetComponent<AircraftSelecter>();
            rigidbody = GetComponent<Rigidbody>();
            radar = GetComponent<Radar>();
            _gunSound = GetComponent<AudioSource>();
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
        public void SetGunTrigger(bool value)
        {
            weaponSystem.SetGunTrigger(value);

            if (value)
            {
                _gunSound.Play();
            }
            else
            {
                _gunSound.Stop();
            }
        }
        public void SetFlareTrigger(bool value)
        {
            weaponSystem.SetFlareTrigger(value);
        }
    }
}
