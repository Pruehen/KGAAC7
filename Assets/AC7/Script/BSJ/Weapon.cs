using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    [SerializeField] GameObject _projectile;

    //Ŭ���̾�Ʈ���� �߻�� �������� �� ��ȣ�� �޾� �߻縦 ���ش�
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
