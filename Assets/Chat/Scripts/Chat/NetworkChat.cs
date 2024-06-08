using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkChat : NetworkBehaviour
{
    [Header ("SetPlayerColor reference richText")]
    [SerializeField] private string E_playerChatColor = "teal";

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
                _playerName = NetworkClient.localPlayer.name;
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

    [Command(requiresAuthority = false)]
    private void CommandSendMessage(string name, string message)
    {
        RpcSendMessage(name, message);
    }

    private void AppendMessage(string name, string message)
    {
        _chatContent = $"{_chatContent}{GetChatFormated(name, message)}";
    }

    [ClientRpc]
    private void RpcSendMessage(string name, string message)
    {
        string chat;
        if(playerName == name)
        {
            chat = GetChatFormated(name, message, E_playerChatColor);
        }
        else
        {
            chat = GetChatFormated(name, message);
        }
        AppendMessage(name, message);
        OnTextChanged?.Invoke(_chatContent);
    }

    private string GetChatFormated(string name, string message)
    {
        return $"{name}: {message}\n";
    }
    private string GetChatFormated(string name, string message, string color)
    {
        return $"<color={color}>{GetChatFormated(name, message)}</color>\n";
    }

}
