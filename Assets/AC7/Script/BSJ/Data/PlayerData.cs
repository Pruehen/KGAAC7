using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string PlayerId {get; private set;}
    public string PlayerAircraft { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Init(string playerId)
    {
        PlayerId = playerId;
    }

    public void SetPlayerAircraft(string playerAircraft)
    {
        PlayerAircraft = playerAircraft;
    }
}
