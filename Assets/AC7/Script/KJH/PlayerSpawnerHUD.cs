using UnityEngine;
using Mirror;

public class PlayerSpawnerHUD : MonoBehaviour
{
    PlayerSpawner spawner;

    public int offsetX;
    public int offsetY;

    void Awake()
    {
        spawner = GetComponent<PlayerSpawner>();
    }

    void OnGUI()
    {
        // If this width is changed, also change offsetX in GUIConsole::OnGUI
        int width = 400;

        GUILayout.BeginArea(new Rect(10 + offsetX, 30 + offsetY, width, 9999));

        if (!NetworkClient.isConnected)
            StartButtons();
        //StatusLabels();

        //StopButtons();

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (!BackUpCam.Instance.gameObject.activeSelf)
            BackUpCam.Instance.SetActiveBackUpCam(true);

        GUILayout.BeginHorizontal();
        GUILayout.Button("UserName");
        spawner.UserNickName = GUILayout.TextField(spawner.UserNickName);
        GUILayout.EndHorizontal();

        GUILayout.Label($"Select : {spawner.SelectAircraftName}");
        if (GUILayout.Button("F-14A")) spawner.SetAircraft_F14();
        if (GUILayout.Button("F-15C")) spawner.SetAircraft_F15();
        if (GUILayout.Button("F-16C")) spawner.SetAircraft_F16();
        if (GUILayout.Button("MiG-29A")) spawner.SetAircraft_M29();
        if (GUILayout.Button("Su-37")) spawner.SetAircraft_S37();

        if (GUILayout.Button("Quit"))
            Application.Quit();
    }
}

