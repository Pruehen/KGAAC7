using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUi : MonoBehaviour
{
    [SerializeField] private string _roomId;
    [SerializeField] private TMPro.TMP_Text _roomText;
    [SerializeField] private MatchManager _matchManager;
    public string roomId { get { return _roomId; } private set {  _roomId = value; } }

    private void Start()
    {
        _roomId = _roomText.text;
    }

    private void OnButtoneDown()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        _matchManager.RequestMatch(roomId)
    }
}
