using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : Mirror.NetworkManager
{
    [SerializeField] Transform Transform_PlayerSpawnParent;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation, Transform_PlayerSpawnParent)
            : Instantiate(playerPrefab, Transform_PlayerSpawnParent);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        VehicleCombat playerVehicleCombat = player.transform.GetChild(0).GetComponent<VehicleCombat>();
        kjh.GameManager.Instance.AddActiveTarget(playerVehicleCombat);
    }
}
