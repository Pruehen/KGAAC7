using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class GameManager : SceneSingleton<GameManager>
    {
        public System.Action OnPlayerSpawned;
        public Transform player;

        public void TriggerPlayerSpawn(Transform player)
        {
            this.player = player;
            OnPlayerSpawned?.Invoke();
        }
    }
}
