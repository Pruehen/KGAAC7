using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [SerializeField] float dmg;
    public float Dmg() { return dmg; }
    [SerializeField] float reloadTime;
    public float ReloadTime() { return reloadTime; }
}
