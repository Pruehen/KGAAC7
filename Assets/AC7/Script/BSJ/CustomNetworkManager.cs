using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] Transform Transform_PlayerSpawnParent;
    //public string userName { get; set; }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //Debug.Log("서버 스폰 완료");
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation, Transform_PlayerSpawnParent)
            : Instantiate(playerPrefab, Transform_PlayerSpawnParent);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        //VehicleCombat playerVehicleCombat = player.transform.GetChild(0).GetComponent<VehicleCombat>();
        //playerVehicleCombat.SetNames(userName);
        //kjh.GameManager.Instance.AddActiveTarget(playerVehicleCombat);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
