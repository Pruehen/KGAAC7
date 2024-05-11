using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] GameObject _projectile;

    //클라이언트에서 발사시 서버에서 이 신호를 받아 발사를 해준다
    //
    [Command]
    public void Fire()
    {
        GameObject temp = Instantiate(_projectile);
        NetworkServer.Spawn(temp);
    }
    [Command]
    public void CmdFire()
    {
        Fire();
    }
}
