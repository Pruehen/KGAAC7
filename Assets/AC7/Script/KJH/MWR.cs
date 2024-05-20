using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MWR : MonoBehaviour
{
    //List<Guided> incomeMissileList = new List<Guided>();
    bool isPlayer;

    private void Start()
    {
        isPlayer = GetComponent<AircraftMaster>().IsPlayer();
    }

    /// <summary>
    /// 리스트에 자신에게 다가오는 미사일을 추가하는 메서드
    /// </summary>
    /// <param name="missile"></param>
    public void AddMissile(Guided missile)
    {
        //incomeMissileList.Add(missile);

        if(isPlayer)
        {
            MissileIndicatorController.Instance.AddMissileIndicator(missile);
        }
    }

    /// <summary>
    /// 리스트에서 자신에게 다가오던 미사일을 제거하는 메서드
    /// </summary>
    /// <param name="missile"></param>
    public void RemoveMissile(Guided missile)
    {
        //incomeMissileList.Remove(missile);

        if (isPlayer)
        {
            MissileIndicatorController.Instance.RemoveMissileIndicator(missile);
        }
    }
}
