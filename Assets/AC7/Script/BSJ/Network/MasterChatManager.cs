using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChatManager : NetworkBehaviour
{
    [SerializeField] private NetworkChat _playerChat;
    private void Start()
    {
        bsj.GameManager.Instance.AfterPlayerSpawned += Init;
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
