using Mirror;
using Steamworks;
using System;
using UnityEngine;


public class SteamLobby : MonoBehaviour
{
    public GameObject hostButton = null;

    NetworkManager _networkManager;

    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> _lobbyEntered;

    private const string _hostAddressKey = "HostAddress";

    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        hostButton.SetActive(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            return;
        }

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), _hostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), _hostAddressKey);

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();

        hostButton.SetActive(false);
    }
}
