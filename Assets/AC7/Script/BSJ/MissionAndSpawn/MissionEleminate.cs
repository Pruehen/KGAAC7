using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEleminate : MonoBehaviour
{
    [SerializeField] List<VehicleCombat> _targets;
    int _targetCount;

    private void Start()
    {
        foreach (VehicleCombat combat in _targets)
        {
            combat.onDead.AddListener(TargetEleminated);
        }
        _targetCount = _targets.Count;
    }

    private void TargetEleminated()
    {
        _targetCount--;
        if( _targetCount == 0 )
        {
            MissionSuccese();
        }
    }

    private void MissionSuccese()
    {
        kjh.GameManager.Instance.GameEnd(true, 2f, 2f);
    }

    
}
