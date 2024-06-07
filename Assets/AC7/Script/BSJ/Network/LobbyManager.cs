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
        StartCoroutine(UpdateRoomList());
    }
    private IEnumerator UpdateRoomList()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);

            StartCoroutine(_matchManager.GetRoomList(SetRoomList));
        }
    }

    private void SetRoomList(List<string> roomIds)
    {

        foreach (GameObject roominst in _roomInstances)
        {
            Destroy(roominst);
        }

        foreach (string roomname in roomIds)
        {
            GameObject room = Instantiate(_roomPrefab);
            room.transform.SetParent(_gridLayout.transform);
            room.GetComponentInChildren<TMPro.TMP_Text>().text = roomname;
        }
    }




}
