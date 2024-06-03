using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponSystem : NetworkBehaviour
    {
        [SerializeField] List<GameObject> weaponPrfList;        
        List<WeaponData> weaponDataList = new List<WeaponData>();
        public List<WeaponData> WeaponDataList() { return weaponDataList; }
        [SerializeField] List<Transform> fireTrfList;
        [SerializeField] List<int> equipedWeaponIndexList;
        public List<int> EquipedWeaponIndexList() { return equipedWeaponIndexList; }
        List<float> weaponCoolDownList; //MSL 무기 개수        

        float innerFireDelay = 0;

        public float MslCoolDownRatio(int index) //0부터 시작해서 참
        {
            float coolTime = weaponPrfList[equipedWeaponIndexList[index]].GetComponent<WeaponData>().ReloadTime();
            return 1 - (weaponCoolDownList[index] / coolTime);
        }
        public bool ActiveMissile(int index)
        {
            return (equipedWeaponIndexList[index] == useWeaponIndex);
        }

        public WeaponData UseWeaponData()
        {
            return weaponDataList[useWeaponIndex];
        }


        public int useWeaponIndex { get; private set; }
        public System.Action weaponChange;

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

        FadableAudio _gunAudio;

        public void SetGunTrigger(bool value)
        {
            gunTrigger = value;
            if (value)
            {
                _gunAudio.Play();
            }
            else
            {
                _gunAudio.Stop();
            }
        }
        public void SetFlareTrigger(bool value)
        {
            flareTrigger = value;
        }
        /// <summary>
        /// 현재 사용중인 무장 인덱스를 교체하는 메서드
        /// </summary>
        public int ChangeWeaponIndex()
        {
            useWeaponIndex++;
            if(useWeaponIndex == 2)
            {
                useWeaponIndex = 0;
            }
            weaponChange?.Invoke();
            return useWeaponIndex;
            //Debug.Log(useWeaponIndex);
        }

        // Start is called before the first frame update
        public void Init()
        {
            if (fireTrfList.Count == 0)
                return;

            useWeaponIndex = 0;

            weaponCoolDownList = new List<float>();
            for (int i = 0; i < fireTrfList.Count; i++)
            {
                weaponCoolDownList.Add(0f);
            }

            gunTrigger = false;
            rigidbody = this.transform.parent?.GetComponent<Rigidbody>();
            vehicleCombat = this.transform.parent?.GetComponent<VehicleCombat>();

            _gunAudio = GetComponent<FadableAudio>();
            _gunAudio?.SetParent(gunFireTrf);

            for (int i = 0; i < weaponPrfList.Count; i++)
            {
                weaponDataList.Add(weaponPrfList[i].GetComponent<WeaponData>());                
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (weaponCoolDownList == null || weaponCoolDownList.Count == 0)
                return;

            for (int i = 0; i < weaponCoolDownList.Count; i++)
            {
                if (weaponCoolDownList[i] > 0)
                {
                    weaponCoolDownList[i] -= Time.deltaTime;
                    if(weaponCoolDownList[i] < 0)
                    {
                        fireTrfList[i].gameObject.SetActive(true);
                    }
                }
            }

            if (innerFireDelay < 0.1f)
            {
                innerFireDelay += Time.deltaTime;
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
        /// <param name="initailVelocity"></param>
        /// <param name="target"></param>
        public void Fire(Vector3 initailVelocity, Radar radar)
        {
            if (innerFireDelay < 0.1f)
                return;

            GameObject useWeaponPrf = weaponPrfList[useWeaponIndex];
            Transform firePoint = null;

            bool canFire = false;
            
            for (int i = 0; i < weaponCoolDownList.Count; i++)
            {
                int k;
                if(i % 2 == 0)
                {
                    k = i / 2;
                }
                else
                {
                    k = weaponCoolDownList.Count - (i / 2) - 1;
                }

                if (equipedWeaponIndexList[k] == useWeaponIndex)
                {
                    if (weaponCoolDownList[k] <= 0)
                    {
                        firePoint = fireTrfList[k];
                        fireTrfList[k].gameObject.SetActive(false);
                        weaponCoolDownList[k] = useWeaponPrf.GetComponent<WeaponData>().ReloadTime();
                        canFire = true;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (canFire)
            {
                GameObject item = Instantiate(useWeaponPrf, firePoint.position, firePoint.rotation);//로컬 미사일 스폰                

                item.GetComponent<Rigidbody>().velocity = initailVelocity;

                Guided guided;
                if (item.TryGetComponent(out guided))
                {
                    guided.SetTarget(radar);
                }

                //CommandCreateServerMissile(useWeaponPrf, firePoint.position, firePoint.rotation, initailVelocity, radar);
            }
        }
    }
}
