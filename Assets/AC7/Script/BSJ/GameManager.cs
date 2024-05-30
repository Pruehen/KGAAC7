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
        /// ������� ��Ƽ�÷������� Ȯ��
        /// </summary>
        public bool IsNetworkScene = false;

        /// <summary>
        /// �������
        /// Awake -> OnSceneLoad -> Start
        /// Awake -> NetworkIdentitySpawn -> Start
        /// ��Ʈ��ũ�� ���ε� ������ ���� ���ؾ��ҵ�
        /// </summary>

        private void Awake()
        {
            //�� �ε�� IsNetworkScene�� ������Ʈ�ϱ�����
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
        ///  �÷��̾� ������ �̺�Ʈ �߻��� �ϱ����� �޼���
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
        /// ������� ��Ƽ�÷������� Ȯ���Ѵ�.
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
