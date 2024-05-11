using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSystem : NetworkBehaviour
{
    [SerializeField] GameObject _projectile;

    //Ŭ���̾�Ʈ���� �߻�� �������� �� ��ȣ�� �޾� �߻縦 ���ش�
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
