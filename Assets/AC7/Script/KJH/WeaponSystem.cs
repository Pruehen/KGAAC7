using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] GameObject weaponPrf_01;
        [SerializeField] GameObject weaponPrf_02;
        [SerializeField] List<Transform> fireTrf_01;
        [SerializeField] List<Transform> fireTrf_02;
        List<float> weapon01CoolDown;
        List<float> weapon02CoolDown;

        int useWeaponIndex;

        [SerializeField] GameObject bulletPrf;
        [SerializeField] Transform gunFireTrf;
        bool gunTrigger = false;
        float gunFireDelay = 0.05f;
        float gunDelayTime = 0;
        Rigidbody rigidbody;

        [SerializeField] GameObject flarePrf;
        bool flareTrigger = false;
        float flareFireDelay = 0.1f;
        float flareDelayTime = 0;

        VehicleCombat vehicleCombat;

        public void SetGunTrigger(bool value)
        {
            gunTrigger = value;
        }
        public void SetFlareTrigger(bool value)
        {
            flareTrigger = value;
        }
        /// <summary>
        /// 현재 사용중인 무장 인덱스를 교체하는 메서드
        /// </summary>
        public void ChangeWeaponIndex()
        {
            useWeaponIndex++;
            if(useWeaponIndex == 2)
            {
                useWeaponIndex = 0;
            }

            //Debug.Log(useWeaponIndex);
        }

        // Start is called before the first frame update
        void Start()
        {
            useWeaponIndex = 0;

            weapon01CoolDown = new List<float>();
            for (int i = 0; i < fireTrf_01.Count; i++)
            {
                weapon01CoolDown.Add(0f);
            }

            weapon02CoolDown = new List<float>();
            for (int i = 0; i < fireTrf_02.Count; i++)
            {
                weapon02CoolDown.Add(0f);
            }

            gunTrigger = false;
            rigidbody = this.transform.parent.GetComponent<Rigidbody>();
            vehicleCombat = this.transform.parent.GetComponent<VehicleCombat>();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < weapon01CoolDown.Count; i++)
            {
                if (weapon01CoolDown[i] > 0)
                {
                    weapon01CoolDown[i] -= Time.deltaTime;
                    if(weapon01CoolDown[i] < 0)
                    {
                        fireTrf_01[i].gameObject.SetActive(true);
                    }
                }
            }
            for (int i = 0; i < weapon02CoolDown.Count; i++)
            {
                if (weapon02CoolDown[i] > 0)
                {
                    weapon02CoolDown[i] -= Time.deltaTime;
                    if (weapon02CoolDown[i] < 0)
                    {
                        fireTrf_02[i].gameObject.SetActive(true);
                    }
                }
            }

            GunFire();
            FlareDeploy();
        }

        void GunFire()
        {
            if(gunDelayTime > 0)
            {
                gunDelayTime -= Time.deltaTime;
            }    

            if(gunTrigger && gunDelayTime <= 0)
            {
                gunDelayTime = gunFireDelay;
                GameObject item = ObjectPoolManager.Instance.DequeueObject(bulletPrf);
                item.GetComponent<Bullet>().Init(gunFireTrf.position, rigidbody.velocity + gunFireTrf.forward * 1000);
            }

        }

        void FlareDeploy()
        {
            if (flareDelayTime > 0)
            {
                flareDelayTime -= Time.deltaTime;
            }
            if (flareTrigger && flareDelayTime <= 0)
            {
                flareDelayTime = flareFireDelay;
                GameObject item = ObjectPoolManager.Instance.DequeueObject(flarePrf);
                item.GetComponent<Flare>().Init(this.transform.position, rigidbody.velocity + this.transform.up * -30 + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)));
                vehicleCombat.FlareDeploy();
            }
        }

        /// <summary>
        /// 현재 적용중인 프리팹을 발사하는 메서드
        /// </summary>
        /// <param name="aircraftVelocity"></param>
        /// <param name="target"></param>
        public void Fire(Vector3 aircraftVelocity, VehicleCombat target)
        {
            GameObject useWeaponPrf;
            List<Transform> useWeaponPointList;
            List<float> useWeaponCoolDownList;
            Transform firePoint = null;

            bool canFire = false;

            if (useWeaponIndex == 0)
            {
                useWeaponPrf = weaponPrf_01;
                useWeaponPointList = fireTrf_01;
                useWeaponCoolDownList = weapon01CoolDown;
            }
            else
            {
                useWeaponPrf = weaponPrf_02;
                useWeaponPointList = fireTrf_02;
                useWeaponCoolDownList = weapon02CoolDown;
            }
            for (int i = 0; i < useWeaponCoolDownList.Count; i++)
            {
                if (useWeaponCoolDownList[i] <= 0)
                {
                    firePoint = useWeaponPointList[i];
                    useWeaponPointList[i].gameObject.SetActive(false);
                    useWeaponCoolDownList[i] = useWeaponPrf.GetComponent<WeaponData>().ReloadTime();
                    canFire = true;
                    break;
                }
            }

            if (canFire)
            {
                GameObject item = Instantiate(useWeaponPrf, firePoint.position, firePoint.rotation);

                item.GetComponent<Rigidbody>().velocity = aircraftVelocity;

                Guided guided;
                if (item.TryGetComponent(out guided))
                {
                    guided.SetTarget(target);
                }
            }
        }
    }
}
