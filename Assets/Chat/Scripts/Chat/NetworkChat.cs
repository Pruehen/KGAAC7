using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class NetworkChat : NetworkBehaviour
{
    [Header ("SetPlayerColor reference richText")]
    [SerializeField] private string E_playerChatColor = "teal";
    [SerializeField] private MasterChatManager _masterChatManager;

    public void Start()
    {
        base.OnStartLocalPlayer();
        {
            if (isServer)
            {
                _masterChatManager = FindAnyObjectByType<MasterChatManager>();
            }
        }
    }

    public event System.Action<string> OnTextChanged;
    public string _chatContent { get; private set; }
    private string _playerName;
    private string playerName
    {
        get
        {
            if (isServerOnly)
            {
                return "Server";
            }
            if (string.IsNullOrWhiteSpace(_playerName))
            {
                _playerName = FindAnyObjectByType<PlayerData>().PlayerId;
                Debug.Assert(!string.IsNullOrWhiteSpace(_playerName));
            }
            return _playerName;
        }
        set
        {
            _playerName = value;
        }
    }

    public void SendUserChat(string name,string message)
    {
        if(isServerOnly)
        {
            return;
        }
        CommandSendMessage(name, message);
    }

    [Command]
    private void CommandSendMessage(string name, string message)
    {
        _masterChatManager.RpcSendChatMessage(name, message);
    }

    private void AppendMessage(string name, string message)
    {
        _chatContent = $"{_chatContent}{GetChatFormated(name, message)}";
    }
    private void AppendMessage(string chat)
    {
        _chatContent = $"{_chatContent}{chat}";
    }

    public void SetChatMessage(string name, string message)
    {
        if (!isLocalPlayer)
            return;
        string chat;
        if (playerName == name)
        {
            chat = GetChatFormated(name, message, E_playerChatColor);
        }
        else
        {
            chat = GetChatFormated(name, message);
        }
        AppendMessage(chat);
        Debug.Log(netId);
        OnTextChanged?.Invoke(_chatContent);
    }


    private string GetChatFormated(string name, string message)
    {
        return $"{name}: {message}\n";
    }
    private string GetChatFormated(string name, string message, string color)
    {
        return $"<color={color}>{name}</color>: {message}\n";
    }

}
