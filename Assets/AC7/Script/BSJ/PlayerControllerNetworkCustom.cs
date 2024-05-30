using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerNetworkCustom : NetworkBehaviour
{
    [SerializeField] private GameObject[] NotForMulti;

    private void Start()
    {

        if(!isLocalPlayer)
        {
            foreach(var item in NotForMulti)
            {
                item.SetActive(false);
            }
            return;
        }
        bsj.GameManager.Instance.TriggerNetworkPlayerSpawn(transform);
    }
    private void OnPlayerSpawn()
    {

    }

    [Command]
    public void CommandFireWeaponNetwork()
    {

        RpcFireWeaponNetwork();
    }

    [ClientRpc]
    public void RpcFireWeaponNetwork()
    {

    }

}
