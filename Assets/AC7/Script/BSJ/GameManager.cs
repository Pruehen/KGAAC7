using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace bsj
{
    public class GameManager : SceneSingleton<GameManager>
    {
        public System.Action AfterPlayerSpawned;
        public Transform player;
        /// <summary>
        /// 현재씬이 멀티플레이인지 확인
        /// </summary>
        public bool IsNetworkScene = false;

        /// <summary>
        /// 실행순서
        /// Awake -> OnSceneLoad -> Start
        /// Awake -> NetworkIdentitySpawn -> Start
        /// 네트워크와 씬로드 순서는 실험 더해야할듯
        /// </summary>

        private void Awake()
        {
            //씬 로드시 IsNetworkScene을 업데이트하기위함
            SceneManager.sceneLoaded += IsNetworkOnSceneLoad;
            if(!IsNetworkScene)
            {
                player = GameObject.Find("Aircraft").transform;
            }
        }
        private void Start()
        {
            
        }

        /// <summary>
        ///  플레이어 스폰시 이벤트 발생을 하기위한 메서드
        /// </summary>
        /// <param name="player"></param>
        public void TriggerNetworkPlayerSpawn(Transform player)
        {
            this.player = player;
            StartCoroutine(TriggerOnAfterPlayerSpawned());
        }

        private IEnumerator TriggerOnAfterPlayerSpawned()
        {
            yield return null;
            yield return new WaitForSeconds(.3f);
            AfterPlayerSpawned?.Invoke();
        }


        /// <summary>
        /// 현재씬이 멀티플레이인지 확인한다.
        /// </summary>
        private void IsNetworkOnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            IsNetworkScene = IsNetworkManagerExist();
            if (!IsNetworkScene)
            {
                AfterPlayerSpawned?.Invoke();
            }
        }

        private bool IsNetworkManagerExist()
        {
            NetworkManager network = FindAnyObjectByType<NetworkManager>();
            return network != null;
        }
    }
}
