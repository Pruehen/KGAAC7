using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponController : MonoBehaviour
    {
        public GameObject controlAircraft;
        WeaponSystem weaponSystem;
        Radar radar;
        Rigidbody rigidbody;
        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            radar = GetComponent<Radar>();
            weaponSystem = controlAircraft.GetComponent<WeaponSystem>();
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
    }
}
