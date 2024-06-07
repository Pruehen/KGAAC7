using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kjh;
using Mirror;

//어떤 항공기를 사용하는지 알려주는 클래스
public class AircraftSelecter : NetworkBehaviour
{
    public GameObject controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }
    public AircraftControl aircraftControl { get; private set; }

    public string name { get; private set; }

    private void Start()
    {
        if (isLocalPlayer)
        {
            PlayerData playerData = FindAnyObjectByType<PlayerData>();
            CommandSetControlAircraft(controlAircraft, playerData.PlayerAircraft);
        }
        else
        {
            bsj.GameManager.Instance.AfterAnyPlayerSpawned += CommandSyncControlAircraft;
        }
    }

    /// <summary>
    /// aircraft가 사용할 항공기를 정해주는 메서드
    /// </summary>
    /// <param name="controlAircraft"></param>
    public void SetControlAircraft(GameObject controlAircraft, string aircraftName)
    {
        name = aircraftName;
        if(string.IsNullOrWhiteSpace(name))
        {
            controlAircraft = transform.Find("F-16C").gameObject;
        }
        if (this.aircraftControl != null)
        {
            this.controlAircraft.SetActive(false);
        }

        if(controlAircraft == null)
        {
            //if (GameObject.Find("_F-16C") != null)
            //{
            //    name = "F-16C";
            //    //Destroy(GameObject.Find("_F-16C"));
            //}
            //else if(GameObject.Find("_MiG-29A") != null)
            //{
            //    name = "MiG-29A";
            //    //Destroy(GameObject.Find("_MiG-29A"));
            //}
            //else if (GameObject.Find("_F-14A") != null)
            //{
            //    name = "F-14A";
            //    //Destroy(GameObject.Find("_F-14A"));
            //}
            //else if (GameObject.Find("_F-15C") != null)
            //{
            //    name = "F-15C";
            //    //Destroy(GameObject.Find("_F-15C"));
            //}
            //else if (GameObject.Find("_Su-37") != null)
            //{
            //    name = "Su-37";
            //    //Destroy(GameObject.Find("_Su-37"));
            //}
            //else
            //{
            //    name = "F-16C";
            //}

            controlAircraft = transform.Find(name)?.gameObject;
            if (controlAircraft == null)
            {
                controlAircraft = transform.Find("F-16C").gameObject;
            }
        }

        this.controlAircraft = controlAircraft;
        this.controlAircraft.SetActive(true);
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        this.gameObject.GetComponent<AircraftFM>().Init();
        weaponSystem = controlAircraft.GetComponent<kjh.WeaponSystem>();
        weaponSystem?.Init();
        aircraftControl = controlAircraft.GetComponent<AircraftControl>();
    }

    [Command]
    private void CommandSetControlAircraft(GameObject controlAircraft, string aircraftName)
    {
        if(isServerOnly)
        {
            SetControlAircraft(controlAircraft, aircraftName);
        }
        else
        {
            RpcSetControlAircraft(controlAircraft, aircraftName);
        }
    }
    [ClientRpc]
    private void RpcSetControlAircraft(GameObject controlAircraft, string aircraftName)
    {
        SetControlAircraft(controlAircraft?.gameObject, aircraftName);
    }

    [Command (requiresAuthority = false)]
    private void CommandSyncControlAircraft()
    {
        RpcSyncControlAircraft(name);
    }
    [ClientRpc]
    private void RpcSyncControlAircraft(string aircraftName)
    {
        SetControlAircraft(controlAircraft ,aircraftName);
    }
}
