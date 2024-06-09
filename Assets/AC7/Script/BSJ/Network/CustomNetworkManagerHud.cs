using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManagerHud : NetworkManagerHUD
{
    PlayerData playerData;
    protected override void Awake()
    {
        base.Awake();
        playerData = FindAnyObjectByType<PlayerData>();
    }

    protected override void OnGUI()
    {
        if(!string.IsNullOrWhiteSpace(playerData.RoomId))
        {
            RoomLable();
        }
    }

    private void RoomLable()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label($"RoomId: {playerData.RoomId}");

        GUILayout.EndHorizontal();
    }

}
