using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerNetworkCustom : NetworkBehaviour
{
    [SerializeField] private GameObject[] OnlyForLocal;
    [SerializeField] private GameObject[] OnlyForServer;
    [SerializeField] private MonoBehaviour[] OnlyForServerComponent;

    private void Start()
    {

        if (!isLocalPlayer)
        {
            foreach (var item in OnlyForLocal)
            {
                item.SetActive(false);
            }
        }
        else
        {
            bsj.GameManager.Instance.TriggerNetworkPlayerSpawn(transform);
            if(!isServer)
            {
                foreach (var item in OnlyForServer)
                {
                    item.SetActive(false);
                }
                foreach (var item in OnlyForServerComponent)
                {
                    item.enabled = false;
                    
                }
            }
        }
    }
}
