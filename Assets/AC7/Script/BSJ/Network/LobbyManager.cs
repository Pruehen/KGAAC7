using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private List<string> _roomList;
    [SerializeField] private List<GameObject> _roomInstances;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private GameObject _roomPrefab;

    private void Start()
    {
        UpdateRoomList();
    }
    public void UpdateRoomList()
    {
        StartCoroutine(_matchManager.GetRoomList(SetRoomList));
    }

    private void SetRoomList(List<string> roomIds)
    {

        foreach (GameObject roominst in _roomInstances)
        {
            Destroy(roominst);
        }
        _roomInstances.Clear();

        foreach (string roomname in roomIds)
        {
            GameObject room = Instantiate(_roomPrefab);
            _roomInstances.Add(room);
            room.transform.SetParent(_gridLayout.transform);
            room.GetComponentInChildren<TMPro.TMP_Text>().text = roomname;
        }
    }




}
