using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomOnEnable : MonoBehaviour
{
    [SerializeField] GameObject[] _sfxPrefabs;

    private void OnEnable()
    {
        StartCoroutine(AfterFrame());
         }
    private IEnumerator AfterFrame()
    {
        yield return null;
        bsj.SoundManager.Instance.PlayInPosition(_sfxPrefabs[Random.Range(0, _sfxPrefabs.Length)], transform.position);

    }
}
