using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{        
    public List<Transform> Positions_SpawnPositions;

    public static PlayerSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }
}
