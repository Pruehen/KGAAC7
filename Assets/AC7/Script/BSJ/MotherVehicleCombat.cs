using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherVehicleCombat : VehicleCombat
{
    VehicleCombat[] _childCombats;



    private void Start()
    {
        _childCombats = GetComponentsInChildren<VehicleCombat>();
        foreach(VehicleCombat combat in _childCombats)
        {
            combat.onDead.AddListener(TakePartBreakDamage);
            base.onDead.AddListener(combat.Die);
        }
        
    }

    private void TakePartBreakDamage()
    {
        float damage = (startHp * .9f) / _childCombats.Length;
        //모든 파츠가 죽으면 10%의 체력이 남음
        TakeDamage(damage, null);
    }



}
