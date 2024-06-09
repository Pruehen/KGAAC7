using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterChatManager : NetworkBehaviour
{
    [SerializeField] private NetworkChat _playerChat;
    private void Awake()
    {
        bsj.GameManager.Instance.AfterPlayerSpawned += Init;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += PrintDebug;
    }

    private void PrintDebug(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.LogError("MasterChatManagerOnLoadScene");
    }

    private void Init()
    {
        _playerChat = bsj.GameManager.Instance.player.parent.GetComponentInChildren<NetworkChat>();
    }



    [ClientRpc]
    public void RpcSendChatMessage(string name, string message)
    {
        _playerChat.SetChatMessage(name, message);
    }

}
