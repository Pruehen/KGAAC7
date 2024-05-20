using kjh;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController_AI : MonoBehaviour
{
    kjh.WeaponController weaponController;
    //WeaponSystem weaponSystem;
    int useWeaponIndex = 0;
    Transform target;
    Radar radar;
    [SerializeField][Range(3f, 10f)] float aiLevel;//난이도 설정. 3부터 10까지의 값을 가짐. 값이 클수록 무장을 효율적이고 빠르게 발사함

    //public bool isEngage { get; set; }
    void Start()
    {
        weaponController = GetComponent<kjh.WeaponController>();
        //weaponSystem = GetComponent<WeaponSystem>();
        radar = GetComponent<Radar>();
        target = kjh.GameManager.Instance.player.transform;        
    }

    public void StartWeaponFireCheck()
    {
        StartCoroutine(MissileFireCheck(11 - aiLevel));
    }

    // Update is called once per frame
    IEnumerator MissileFireCheck(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            radar.LockOn();
            float toTargetAngle = radar.toTargetAngle;
            float distance = radar.toTargetDistance;

            WeaponData weaponData = weaponController.GetUseWeaponData();

            float weaponMaxAngle = weaponData.MaxSeekerAngle();
            float weaponMaxRange = weaponData.LockOnRange();

            if (useWeaponIndex == 0 && weaponMaxRange < distance)
            {
                useWeaponIndex = weaponController.ChangeWeapon();
                weaponMaxRange = weaponController.GetUseWeaponData().LockOnRange();
            }
            else if (useWeaponIndex == 1 && weaponMaxRange > distance)
            {
                useWeaponIndex = weaponController.ChangeWeapon();
                weaponMaxRange = weaponController.GetUseWeaponData().LockOnRange();
            }

            if (weaponMaxRange > distance && weaponMaxAngle > toTargetAngle)
            {
                weaponController.Fire();
            }
        }
    }
}
