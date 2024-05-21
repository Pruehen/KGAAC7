using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEleminate : MonoBehaviour
{
    [SerializeField] List<VehicleCombat> _targets;

    private void Start()
    {
        kjh.GameManager.Instance.targetCountChanged += TargetCountCheck;
    }
    private void TargetCountCheck(int count)
    {
        if( count == 0 )
        {
            MissionSuccese();
        }
    }

    private void MissionSuccese()
    {
        kjh.GameManager.Instance.GameEnd(true, 2f, 2f);
    }

    
}
