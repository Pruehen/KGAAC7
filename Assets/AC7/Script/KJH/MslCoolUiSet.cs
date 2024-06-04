using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MslCoolUiSet : MonoBehaviour
{
    [SerializeField] GameObject mslCoolUIPrf_S;
    [SerializeField] GameObject mslCoolUIPrf_M;
    kjh.WeaponSystem weaponSystem;
    // Start is called before the first frame update
    private void Start()
    {
        //bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
        OnPlayerSpawn();
    }
    private void OnPlayerSpawn()
    {
        weaponSystem = kjh.GameManager.Instance.player.AircraftSelecter().weaponSystem;
        List<int> weaponIndexList = weaponSystem.EquipedWeaponIndexList();

        for (int i = 0; i < weaponIndexList.Count; i++)
        {
            GameObject prf;
            if (weaponIndexList[i] == 0)
            {
                prf = mslCoolUIPrf_S;
            }
            else
            {
                prf = mslCoolUIPrf_M;
            }
            GameObject item = Instantiate(prf, this.transform);
            item.transform.localPosition = SetLocalPos(i, weaponIndexList.Count);
            item.GetComponent<CooldownImage>().Init(i);
        }
    }

    Vector3 SetLocalPos(int index, int maxIndex)
    {
        int centerIndex = maxIndex / 2;
        if(index < centerIndex)
        {
            return new Vector3((index - centerIndex + 1) * 30 - 100, -20, 0);
        }
        else
        {
            return new Vector3((index - centerIndex) * 30 + 100, -20, 0);
        }
    }
}
