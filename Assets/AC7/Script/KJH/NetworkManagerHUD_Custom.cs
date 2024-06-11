using UnityEngine;
using Mirror;
using System.Collections;

/// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Manager HUD")]
[RequireComponent(typeof(NetworkManager))]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
public class NetworkManagerHUD_Custom : MonoBehaviour
{
    CustomNetworkManager manager;

    public int offsetX;
    public int offsetY;

    void Awake()
    {
        manager = GetComponent<CustomNetworkManager>();        
    }

    void OnGUI()
    {
        // If this width is changed, also change offsetX in GUIConsole::OnGUI
        int width = 400;

        GUILayout.BeginArea(new Rect(10 + offsetX, 20 + offsetY, width, 180));

        if (!NetworkClient.isConnected && !NetworkServer.active)
            StartButtons();
        else
            StatusLabels();

        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                // client ready
                NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                    NetworkClient.AddPlayer();
            }
        }

        StopButtons();

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (!NetworkClient.active)
        {
#if UNITY_WEBGL
                // cant be a server in webgl build
                if (GUILayout.Button("Single Player"))
                {
                    NetworkServer.dontListen = true;
                    manager.StartHost();
                }
#else
            // Server + Client
            if (GUILayout.Button("Host (Server + Client)"))
                manager.StartHost();
#endif

            // Client + IP (+ PORT)
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Client"))
                manager.StartClient();

            manager.networkAddress = GUILayout.TextField(manager.networkAddress);
            // only show a port field if we have a port transport
            // we can't have "IP:PORT" in the address field since this only
            // works for IPV4:PORT.
            // for IPV6:PORT it would be misleading since IPV6 contains ":":
            // 2001:0db8:0000:0000:0000:ff00:0042:8329
            if (Transport.active is PortTransport portTransport)
            {
                // use TryParse in case someone tries to enter non-numeric characters
                if (ushort.TryParse(GUILayout.TextField(portTransport.Port.ToString()), out ushort port))
                    portTransport.Port = port;
            }

            GUILayout.EndHorizontal();

            // Server Only
#if UNITY_WEBGL
                // cant be a server in webgl build
                GUILayout.Box("( WebGL cannot be server )");
#else
            /*if (GUILayout.Button("Server Only"))
                manager.StartServer();*/
#endif
        }
        else
        {
            // Connecting
            GUILayout.Label($"Connecting to {manager.networkAddress}..");
            if (GUILayout.Button("Cancel Connection Attempt"))
                manager.StopClient();
        }
    }

    void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            // host mode
            GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
        }
        else if (NetworkServer.active)
        {
            // server only
            GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
        }
        else if (NetworkClient.isConnected)
        {
            // client only
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
        }
    }

    void StopButtons()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            GUILayout.BeginHorizontal();
#if UNITY_WEBGL
                if (GUILayout.Button("Stop Single Player"))
                    manager.StopHost();
#else
            // stop host if host mode
            if (GUILayout.Button("Disconnect"))
            {
                //kjh.GameManager.Instance.player.vehicleCombat.TakeDamageExecuteCommand(99999);
                StartCoroutine(StopHostDelay(2.5f));                
            }
#endif
            GUILayout.EndHorizontal();
        }
        else if (NetworkClient.isConnected)
        {
            GUILayout.BeginHorizontal();
            // stop client if client-only
            if (GUILayout.Button("Disconnect"))
            {
                //kjh.GameManager.Instance.player.vehicleCombat.TakeDamageExecuteCommand(99999);
                StartCoroutine(StopClientDelay(2.5f));
            }

            GUILayout.EndHorizontal();
        }
        /*else if (NetworkServer.active)
        {
            // stop server if server-only
            if (GUILayout.Button("Stop Server"))
                manager.StopServer();
        }*/
    }
    
    IEnumerator StopClientDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        BackUpCam.Instance.SetActiveBackUpCam(true);
        manager.StopClient();
    }
    IEnumerator StopHostDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        BackUpCam.Instance.SetActiveBackUpCam(true);
        manager.StopClient();
        manager.StopHost();
    }
}

