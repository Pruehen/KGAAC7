using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSystem : NetworkBehaviour
{
    /*[SerializeField] GameObject _projectile;
    [SerializeField] GameObject _FirePos;
    AircraftFM _owner;
    //클라이언트에서 발사시 서버에서 이 신호를 받아 발사를 해준다
    //

    private void Awake()
    {
        _owner = transform.GetComponent<AircraftFM>();
    }

    [Command]
    public void Fire()
    {
        GameObject temp = Instantiate(_projectile, transform.position, transform.rotation);
        temp.transform.GetComponent<PlayerProjectile>().Init(_owner);
        NetworkServer.Spawn(temp);
    }
    [Command]
    public void CmdFire()
    {
        Fire();
    }*/
}
