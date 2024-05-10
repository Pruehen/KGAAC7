using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    [SerializeField] GameObject _projectile;

    //클라이언트에서 발사시 서버에서 이 신호를 받아 발사를 해준다
    [ClientRpc]
    public void Fire()
    {
        Instantiate(_projectile);
    }
    [Command]
    public void CmdFire()
    {
        Fire();
    }
}
