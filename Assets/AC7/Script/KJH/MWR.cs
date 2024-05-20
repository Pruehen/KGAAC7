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
    /// ����Ʈ�� �ڽſ��� �ٰ����� �̻����� �߰��ϴ� �޼���
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
    /// ����Ʈ���� �ڽſ��� �ٰ����� �̻����� �����ϴ� �޼���
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
