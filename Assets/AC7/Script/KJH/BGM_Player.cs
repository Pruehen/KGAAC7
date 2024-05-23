using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Player : SceneSingleton<BGM_Player>
{
    AudioSource audioSource;
    [SerializeField] float loopStart;
    [SerializeField] float loopEnd;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.Play();
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
