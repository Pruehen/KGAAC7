using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 문제 이펙트에 자식프리팹으로서 작동하는데 부모가 먼저 삭제될 수 도 있음
/// </summary>
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
        float distance = (Camera.main.transform.position - transform.position).magnitude;
        StartCoroutine(CheckInListhenRange());
    }

    private void PlayByDistance(float distance)
    {
        if (distance >= 2500f)
        {
            if (!(_farSfxPrefabs.Length == 0))
                PlaySound(_farSfxPrefabs[Random.Range(0, _farSfxPrefabs.Length)]);
        }
        else if (distance >= 300f)
        {
            if (!(_midSfxPrefabs.Length == 0))
                PlaySound(_midSfxPrefabs[Random.Range(0, _midSfxPrefabs.Length)]);
        }
        else
        {
            if (!(_closeSfxPrefabs.Length == 0))
                PlaySound(_closeSfxPrefabs[Random.Range(0, _closeSfxPrefabs.Length)]);
        }
    }

    private void PlaySound(GameObject sfxPrefab)
    {
        bsj.SoundManager.Instance.PlayAttached(sfxPrefab, transform);
    }

    private IEnumerator DelayPerDistance(float distance)
    {
        float delay = distance / 334f;
        yield return new WaitForSeconds(delay);
        PlayByDistance(distance);
    }

    private IEnumerator CheckInListhenRange()
    {
        float timeStamp = Time.time;
        float distanceStamp = (Camera.main.transform.position - transform.position).magnitude;
        float audioRange = 0f;
        while (true)
        {
            float distance = (Camera.main.transform.position - transform.position).magnitude;
            yield return null;
            if (distance < audioRange)
            {
                PlayByDistance(distanceStamp);
                Debug.Log("사운드 디버그" + (Time.time - timeStamp) + distanceStamp + distance + audioRange);
                break;
            }
            audioRange += Time.deltaTime * 334f;
            transform.position.DrawSphere(audioRange, Color.yellow);
        }
    }
}
