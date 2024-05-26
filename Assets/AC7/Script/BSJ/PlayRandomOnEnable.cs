using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���� ����Ʈ�� �ڽ����������μ� �۵��ϴµ� �θ� ���� ������ �� �� ����
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
        if (distance >= 5000f)
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
                float shakePower = 1f - (Mathf.Clamp01(distance / 500f));
                kjh.GameManager.Instance.cameraShake.TriggerShake(.3f * shakePower,.04f * shakePower, .01f * shakePower);
                break;
            }
            audioRange += Time.deltaTime * 1000f;
            //transform.position.DrawSphere(audioRange, Color.yellow);
            
        }
    }
}
