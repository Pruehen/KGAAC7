using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSystem : NetworkBehaviour
{
    /*[SerializeField] GameObject _projectile;
    [SerializeField] GameObject _FirePos;
    AircraftFMNetwork _owner;
    //Ŭ���̾�Ʈ���� �߻�� �������� �� ��ȣ�� �޾� �߻縦 ���ش�
    //

    private void Awake()
    {
        _owner = transform.GetComponent<AircraftFMNetwork>();
    }
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
