using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Player : SceneSingleton<BGM_Player>
{
    AudioSource audioSource;
    [SerializeField] float loopStart;
    [SerializeField] float loopEnd;

    float maxVolume;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
    }

    public void Play()
    {
        audioSource.volume = 0;
        audioSource.Play();
        StartCoroutine(audioVolumeLerpSet(maxVolume));
    }
    public void Stop()
    {
        StartCoroutine(audioVolumeLerpSet(0));
    }

    IEnumerator audioVolumeLerpSet(float targetVolume)
    {
        while (audioSource.volume != targetVolume)
        {
            yield return null;
            if(audioSource.volume < targetVolume) 
            { 
                audioSource.volume += Time.deltaTime * 0.5f;
                if (audioSource.volume >= targetVolume)
                {
                    audioSource.volume = targetVolume;
                    yield break;
                }
            }
            else
            {
                audioSource.volume -= Time.deltaTime * 0.5f;
                if (audioSource.volume <= targetVolume)
                {
                    audioSource.volume = targetVolume;
                    audioSource.Stop();
                    yield break;
                }
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            audioSource.time = loopEnd - 3;
        }

        if(audioSource.time > loopEnd)
        {
            audioSource.time = loopStart;
        }
    }
}
