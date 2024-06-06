using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoConnector : MonoBehaviour
{
    [Header ("Please Setup These Value")]
    [SerializeField] private bool E_IsDedicatedServerBuild = false;
    [SerializeField] private NetworkManager E_networkManager;

    private void Start()
    {
        if(E_IsDedicatedServerBuild)
        {
            E_networkManager.StartHost();
        }
        else
        {
            //Load Multiplay AircraftScene
            SceneManager.LoadScene(1);
        }
    }
}
