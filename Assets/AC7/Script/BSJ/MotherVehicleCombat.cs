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
        //��� ������ ������ 10%�� ü���� ����
        TakeDamage(damage, null);
    }



}
