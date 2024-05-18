using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomOnEnable : MonoBehaviour
{
    [SerializeField] GameObject[] _closeSfxPrefabs;
    [SerializeField] GameObject[] _midSfxPrefabs;
    [SerializeField] GameObject[] _farSfxPrefabs;

    private void OnEnable()
    {
        StartCoroutine(AfterInitPosition());
    }
    private IEnumerator AfterInitPosition()
    {
        yield return null;
        float dist = Vector3.Distance(kjh.GameManager.Instance.player.transform.position, transform.position);
        if(dist >= 1000f)
        {
            PlaySound(_farSfxPrefabs[Random.Range(0, _farSfxPrefabs.Length)]);
        }
        else if( dist >= 200f)
        {
            PlaySound(_midSfxPrefabs[Random.Range(0, _midSfxPrefabs.Length)]);
        }
        else
        {
            PlaySound(_closeSfxPrefabs[Random.Range(0, _closeSfxPrefabs.Length)]);
        }

    }
    private void PlaySound(GameObject sfxPrefab)
    {
        bsj.SoundManager.Instance.PlayInPosition(sfxPrefab, transform.position);
    }
}
